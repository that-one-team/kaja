using DG.Tweening;
using UnityEngine;

public class Weapon : Item
{
    [SerializeField] float _equipSpeed = 0.2f;
    bool _isReady = false;

    public void Equip()
    {
        transform.localPosition = Vector3.down;
        gameObject.SetActive(true);
        transform.DOLocalMoveY(0, _equipSpeed).OnComplete(() => _isReady = true);
    }

    public void Unequip()
    {
        transform.DOLocalMoveY(Vector3.down.y, _equipSpeed).OnComplete(() =>
        {
            gameObject.SetActive(false);
            _isReady = false;
        });
    }

    public void Shoot()
    {
        if (!_isReady) return;

        print("Shooting");
    }
}