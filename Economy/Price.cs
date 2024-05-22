using UniRx;
using System;

[Serializable]
public class Price
{
    public Currency currency;
    public IntReactiveProperty cost;

    public Action<int> onPay;

    public Price (Currency currency)
        : this(currency, 0)
    {
    }
    public Price(Currency currency, int cost)
    {
        this.currency = currency;
        this.cost = new IntReactiveProperty(cost);
    }

    public bool IsAffordable()
    {
        return cost.Value <= currency.value.Value;
    }

    public IObservable<bool> IsAffordableObservable()
    {
        return
            Observable
            .CombineLatest(cost, currency.value,
                           (cost, currency) => cost <= currency);
    }

    public IObservable<float> SavingProgressObservable()
    {
        return
            Observable
            .CombineLatest(cost, currency.value,
                           (cost, currency) => (float)currency / cost);
    }


    public void Pay()
    {
        currency.value.Value -= cost.Value;
        onPay?.Invoke(cost.Value);
    }

    public void GetPaid()
    {
        currency.value.Value += cost.Value;
    }
}
