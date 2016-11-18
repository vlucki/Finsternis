using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Text))]
public class TextSetter : MonoBehaviour
{
    [SerializeField]
    private int decimalPlaces = 2;

    private Text text;

    private string decimalFormat;

    void Awake()
    {
        this.text = GetComponent<Text>();
        UpdateDecimalFormat();
    }

    public void SetDecimalPlaces(int decimalPlaces)
    {
        this.decimalPlaces = decimalPlaces;
        UpdateDecimalFormat();
    }

    private void UpdateDecimalFormat()
    {
        this.decimalFormat = "F";
        if (this.decimalPlaces == 0)
            return;

        this.decimalFormat += this.decimalPlaces.ToString();
    }

    public void SetText(int i)
    {
        SetText(i.ToString());
    }

    public void SetText(float f)
    {
        SetText(f.ToString(this.decimalFormat));
    }

    public void SetText(Vector2 v)
    {
        SetText(v.ToString(this.decimalFormat));
    }

    public void SetText(Vector3 v)
    {
        SetText(v.ToString(this.decimalFormat));
    }

    public void SetText(Vector4 v)
    {
        SetText(v.ToString(this.decimalFormat));
    }

    public void SetText(string s)
    {
        this.text.text = s;
    }
}
