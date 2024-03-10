using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public enum AnimationIndex
{
    MOVE = 0,
    ATTACK = 1,
    STUNNED = 2,
    DEATH = 3
}

public class SpritesheetAnimation : MonoBehaviour
{
    [SerializeField] float _animationSpeed = 4;
    public AnimationIndex AnimationIndex { get; private set; } = 0;
    readonly string _row = "_CurrRow", _col = "_CurrCol";

    public int CurrentFrame { get; private set; }

    public bool IsFrozen { get; set; }

    Material _mat;

    private void Awake()
    {
        _mat = GetComponentInChildren<Renderer>().material;
    }

    void Update()
    {
        CurrentFrame = IsFrozen ? 0 : (int)(Time.time * _animationSpeed);
        _mat.SetFloat(_row, (int)AnimationIndex);
        _mat.SetFloat(_col, CurrentFrame);
    }

    public void SetAnimation(AnimationIndex index)
    {
        AnimationIndex = index;
    }

    public void SetAnimation(AnimationIndex index, float animationSpeed)
    {
        AnimationIndex = index;
        _animationSpeed = animationSpeed;
    }
}
