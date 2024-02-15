using UnityEngine;

public class Player : LivingBeing
{
    public static Player Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] PlayerHUD _hud;

    public override void Die()
    {
        GameManager.Instance.Freeze(true);
    }
}