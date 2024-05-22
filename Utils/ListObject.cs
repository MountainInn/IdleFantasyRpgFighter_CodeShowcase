using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

[Serializable]
public class ListObject<T> : IList<T>
{
    [SerializeField] public List<T> list;

    public ListObject()
    {
        list = new();
    }

    static public implicit operator List<T>(ListObject<T> listObj)
    {
        return listObj.list;
    }

    static public implicit operator ListObject<T>(List<T> list)
    {
        return new ListObject<T>() { list = list };
    }


    public T this[int index] { get => ((IList<T>)list)[index]; set => ((IList<T>)list)[index] = value; }

    public int Count => ((ICollection<T>)list).Count;

    public bool IsReadOnly => ((ICollection<T>)list).IsReadOnly;

    public void Add(T item)
    {
        ((ICollection<T>)list).Add(item);
    }

    public void Clear()
    {
        ((ICollection<T>)list).Clear();
    }

    public bool Contains(T item)
    {
        return ((ICollection<T>)list).Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        ((ICollection<T>)list).CopyTo(array, arrayIndex);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)list).GetEnumerator();
    }

    public int IndexOf(T item)
    {
        return ((IList<T>)list).IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        ((IList<T>)list).Insert(index, item);
    }

    public bool Remove(T item)
    {
        return ((ICollection<T>)list).Remove(item);
    }

    public void RemoveAt(int index)
    {
        ((IList<T>)list).RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)list).GetEnumerator();
    }
}
