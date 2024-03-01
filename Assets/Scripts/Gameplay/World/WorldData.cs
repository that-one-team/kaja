using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World Data", menuName = "proj/World Data", order = 0)]
public class WorldData : ScriptableObject
{
    public new string name;
    public List<Room> Rooms;
    public Vector2 RoomsSpawnRange;
}