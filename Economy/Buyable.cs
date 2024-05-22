using System.Linq;
using UniRx;
using System.Collections.Generic;
using System;
using UniRx.Diagnostics;

public class Buyable<T>
    where T : Buyable<T>.IWare
{
    public List<Price> prices;
    public Action<T> onBuy;
    public T ware;

    public Buyable(T ware, Action<T> onBuy, params Price[] prices)
    {
        this.prices = prices.ToList();
        this.ware = ware;
        this.onBuy = onBuy;
    }

    public bool IsAffordable()
    {
        return prices.All(price => price.IsAffordable());
    }

    public IObservable<bool> ObserveIsAffordable()
    {
        return
            Observable
            .CombineLatest(
                prices
                .Select(price => price.IsAffordableObservable())
                .Append(ware.ObserveIsAffordable())
            )
            .Select(affordables => affordables.All(a => a == true));
    }

    public IObservable<float> SavingProgressObservable()
    {
        return
            Observable
            .CombineLatest(
                prices
                .Select(price => price.SavingProgressObservable())
            )
            .Select(savings =>
                    savings.Sum() / prices.Count);
    }

    public void Buy()
    {
        prices.Map(price => price.Pay());
        onBuy?.Invoke(ware);
    }

    public interface IWare
    {
        public IObservable<bool> ObserveIsAffordable();
    }
}
