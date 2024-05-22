using UnityEngine;
using Zenject;
using System;
using UniRx;

public class VaultView : MonoBehaviour
{
    [SerializeField] public CurrencyView goldView;
    [SerializeField] public FloatingTextSpawner floatingTextSpawner;

    [Inject]
    public void Construct(Vault vault)
    {
        goldView.InitAndSubscribe(vault.gold);

        if (floatingTextSpawner)
            vault.gold
                .ObserveChange()
                .Subscribe(change =>
                {
                    floatingTextSpawner.Float(change.ToString());
                })
                .AddTo(this);
    }
}
