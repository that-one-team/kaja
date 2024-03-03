using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DoorInteractable : Interactable
{
    [Header("Door settings")]
    [SerializeField] bool _canOnlyOpenOnce = true;
    [SerializeField] float _targetY = 90;
    [SerializeField] float _openSpeed = 0.5f;
    [SerializeField] AudioClip _doorSound;

    public bool IsLocked = false;

    bool _isOpened = false;
    float _y;

    AudioSource _source;

    private void Start()
    {
        _y = transform.localEulerAngles.y;
        _source = GetComponent<AudioSource>();
    }

    private void OnValidate()
    {
        InteractionType = InteractionType.LOOK_AT;
        _source = GetComponent<AudioSource>();
        _source.spatialBlend = 1;
    }

    public override void Interact()
    {
        if (IsLocked) return;
        if (_canOnlyOpenOnce && _isOpened) return;

        var sign = Mathf.Sign(Vector3.Dot(transform.forward, Camera.main.transform.forward));
        var targ = _isOpened ? _y : _targetY * sign;
        transform.DOLocalRotate(Vector3.up * targ, _openSpeed).SetEase(Ease.OutBack);
        _isOpened = !_isOpened;
        _source.PlayOneShot(_doorSound);
    }

    private void OnCollisionEnter(Collision other)
    {
        var col = other.collider;
        if (col.CompareTag("Player") && col.GetComponent<PlayerController>().IsSliding)
        {
            Interact();
        }
    }
}
