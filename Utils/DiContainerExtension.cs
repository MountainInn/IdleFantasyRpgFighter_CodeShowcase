using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

static public class DiContainerExtension
{
    public static void BindSOs<T>(this DiContainer Container, params string[] paths)
        where T : ScriptableObject
    {
        var objects =
            paths
            .SelectMany(Resources.LoadAll<T>)
            .ToList();

        var boundTypes =
            objects
            .Select(o =>
            {
                Type type = o.GetType();

                Container
                    .BindInterfacesAndSelfTo(type)
                    .FromNewScriptableObject(o)
                    .AsSingle()
                    .NonLazy();

                return type;
            });

        Container
            .Bind<List<T>>()
            .FromMethod(() =>
            {
                return
                    boundTypes
                    .Select(t => Container.Resolve(t))
                    .Cast<T>()
                    .ToList();
            })
            .AsSingle();
    }



    public static ConditionCopyNonLazyBinder BindView<T>(this DiContainer Container, T prefabView, Transform parent)
        where T : Component
    {
        return
            Container
            .Bind<T>()
            .FromComponentInNewPrefab(prefabView)
            .AsTransient()
            .OnInstantiated<T>((ctx, view) =>
            {
                view.transform.SetParent(parent);
                view.transform.localScale = Vector3.one;
            });
    }
}
