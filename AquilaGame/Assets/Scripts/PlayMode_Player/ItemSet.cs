using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public string Name;
    public string Log;
    public int count;
    public Item(int cnt, string name, string log)
    {
        count = cnt;
        Name = name;
        Log = log;
    }
    public Item(Item i)
    {
        count = i.count;
        Name = i.Name;
        Log = i.Log;
    }
    public static bool operator ==(Item it1, Item it2)
    {
        if (it1 as object == null || it2 as object == null)
            return Equals(it1, it2);
        return it1.Name == it2.Name && it1.Log == it2.Log;
    }
    public static bool operator !=(Item it1, Item it2)
    {
        if (it1 as object == null || it2 as object == null)
            return !Equals(it1, it2);
        return it1.Name != it2.Name || it1.Log != it2.Log;
    }
    public override bool Equals(object obj)
    {
        if (obj.GetType() != GetType() || Name != ((Item)obj).Name || Log != ((Item)obj).Log)
            return false;
        return true;
    }
    public override int GetHashCode()
    {
        return (Name + Log).GetHashCode();
    }
    public override string ToString()
    {
        return count.ToString() + '\n' + Name +'\n' + Log;
    }
}

public class ItemList
{
    public List<Item> items = new List<Item>();
    public void Add(Item item)
    {
        int index = items.IndexOf(item);
        if (index == -1)
            items.Add(item);
        else
            items[index].count += item.count;
    }
    public void Use(Item item)
    {
        if (item == null)
            return;
        item.count--;
        if (item.count <= 0)
        {
            items.Remove(item);
        }
    }
    public void Remove(Item item)
    {
        items.Remove(item);
    }

    public int Count { get { return items.Count; } }
}
