
using UnityEngine;

[System.Serializable]
public struct Range<T>
{
    public T Min;
    public T Max;

    public Range(T min, T max)
    {
        Min = min;
        Max = max;
    }
}

public static class Misc
{
    public static void SnapToGround(GameObject obj)
    {
        if (Physics.Raycast(obj.transform.position, Vector3.down, out RaycastHit hit, 100))
            obj.transform.position = hit.point;
    }
}