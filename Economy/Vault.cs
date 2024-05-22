using UnityEngine;
using System;
using UniRx;
using Zenject;

public class Vault : MonoBehaviour
{
    static public Vault instance => _inst ??= FindObjectOfType<Vault>();
    static Vault _inst;

    [SerializeField] public Currency gold;

    [Inject] SaveSystem saveSystem;
    [Inject]
    void RegisterWithSaveSystem()
    {
        saveSystem
            .MaybeRegister<int>(this,
                                "gold",
                                () => gold.value.Value,
                                (val) => gold.value.Value = val);
    }

    [Inject]
    public void SubscribeToCheats(Cheats cheats)
    {
        int cachedGold = gold.value.Value;
        IDisposable moneySubscription = null;

        cheats.infinitMoney
            .Subscribe(toggle =>
            {
                if (toggle)
                {
                    cachedGold = gold.value.Value;

                    moneySubscription =
                        gold
                        .ObserveChange()
                        .StartWith(int.MaxValue)
                        .Subscribe(_ => gold.value.Value = int.MaxValue);
                }
                else
                {
                    moneySubscription?.Dispose();
                    moneySubscription = null;

                    gold.value.Value = cachedGold;
                }
            })
            .AddTo(this);
    }

    void Awake()
    {
        gold.value.Value = 0;
    }
}
