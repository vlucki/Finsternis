
using System.Collections;
using UnityEngine;

public class PopupAttribute : PropertyAttribute
{
    public enum PopUpType
    {
        INT = 0,
        FLOAT = 1,
        STRING = 2
    }

    public readonly PopUpType popupType;

    public readonly ArrayList _items;

    public ArrayList Items { get { return _items; } }

    public PopupAttribute(params string[] items)
    {
        popupType = PopUpType.STRING;
        _items = MakeItems(items);
    }

    public PopupAttribute(params int[] items)
    {
        popupType = PopUpType.INT;
        _items = MakeItems(items);
    }

    public PopupAttribute(params float[] items)
    {
        popupType = PopUpType.FLOAT;
        _items = MakeItems(items);
    }

    private ArrayList MakeItems<T>(T[] items)
    {
        if (items == null || items.Length < 1)
            throw new System.ArgumentException("Must pass at least one item to popup!");
        return ArrayList.ReadOnly(new ArrayList(items));
    }
}