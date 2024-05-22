using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using Zenject;
using UniRx;
using System.Linq;
using UnityEngine.Events;
using System.Collections.Generic;

[RequireComponent(typeof(TrashTrigger))]
[RequireComponent(typeof(FaceTarget))]
public class Prop : MonoBehaviour
{
    [SerializeField] ParticleSystem interactablePS;
    [SerializeField] ParticleSystem onInteractedPS;
    [SerializeField] NominalParticles nominalParticles;
    [Space]
    [SerializeField] BoxCollider trigger;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] PropSO propSO;
    [Space]
    [SerializeField] UnityEvent<IEnumerable<DropList.Entry>> onLootCollected;
    [SerializeField] UnityEvent onInteractableClicked;

    [Inject] Character character;
    [Inject] void SubscribeToLootManager(LootManager lootManager)
    {
        lootManager.SubscribeForProps(onLootCollected, nominalParticles, gameObject);
    }

    bool interactable;
    IEnumerable<DropList.Entry> loot;

    void OnValidate()
    {
        if (spriteRenderer != null && propSO != null)
        {
            spriteRenderer.sprite = propSO.sprite;
        }
    }

    void RollLoot()
    {
        loot = propSO.dropList.Roll();

        if (loot.Any())
        {
            SetInteractable(true);
        }
    }

    void SetInteractable(bool interactable)
    {
        this.interactable = interactable;

        trigger.enabled = interactable;

        if (interactable)
            interactablePS.Play();
        else
            interactablePS.Stop();
    }

    void OnMouseDown()
    {
        Interact();
    }

    void Interact()
    {
        if (!interactable)
            return;

        onInteractedPS.Play();

        if (propSO.healingAmount > 0)
        {
            character.health.Add(propSO.healingAmount);
        }
        if (propSO.staminaAmount > 0)
        {
            character.stamina.Add(propSO.staminaAmount);
        }
        if (propSO.manaAmount > 0)
        {
            character.mana.Add(propSO.manaAmount);
        }

        if (loot == null)
            return;

        onLootCollected?.Invoke(loot.ToList());

        SetInteractable(false);
        loot = null;
    }

    public class Pool : PoolOnTreadmill<Prop>
    {
        protected override void OnCreated(Prop item)
        {
            base.OnCreated(item);

            item.gameObject
                .AddComponent<TrashTrigger>()
                .onEnterCleaner += () => Despawn(item);
        }

        protected override void Reinitialize(Vector3 position, Prop item)
        {
            base.Reinitialize(position, item);
            item.RollLoot();
        }

        protected override void OnSpawned(Prop item)
        {
            base.OnSpawned(item);
        }
    }
}
