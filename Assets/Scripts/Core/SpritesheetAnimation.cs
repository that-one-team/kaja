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

    Material _mat;

    private void Start()
    {
        _mat = GetComponentInChildren<Renderer>().material;
    }

    void Update()
    {
        int frame = (int)(Time.time * _animationSpeed);
        _mat.SetFloat(_row, (int)AnimationIndex);
        _mat.SetFloat(_col, frame);
    }

    public void SetAnimation(AnimationIndex index)
    {
        print("[ANIM]: set animation to " + index);
        AnimationIndex = index;
    }
}
