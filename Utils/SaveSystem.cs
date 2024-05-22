using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using UnityEngine;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;
using Unity.Services.CloudSave.Models;

public class SaveSystem : MonoBehaviour
{
    static readonly string KEY = "Tut-turu";

    Dictionary<string, object> subs = new();

    Dictionary<string, Action> savers = new();
    Dictionary<string, Action> loaders = new();
    Dictionary<object, Action> loadCallbacks = new();

    HashSet<string> keys = new();
    Dictionary<string, object> saveDict = new();
    Dictionary<string, Unity.Services.CloudSave.Models.Item> loadDict = new();

    public void MaybeRegister<T>(object sub, string key, Func<T> getter, Action<T> setter,
                                 Action afterLoad = null)
    {
        if (!keys.Contains(key))
            keys.Add(key);

        /// TODO: Сделать проверку на повторяющиеся ключи,
        /// TODO: чтобы не сделать один и тот же ключи двум разным объектам

        bool isPresent = subs.TryGetValue(key, out object cachedSub);
        //
        // Same null check as in PopulateSaveDict()
        //
        bool isEqualsNull = (cachedSub?.Equals(null) ?? true);
        bool isExpired = (isPresent && isEqualsNull);

        if (isExpired || !isPresent)
        {
            subs[key] = sub;

            savers[key] = () =>
            {
                saveDict[key] = getter.Invoke();
            };

            loaders[key] = () =>
            {
                if (loadDict.TryGetValue(key, out Item loadedItem))
                    setter.Invoke(loadedItem.Value.GetAs<T>());
            };

            if (afterLoad != null)
            {
                loadCallbacks[sub] = afterLoad;
            }
        }
    }

    void PopulateSaveDict()
    {
        saveDict = new();

        foreach (var (k, v) in savers)
        {
            //______________________________________
            // This is not enough
            // to check object for null for some reason:
            //
            // bool isNotNull = comp != null;
            //______________________________________

            object sub = subs[k];
            // Better
            bool isNotEqualsNull = (!sub?.Equals(null) ?? false);

            if (isNotEqualsNull)
            {
                v.Invoke();
            }
        }
    }

    void ProcessLoadDict()
    {
        foreach (var (k, v) in loaders)
            if (subs[k] != null)
                v.Invoke();
    }

    void CallbackAfterLoad()
    {
        foreach (var (k, v) in loadCallbacks)
            if (k != null)
                v.Invoke();
    }

    async UniTask MaybeInitialize()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized)
            return;

        if (UnityServices.State == ServicesInitializationState.Initializing)
        {
            Debug.Log($"Initializing...");
            await UniTask.WaitUntil(() =>
                                    UnityServices.State == ServicesInitializationState.Initialized);
            return;
        }

        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            Debug.Log($"Start Initialization");

            await UnityServices.InitializeAsync();
        }

        if (AuthenticationService.Instance.IsExpired ||
            !AuthenticationService.Instance.IsAuthorized)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public async UniTask Save()
    {
        // await MaybeInitialize();

        PopulateSaveDict();

        Debug.Log("Saved: " + string.Join(",", saveDict));


        // var saved = await CloudSaveService.Instance.Data.Player.SaveAsync(saveDict);

        // Debug.Log("Saved: " + string.Join(",", saved));
    }

    public async UniTask Load()
    {
        // await MaybeInitialize();

        // loadDict = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

        Debug.Log("Loaded: " + string.Join(",", loadDict));

        ProcessLoadDict();

        CallbackAfterLoad();
    }
}
