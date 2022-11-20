using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GunPickup", menuName = "Drops/Gun Pickup")]
public class GunType : ScriptableObject
{
    public float Power = 1;
    public float Speed = 60f;
    public float Size = 0.3f;
    public int Count = 1;
    public float Delay = 0.3f;
    public float Spread = 0.001f;
    public Color Colour;
}
