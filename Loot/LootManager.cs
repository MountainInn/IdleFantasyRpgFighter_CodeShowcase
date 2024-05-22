using UnityEngine;
using Zenject;
using UniRx;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using System.Collections.Generic;

public class LootManager : MonoBehaviour
{
    NominalParticles nominalParticles;
    [Inject] GameSettings gameSettings;
    [Inject] Vault vault;
    [Inject] DiContainer Container;

    public void SubscribeForProps(UnityEvent<IEnumerable<DropList.Entry>> eventToDrop,
                                  NominalParticles nominalParticles,
                                  GameObject callingGameObject)
    {
        InitializeNominalParticles(nominalParticles, maybePosition: null);

        eventToDrop
            .AsObservable()
            .Subscribe(async loot =>
            {
                foreach (var item in loot)
                {
                    DropGold(item.drop.currency.cost.Value,
                             nominalParticles);

                    await UniTask.WaitForSeconds(gameSettings.intervalBetweenDrops);
                }
            })
            .AddTo(callingGameObject);
    }

    void InitializeNominalParticles(NominalParticles nominalParticles, Vector3? maybePosition)
    {
        foreach (var field in nominalParticles.Fields)
        {
            if (maybePosition.HasValue)
            {
                field.particles.transform.position =
                    maybePosition.Value + new Vector3(0, 0.5f, 0);
            }

            field.particles
                .onParticleHitCharacter
                .AddListener(() => LootGold(field.amount));
        }
    }

    int goldMargin;

    void DropGold(int amount, NominalParticles nominalParticles)
    {
        int range = 2;

        foreach (var field in nominalParticles.Fields)
        {
            if (amount <= 0)
            {
                goldMargin = amount;
                break;
            }

            int ceil = Mathf.CeilToInt((float)amount / field.amount);
            int count;

            if (field == nominalParticles.Fields.Last())
            {
                count = ceil;
            }
            else
            {
                int floor = Mathf.Max(0, ceil - range);
                count = UnityEngine.Random.Range(floor, ceil+1);
            }

            count.ForLoop(_ => field.particles.Emit());

            amount -= field.amount * count;
        }
    }

    void LootGold(int amount)
    {
        if (goldMargin != 0)
        {
            amount += goldMargin;
            goldMargin = 0;
        }

        vault.gold.value.Value += amount;
    }
}
