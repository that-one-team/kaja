using UnityEngine;

public class Sway : MonoBehaviour
{
    [SerializeField] private float _smoothing;
    [SerializeField] private float _swayMultiplier;

    GameplayActions.PlayerActions _actions;

    private void Start()
    {
        _actions = PlayerInputs.Instance.Actions;
    }

    void Update()
    {
        Vector2 input = new(_actions.Look.ReadValue<Vector2>().x * _swayMultiplier, _actions.Look.ReadValue<Vector2>().y * _swayMultiplier);
        var rotX = Quaternion.AngleAxis(-input.y, Vector3.right);
        var rotY = Quaternion.AngleAxis(input.x, Vector3.up);

        var targetRot = rotX * rotY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, _smoothing * Time.deltaTime);
    }
}