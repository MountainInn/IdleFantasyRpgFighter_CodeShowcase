using System;

[Serializable]
public struct Alias
{
    public string Value;

    public Alias(string value)
    {
        Value = value;
    }

    static public implicit operator string(Alias alias) => alias.Value;
    static public implicit operator Alias(string val) => new Alias(val);
}
