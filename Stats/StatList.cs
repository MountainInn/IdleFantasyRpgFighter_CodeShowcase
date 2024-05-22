using System;
using System.Collections.Generic;

[Serializable]
public class StatList : List<float>
{
    public List<float> AsList()
    {
        return this as List<float>;
    }
}
