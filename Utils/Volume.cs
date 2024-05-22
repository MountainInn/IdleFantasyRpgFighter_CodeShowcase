using UnityEngine;
using System.Linq;
using UniRx;
using System;

[System.SerializableAttribute]
public class Volume
{
    public FloatReactiveProperty maximum => _maximum ??= new();
    public FloatReactiveProperty current => _current ??= new();

    [SerializeField]
    protected FloatReactiveProperty _current, _maximum;


    public float Unfilled => maximum.Value - current.Value;


    public Volume() {}
    public Volume(float currentAndMaximum)
        :this(currentAndMaximum, currentAndMaximum)
    {

    }

    public Volume(float current, float maximum)
    {
        this._current = new FloatReactiveProperty(current);
        this._maximum = new FloatReactiveProperty(maximum);
    }

    public IObservable<(float current, float maximum, float ratio)> ObserveAll() =>
        ObserveRatio()
        .Select(ratio => (current.Value, maximum.Value, ratio));

    public IObservable<float> ObserveRatio() =>
        Observable.CombineLatest(
            current, maximum,
            (cur , max) => (Ratio));


    public IObservable<bool> ObserveFull() =>
        Observable.CombineLatest(
            current, maximum,
            (cur , max) => IsFull);

    public IObservable<bool> ObserveEmpty() =>
        current
        .Select(v => IsEmpty);

    public IObservable<bool> ObserveRefill() =>
        ObserveFull()
        .Pairwise()
        .Select(fulls =>
                fulls.Previous == false && fulls.Current == true);

    public IObservable<float> ObserveChange() =>
        current
        .Pairwise((a, b) => b - a);

    public bool IsFull => (current.Value == maximum.Value);
    public bool IsEmpty => (current.Value == 0);

    public void ResetToZero()
    {
        current.Value = 0;
    }

    public float Ratio
    {
        get {
            float ratio = (current.Value / maximum.Value);

            if (float.IsNaN(ratio))
                ratio = 0;

            return ratio;
        }
    }

        public void ResetTo(float newCurrentAmount)
        {
            current.Value = newCurrentAmount;
        }

    public void Add(float amount, out float overflow)
    {
        overflow = 0;

        if (amount > Unfilled)
            overflow = amount - Unfilled;

        Add(amount);
    }
    public void Add(float amount)
    {
        amount = Mathf.Min(amount, Unfilled);
        current.Value = Mathf.Clamp(current.Value + amount,
                                    0, maximum.Value);
    }

    public void Subtract(float amount, out float overflow)
    {
        overflow = 0;

        if (amount > current.Value)
            overflow = amount - current.Value;

        Subtract(amount);
    }

    public void Subtract(float amount)
    {
        current.Value -= Mathf.Min(amount, current.Value);
    }

    public void Reinitialize(float current, float maximum)
    {
        this.current.Value = current;
        this.maximum.Value = maximum;
    }

    public void ResizeAndRefill(float newMaximum)
    {
        maximum.Value = newMaximum;
        current.Value = newMaximum;
    }

    public void Resize(float newMaximum)
    {
        maximum.Value = newMaximum;
        current.Value = Clamp(current.Value);
    }

    public void Refill()
    {
        current.Value = maximum.Value;
    }

    float Clamp(float amount)
    {
        return Mathf.Clamp(amount, 0, maximum.Value);
    }

    public override string ToString()
    {
        return $"{current.Value:0}/{maximum.Value:0}";
    }
}
