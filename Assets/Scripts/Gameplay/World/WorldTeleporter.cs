using UnityEngine;

public class WorldTeleporter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WorldManager.Instance.NextWorld();
        }
    }
}
