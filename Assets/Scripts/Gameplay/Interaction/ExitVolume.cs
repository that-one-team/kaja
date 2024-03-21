using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitVolume : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player.Instance.Die();
            StartCoroutine(Exit());
        }
    }

    IEnumerator Exit()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("SCN_MainMenu");
    }
}
