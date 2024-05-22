using Unity.Mathematics;

static public class MathematicsExtension
{
    static public float3 AddX(this float3 v, float x)
    {
        return new float3(v.x + x, v.y, v.z);
    }
    static public float3 AddY(this float3 v, float y)
    {
        return new float3(v.x, v.y + y, v.z);
    }
    static public float3 AddZ(this float3 v, float z)
    {
        return new float3(v.x, v.y, v.z + z);
    }

    static public float3 WithX(this float3 v, float x)
    {
        return new float3(x, v.y, v.z);
    }
    static public float3 WithY(this float3 v, float y)
    {
        return new float3(v.x, y, v.z);
    }
    static public float3 WithZ(this float3 v, float z)
    {
        return new float3(v.x, v.y, z);
    }
}
