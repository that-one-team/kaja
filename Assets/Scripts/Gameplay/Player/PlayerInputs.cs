using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    public static PlayerInputs Instance { get; private set; }
    GameplayActions _actions;

    public GameplayActions.PlayerActions Actions { get; private set; }
    public GameplayActions.UIActions UI { get; private set; }
    public GameplayActions.ItemsActions Items { get; private set; }

    void Awake()
    {
        Instance = this;

        _actions = new();
        _actions.Enable();
        Actions = _actions.Player;
        UI = _actions.UI;
        Items = _actions.Items;
    }
}