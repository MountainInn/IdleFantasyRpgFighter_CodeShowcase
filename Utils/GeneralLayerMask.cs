#if UNITY_EDITOR
#endif

using UnityEngine;

[System.SerializableAttribute]
public class GeneralLayerMask
{
    public LayerMask layers = int.MaxValue;

    internal bool Check(int otherLayer)
    {
        return 0 != (layers.value & (1 << otherLayer));
    }
}
