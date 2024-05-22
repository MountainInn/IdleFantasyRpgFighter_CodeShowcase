using System;
using UniRx;

public sealed class Modifier
{
    public BoolReactiveProperty Enabled;
    public float Value;

    public Modifier()
    {
        this.Value = 1f;
    }

    static public implicit operator float(Modifier modifier)
    {
        return modifier.Value;
    }
}
