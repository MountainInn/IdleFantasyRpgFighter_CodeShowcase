using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Zenject;

public class ProjectileParticles : MonoBehaviour
{
    [SerializeField] ParticleSystem ps;
    [SerializeField] public UnityEvent onParticleHitCharacter;

    [Inject]
    public void Construct(BoxCollider characterCollider)
    {
        ps.trigger.AddCollider(characterCollider);
    }

    public void OnParticleTrigger()
    {
        List<ParticleSystem.Particle> enter = new();

        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);

        for (int i = 0; i < numEnter; i++)
        {
            onParticleHitCharacter.Invoke();
        }
    }
}
