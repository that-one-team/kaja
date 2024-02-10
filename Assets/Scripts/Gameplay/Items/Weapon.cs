using DG.Tweening;
using UnityEngine;

public class Weapon : Item
{
    bool _isReady = false;

    public void Equip()
    {
        transform.localPosition = Vector3.down;
        transform.DOLocalMoveY(0, 0.5f).OnComplete(() => _isReady = true);
    }

    public void Unequip()
    {
        transform.DOLocalMoveY(Vector3.down.y, 0.5f).OnComplete(() => _isReady = false);
    }

    public void Shoot()
    {
        if (!_isReady) return;

        print("Shooting");
    }
}