using UnityEngine;

public class SecondWindItem : MonoBehaviour
{
    private void Start()
    {
        foreach (var weapon in PlayerInventory.Instance.SpawnedWeapons)
        {
            var weap = weapon.GetComponent<Weapon>();
            weap.FireRateMultiplier -= 0.1f;
        }
    }
}