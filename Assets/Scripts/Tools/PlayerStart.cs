using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStart : MonoBehaviour
{
    [SerializeField] GameObject _playerPrefab;

    public void TeleportPlayer()
    {
        if (PlayerController.Instance == null)
            Instantiate(_playerPrefab, transform.position, Quaternion.identity);

        var rb = PlayerController.Instance.GetComponent<Rigidbody>();
        rb.MovePosition(transform.position);
        rb.MoveRotation(transform.rotation);
        var player = PlayerController.Instance.transform;
        player.rotation = transform.rotation; // stupid ass code fuck you unity not working
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 255, 0.2f);
        Gizmos.DrawCube(transform.position + Vector3.up, new Vector3(1, 2, 1));
    }

}
