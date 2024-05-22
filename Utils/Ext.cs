using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UniRx;

static public class UnityEventExtension
{
    static public ObservableYieldInstruction<Unit> TakeYield(this UnityEvent unityEvent, int count)
    {
        return unityEvent.AsObservable().Take(count).ToYieldInstruction();
    }
}

static public class GameObjectExtension
{
    static public LayerMask GetLayerMask(this GameObject go)
    {
        int layer = go.layer;
        string layerName = LayerMask.LayerToName(layer);
        LayerMask thisLayerMask = LayerMask.GetMask(layerName);

        return thisLayerMask;
    }
    public static void DelayAction(this MonoBehaviour go, float seconds, Action action)
    {
        go.StartCoroutine(InvokeDelayedAction(seconds, action));
    }

    private static IEnumerator InvokeDelayedAction(float seconds, Action action)
    {
        if (action == null) yield break;
        yield return new WaitForSeconds(seconds);
        action.Invoke();
    }
}

static public class IntegerExtension
{
    static public bool IsBetween(this int a, int startInclusive, int endExclusive)
    {
        return a >= startInclusive && a < endExclusive;
    }
}

static public class Vector3Extensions
{
    static public Vector3 AddX(this Vector3 v, float x)
    {
        return new Vector3(v.x + x, v.y, v.z);
    }
    static public Vector3 AddY(this Vector3 v, float y)
    {
        return new Vector3(v.x, v.y + y, v.z);
    }
    static public Vector3 AddZ(this Vector3 v, float z)
    {
        return new Vector3(v.x, v.y, v.z + z);
    }

    static public Vector3 WithX(this Vector3 v, float x)
    {
        return new Vector3(x, v.y, v.z);
    }
    static public Vector3 WithY(this Vector3 v, float y)
    {
        return new Vector3(v.x, y, v.z);
    }
    static public Vector3 WithZ(this Vector3 v, float z)
    {
        return new Vector3(v.x, v.y, z);
    }
}

static public class TransformExtension
{
    static public Vector3 DirectionTo(this Transform from, Transform to)
    {
        return PathTo(from, to).normalized;
    }

    static public Vector3 PathTo(this Transform from, Transform to)
    {
        return (to.position - from.position);
    }

    static public float DistanceSqrTo(this Transform from, Component to)
    {
        return (to.transform.position - from.position).sqrMagnitude;
    }

    static public float DistanceTo(this Transform from, Component to)
    {
        return Vector3.Distance(from.position, to.transform.position);
    }
    static public float DistanceTo(this Transform from, Transform to)
    {
        return Vector3.Distance(from.position, to.position);
    }
}

static public class StringExtension
{
    static public string Capitalize(this string str)
    {
        return char.ToUpper(str[0]) + str.Skip(1).JoinAsString();
    }

    static public string Encode(this string str, string key)
    {
        return Code(str, key, (a, b) => (char)(a + b));
    }

    static public string Decode(this string str, string key)
    {
        return Code(str, key, (a, b) => (char)(a - b));
    }

    static private string Code(this string str, string key, Func<char, char, char> func)
    {
        string keyHash = Hash128.Compute(key).ToString();
        Debug.Log($"keyHash: {keyHash}");

        return
            str
            .Select((strChar, i) =>
            {
                char keyChar = keyHash[i % keyHash.Length];
                return func.Invoke(strChar, keyChar);
            })
            .JoinAsString();
    }


    static public string JoinAsString(this IEnumerable<string> strs, string delimeter)
    {
        return string.Join(delimeter, strs);
    }
    static public string JoinAsString(this IEnumerable<char> chars)
    {
        return string.Join("", chars);
    }

    static public string UnLines(this IEnumerable<string> strs)
    {
        return string.Concat(strs);
    }
    static public IEnumerable<string> Lines(this string str)
    {
        var res = str.Split("\n").ToArray();
        return res;
    }

    static public IEnumerable<string> Between(this string str, string left, string right)
    {
        string s = str;

        while (s.Contains(left) && s.Contains(right))
        {
            s = s.Split(left,
                        2,
                        System.StringSplitOptions.None)
                .Last().JoinAsString();

            s = s.Split(right,
                        2,
                        System.StringSplitOptions.None)
                .First().JoinAsString();

            yield return s;
        }
    }

    private static readonly Regex sWhitespace = new Regex(@"\s+");

    public static string ReplaceWhitespace(string input, string replacement)
    {
        if (input == null)
            return "";

        return sWhitespace.Replace(input, replacement);
    }

    static public string GetFileName(this string str)
    {
        return str.Split('/').Last().Split('.').First();
    }
}

static public class Ext
{
    static private Canvas _canvas;
    static private Canvas canvas => _canvas ?? (_canvas = GameObject.FindObjectOfType<Canvas>());
           
    static public T GetRandom<T>(this List<T> list)
    {
        int id = UnityEngine.Random.Range(0, list.Count);

        return list[id];
    }

    static public Vector3 MousePositionScaledToCanvas()
    {
        return Input.mousePosition * canvas.transform.lossyScale.x;
    }

    static public Texture2D ResizeTexture(Texture2D texture2D,int targetX,int targetY)
    {
        RenderTexture rt = new RenderTexture(targetX, targetY, 24);
        RenderTexture.active = rt;

        Graphics.Blit(texture2D, rt);
       
        Texture2D result = new Texture2D(targetX, targetY);
        result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
        result.Apply();

        return result;
    }
}

static public class GridExt
{

    static public void HexXBorders(int radius, int y, out int left, out int right)
    {
        int absY = Math.Abs(y);

        left = -radius + (absY)  /2;
        right = radius - (absY+1)/2;
    }
}

static public class Vector3Ext
{
    static public Vector2 xy(this Vector3 v3) => new Vector2(v3.x, v3.y);
    static public Vector2 xz(this Vector3 v3) => new Vector2(v3.x, v3.z);
}

static public class Vector2Ext
{
    static public Vector3 x_z(this Vector2 v2, float y = 0) => new Vector3(v2.x, y, v2.y);

    static public Vector3 xy_(this Vector2 v2, float z = 0) => new Vector3(v2.x, v2.y, z);

    static public Vector2Int FloorToVector2Int(this Vector2 v2) => new Vector2Int(Mathf.FloorToInt(v2.x), Mathf.FloorToInt(v2.y));
}

static public class Vector3IntExt
{
    static public Vector2Int xy(this Vector3Int v3) => new Vector2Int(v3.x, v3.y);
}

static public class Vector2IntExt
{
    static public Vector3Int xy_(this Vector2Int v2, int z = 0) => new Vector3Int(v2.x, v2.y, z);
    static public void Deconstruct(this Vector2Int v2, out int x, out int y)
    {
        x = v2.x;
        y = v2.y;
    }
}

static public class ArrayExt
{
    static public T ArrayGetRandom<T>(this System.Array array)
    {
        int id = UnityEngine.Random.Range(0, array.Length);

        return (T) array.GetValue(id);
    }
}

static public class IntExt
{
    static public int Concat(this int i, string numberString)
    {
        if (!int.TryParse(numberString, out int result))
        {
            throw new System.ArgumentException($"String {numberString} is not a number");
        }

        return int.Parse(i.ToString() + numberString);
    }

    static public int MaybeParse(string str, int defaultValue)
    {
        if (int.TryParse(str, out int result))
        {
            return result;
        }
        else
        {
            return defaultValue;
        }
    }

    static public IEnumerable<TResult> PairSelect<T, TResult>(this IEnumerable<T> source,
                                                              Func<(T a, T b), TResult> func)
    {
        int count = source.Count() - 1;
       
        return
            IEnumerableExt
            .Zip(source.Take(count),
                 source.TakeLast(count))
            .Select(tup =>
                    func.Invoke((tup.Item1, tup.Item2)));
    }


    static public void ForPairLoop(this int i, Action<int> forAction, Action<int,int> pairAction)
    {
        Enumerable.Range(0, i).Map(forAction);

        IEnumerableExt.
            Zip(Enumerable.Range(0, i-1),
                Enumerable.Range(1, i))
            .Map(tup =>
                 pairAction(tup.Item1, tup.Item2));
    }

    static public void ForLoop(this int i, Action<int> action)
    {
        Enumerable.Range(0, i).ToList().ForEach(action);
    }
    static public IEnumerable<int> ToRange(this int i)
    {
        return Enumerable.Range(0, i);
    }
}

static public class IEnumerableExt
{
    public static IEnumerable<T> Scan<T>(this IEnumerable<T> source,
                                         Func<T, T, T> scanner)
    {
        return
            source
            .Skip(1)
            .Aggregate(new [] { source.First() }.AsEnumerable(),
                       (acum, border) =>
                       acum.Append( scanner.Invoke(acum.Last(), border) ));
    }

    public static IEnumerable<IEnumerable<T>> Chunks<T>(this IEnumerable<T> source, int size)
    {
        return
            source
            .Select((obj, i) => (obj, chunkIndex: i / size))
            .GroupBy(tuple => tuple.chunkIndex)
            .Select(gr =>
                    gr.Select(tuple => tuple.obj));
    }

    public static void DestroyAll<T>(this IEnumerable<T> source)
        where T : Component
    {
        foreach (var item in source) MonoBehaviour.Destroy(item.gameObject);
    }

    public static void DestroyAllImmediate<T>(this IEnumerable<T> source)
        where T : Component
    {
        foreach (var item in source) MonoBehaviour.DestroyImmediate(item.gameObject);
    }

    public static void DestroyAllImmediate(this IEnumerable<GameObject> source)
    {
        foreach (var item in source) GameObject.DestroyImmediate(item);
    }

    static public T GetRandom<T>(this IEnumerable<T> source)
    {
        int id = UnityEngine.Random.Range(0, source.Count());

        return source.ElementAt(id);
    }

    static public IEnumerable<T> GetRandoms<T>(this IEnumerable<T> source, int count)
    {
        for (int i = 0; i < count; i++)
        {
            int id = UnityEngine.Random.Range(0, source.Count());

            yield return source.ElementAt(id);
        }
    }


    static public IEnumerable<T> Takeout<T>(this IEnumerable<T> source, out IEnumerable<T> takeout, Func<T, bool> predicate)
    {
        List<T>
            takeoutList = new List<T>(),
            stayList = new List<T>();

        foreach (var item in source)
        {
            bool v = predicate.Invoke(item);
            if (v)
                takeoutList.Add(item);
            else
                stayList.Add(item);
        }

        takeout = takeoutList;

        return stayList;
    }
    static public IEnumerable<T> Takeout<T>(this IEnumerable<T> source, out IEnumerable<T> takeout)
    {
        takeout = source;
        return source;
    }


    static public IEnumerable<T> Separate<T>(this IEnumerable<T> source, Func<T, bool> predicate, out IEnumerable<T> fals)
    {
        fals = source.Where(item => predicate.Invoke(item) == false);
        return source.Where(item => predicate.Invoke(item) == true);
    }

    static public IEnumerable<T> MultiplyByField<T>(this IEnumerable<T> source, Func<T, int> fieldSelector)
    {
        return source
            .SelectMany(obj =>
                        Enumerable
                        .Range(0, fieldSelector.Invoke(obj))
                        .Select(_ => obj));
    }
    static public Dictionary<TKey, TValue> ToDict<TKey, TValue>(this IEnumerable<(TKey, TValue )> source)
    {
        return source.ToDictionary(kv => kv.Item1,
                                   kv => kv.Item2);
    }
    static public Dictionary<TKey, TValue> ToDict<TKey, TValue>(this IEnumerable<Tuple<TKey, TValue>> source)
    {
        return source.ToDictionary(kv => kv.Item1,
                                   kv => kv.Item2);
    }
    static public Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
    {
        return source.ToDictionary(kv => kv.Key,
                                   kv => kv.Value);
    }
    static public IEnumerable<( int, T )> Enumerate<T>(this IEnumerable<T> source)
    {
        int i = 0;
        foreach (var item in source)
        {
            yield return (i, item);
            i++;
        }
    }
    static public IEnumerable<T> UnionBy<T>(this ICollection<T> source,
                                            IEnumerable<T> other,
                                            Func<T, object> fieldSelector)
    {
        other.Where(o =>
                    source.None(s =>
                                fieldSelector.Invoke(o) == fieldSelector.Invoke(s)))
            .Map(source.Add);

        return source;
    }

    static public IEnumerable<TResult> WhereCast<TResult>(this IEnumerable source)
    {
        foreach (var item in source)
        {
            if (item is TResult)
                yield return (TResult)item;
            else
                continue;
        }
    }
    static public bool None<T>(this IEnumerable<T> source)
    {
        return source == null || !source.Any();
    }
    static public bool None<T>(this IEnumerable<T> source, Func<T, bool> pred)
    {
        return !source.Any(pred);
    }

    static public IEnumerable<T> Map<T>(this IEnumerable<T> source, Action<T> action)
    {
        if (source.None())
        {
            return source;
        }

        foreach (var item in source)
        {
            if (item == null) continue;
            action.Invoke(item);
        }

        return source;
    }
    static public void Split<T>(this IEnumerable<T> source, Func<T, bool> predicate, out IEnumerable<T> a, out IEnumerable<T> b)
    {
        a = source.Where(predicate);
        b = source.Where(item => predicate.Invoke(item) == false);
    }
    static public IEnumerable<(T, O)> Zip<T, O>(this IEnumerable<T> source, IEnumerable<O> other)
    {
        return source.Zip(other, (a , b) => (a, b));
    }
    static public IEnumerable<(T, O, E)> Zip<T, O, E>(this IEnumerable<T> source, IEnumerable<O> other, IEnumerable<E> elseOther)
    {
        return source.Zip(other, (a , b) => (a, b)).Zip(elseOther, (a, b) => (a.a, a.b, b));
    }
    static public IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.OrderBy(_ => UnityEngine.Random.value);
    }
    static public T GetRandomOrThrow<T>(this IEnumerable<T> source)
    {
        int count = source.Count();

        if (count == 0)
            throw new System.Exception("No items in collection!");

        int id = UnityEngine.Random.Range(0, count);

        return source.ElementAt(id);
    }
    static public T GetRandomOrDefault<T>(this IEnumerable<T> source)
    {
        int count = source.Count();

        if (count == 0)
            return default;

        int id = UnityEngine.Random.Range(0, count);

        return source.ElementAt(id);
    }
        static public IObservable<T> WhereEqual<T>(this IObservable<T> source, T other)
    {
        return source.Where(item => item.Equals(other));
    }
    static public IObservable<T> WhereNotEqual<T>(this IObservable<T> source, T other)
    {
        return source.Where(item => !item.Equals(other));
    }

    public static IObservable<T> WhereNotNull<T>(this IObservable<T> source)
        where T : class
    {
        return source.Where(item => item != null);
    }
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> source)
        where T : class
    {
        return source.Where(item => item != null);
    }

    static public IEnumerable<T> NotEqual<T>(this IEnumerable<T> source, T other)
    {
        return source.Where(item => !item.Equals(other));
    }
    static public IEnumerable<T> Log<T>(this IEnumerable<T> source, string prefixMessage="")
    {
        string str =
            (!source.Any())
            ? "Empty"
            : source
            .Select(item => item?.ToString() ?? "NULL")
            .Aggregate((a, b) => a + ", " + b);

        Debug.Log(prefixMessage + ": " + str);

        return source;
    }

}
static public class ListExtension
{
    public static void DestroyAll<T>(this List<T> source)
        where T : Component
    {
        foreach (var item in source) MonoBehaviour.Destroy(item.gameObject);
        source.Clear();
    }

    public static IEnumerable<T> ResizeDestructive<T>(this List<T> list, int newSize)
    {
        return list.ResizeDestructive(newSize, () => default, null);
    }

    public static List<T> ResizeDestructive<T>(this List<T> list, int newSize, Func<T> onAdd, Action<T> onRemove)
    {
        int size = list.Count();

        if (size == newSize)
            return list;

        if (newSize < size)
        {
            if (onRemove != null)
                list
                    .Skip(newSize)
                    .Where(item => item != null)
                    .Map(onRemove);

            list = list.Take(newSize).ToList();
        }
        else if (newSize > size)
        {
            var addition =
                (newSize - size)
                .ToRange()
                .Select(_ => onAdd.Invoke());

            list = list.Concat(addition).ToList();
        }

        return list;
    }
}

static public class EnumerableBoolExt
{
    static public IEnumerable<bool> IsFalse(this IEnumerable<bool> source)
    {
        return source.Where(b => b == false);
    }
    static public IEnumerable<bool> IsTrue(this IEnumerable<bool> source)
    {
        return source.Where(b => b == true);
    }
}


static public class MathExt
{
    static public int Fact(int n)
    {
        return
            Enumerable
            .Range(1, n)
            .Aggregate((a , b) => a * b);
    }
}

public static class BoolExt
{
    static public bool MaybeParse(string str, bool defaultValue)
    {
        if (bool.TryParse(str, out bool result))
        {
            return result;
        }
        else
        {
            return defaultValue;
        }
    }

    static public bool All(bool a, bool b, bool c)
    {
        return a && b && c;
    }
    static public bool All(bool a, bool b)
    {
        return a && b;
    }
}




public static class MonoBehaviourExtension
{
    static public IEnumerator InvokeWhen(this GameObject mono, Func<bool> whenPredicate, Action action)
    {
        while (!whenPredicate.Invoke())
        {
            yield return null;
        }

        action.Invoke();
    }
    public static Coroutine StartInvokeWhen(this MonoBehaviour mono, Func<bool> whenPredicate, Action action)
    {
        return mono.StartCoroutine(mono.InvokeWhen(whenPredicate, action));
    }

    static public IEnumerator InvokeWhen(this MonoBehaviour mono, Func<bool> whenPredicate, Action action)
    {
        do
        {
            yield return null;
        }
        while (!whenPredicate.Invoke());

        action.Invoke();
    }

    public static Coroutine StartInvokeAfter(this MonoBehaviour mono, Action action, float seconds)
    {
        return mono.StartCoroutine(CoroutineExtension.InvokeAfter( action,  seconds));
    }

    public static Coroutine StartSearchForObjectOfType<T>(this MonoBehaviour mono, Action<T> onFound)
        where T : Component
    {
        return mono.StartCoroutine(CoroutineExtension.SearchForObjectOfType<T>(onFound));
    }

    public static Coroutine StartWaitWhile(this MonoBehaviour mono, Func<bool> predicate)
    {
        return mono.StartCoroutine(CoroutineExtension.WaitWhile(predicate));
    }
}

public static class CoroutineExtension
{
    public static System.Collections.IEnumerator InvokeAfter(Action action, float seconds)
    {
        var wait = new WaitForEndOfFrame();

        while ((seconds -= Time.deltaTime) > 0f) yield return wait;

        action.Invoke();
    }
    public static IEnumerator SearchForObjectOfType<T>(Action<T> onFound)
        where T : Component
    {
        T obj = null;
        do
        {
            yield return new WaitForEndOfFrame();

            obj = GameObject.FindObjectOfType<T>();

            if (obj != null)
            {
                onFound.Invoke(obj);
                yield break;
            }
        }
        while (obj == null);
    }

    public static IEnumerator WaitWhile(Func<bool> predicate)
    {
        while (predicate.Invoke())
        {
            yield return new WaitForEndOfFrame();
        }
    }
}

public static class CanvasGroupExtension
{
    static public void SetVisibleAndInteractable(this CanvasGroup canvasGroup, bool visible)
    {
        canvasGroup.alpha = (visible) ? 1f : 0f;
        SetInteractable(canvasGroup, visible);
    }

    private static void SetInteractable(this CanvasGroup canvasGroup, bool visible)
    {
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }
}
