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

        var player = PlayerController.Instance.transform.parent;
        var rb = PlayerController.Instance.GetComponent<Rigidbody>();
        rb.MovePosition(transform.position);
        rb.MoveRotation(transform.rotation);

        // TODO fix player start rotation shit
        var angle = -Quaternion.Angle(transform.rotation, player.rotation);
        player.Rotate(Vector3.up, angle);
        PlayerController.Instance.GetComponent<PlayerLook>().AngleOffset = -angle;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 255, 0.2f);
        Gizmos.DrawCube(transform.position + Vector3.up, new Vector3(1, 2, 1));
    }

}
