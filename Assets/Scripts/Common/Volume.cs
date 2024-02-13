using UnityEngine;

public abstract class Volume : MonoBehaviour
{
    public abstract void OnEnter();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            OnEnter();
    }
}