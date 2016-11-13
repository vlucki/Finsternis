using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Renderer))]
public class FlashMaterialColor : MonoBehaviour
{

    [SerializeField]
    private Color flashColor = Color.red;

    [SerializeField][Range(1, 20)]
    private int numberOfFlashes = 10;

    [SerializeField][Range(.01f, 1f)]
    private float flashFadeInTime = .1f;

    [SerializeField]
    [Range(.01f, 1f)]
    private float flashFadeOutTime = .15f;

    private Material[] materials;
    private Color[] originalColors;

    void Awake()
    {
        var renderer = GetComponent<Renderer>();
        materials = renderer.materials;
        originalColors = new Color[materials.Length];
        for(int i = 0; i < originalColors.Length; i++)
        {
            originalColors[i] = materials[i].GetColor("_EmissionColor");
        }
    }

    public void DoFlash()
    {
        StopAllCoroutines();
        SetColors(this.originalColors);
        StartCoroutine(_Flash());
    }

    private IEnumerator _Flash()
    {
        for(int i = 0; i < this.numberOfFlashes; i++)
        {
            yield return StartCoroutine(_UpdateColor(this.flashFadeInTime, this.flashColor));
            yield return StartCoroutine(_UpdateColor(this.flashFadeOutTime, this.originalColors));
        }
    }

    private IEnumerator _UpdateColor(float duration, params Color[] targetColors)
    {
        float elapsedTime = 0;
        while(elapsedTime <= this.flashFadeInTime)
        {
            elapsedTime += Time.deltaTime;
            for(int i = 0; i < this.materials.Length; i++)
            {
                LerpColor(this.materials[i], targetColors.Length > 1 ? targetColors[i] : targetColors[0], elapsedTime / duration);
            }
            yield return null;
        }
        SetColors(targetColors);
    }

    private void SetColors(params Color[] targetColors)
    {
        for (int i = 0; i < this.materials.Length; i++)
        {
            this.materials[i].SetColor("_EmissionColor", targetColors.Length > 1 ? targetColors[i] : targetColors[0]);
        }
    }

    private void LerpColor(Material material, Color targetColor, float t)
    {
        material.SetColor("_EmissionColor", Color.Lerp(material.color, targetColor, t));
    }
}
