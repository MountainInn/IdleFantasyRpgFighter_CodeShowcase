using System.Linq;
using UnityEngine;
using Zenject;
using System.Collections.Generic;
using System;

public abstract class BaseInstaller : MonoInstaller
{
    [SerializeField] protected RuntimeAnimatorController characterAnimatorController;
    [Space]
    [SerializeField] protected Canvas canvas;
    // [Space]
    // [SerializeField] protected ParticleSystemForceField particleSystemForce;

    override public void InstallBindings()
    {
        Container
            .Bind<Canvas>()
            .FromInstance(canvas)
            .AsCached();

        // Container
        //     .Bind<ParticleSystemForceField>()
        //     .FromInstance(particleSystemForce)
        //     .AsSingle();

    }

    protected List<T> InstantiateSOs<T>(string path)
        where T : ScriptableObject
    {
        var objects = Resources.LoadAll<T>(path);

        return
            objects
            .Select(t => Instantiate(t))
            .Map(Container.Inject)
            .ToList();
    }
}
