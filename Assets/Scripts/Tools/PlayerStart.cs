using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStart : MonoBehaviour
{
    [SerializeField] GameObject _playerPrefab;
    void Start()
    {
        var player = PlayerController.Instance.transform.parent;
        if (player != null)
        {
            player.transform.position = transform.position;
            PlayerController.Instance.GetComponent<Rigidbody>().MovePosition(transform.position);
        }
        else
        {
            Instantiate(_playerPrefab, transform.position, Quaternion.identity);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 255, 0.2f);
        Gizmos.DrawCube(transform.position + Vector3.up, new Vector3(1, 2, 1));
    }

}
