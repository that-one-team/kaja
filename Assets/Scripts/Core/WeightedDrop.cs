
using UnityEngine;

[System.Serializable]
public class WeightedDrop
{
    public GameObject Item;

    [Range(0, 100)]
    public float Chance;
}