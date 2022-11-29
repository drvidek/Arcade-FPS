using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogDriver : MonoBehaviour
{
    [SerializeField] private bool disabled;
    [SerializeField] private Camera camera;
    [SerializeField] private Material material;
    [SerializeField] private float fogStart = 5f, fogEnd = 25f;
    [SerializeField] private Color fogColor = new Color(1, 1, 1);

    private void Awake()
    {
        camera.depthTextureMode = DepthTextureMode.Depth;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (disabled)
        {
            Graphics.Blit(source,destination);
            return;
        }
       //material.SetFloat("_FogStart", fogStart);
       //material.SetFloat("_FogEnd", fogEnd);
       //material.SetColor("_FogColor", fogColor);
        Graphics.Blit(source, destination, material);
    }
}
