using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DoorInteractable : Interactable
{
    [SerializeField] bool _canOnlyOpenOnce = true;
    [SerializeField] float _targetY = 90;
    [SerializeField] float _openSpeed = 0.5f;
    [SerializeField] AudioClip _doorSound;

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
        InteractionType = InteractionType.BUTTON;
        _source = GetComponent<AudioSource>();
        _source.spatialBlend = 1;
    }

    public override void Interact()
    {
        if (_canOnlyOpenOnce && _isOpened) return;

        var targ = _isOpened ? _y : _targetY;
        transform.DOLocalRotate(Vector3.up * targ, _openSpeed).SetEase(Ease.OutBack);
        _isOpened = !_isOpened;
        _source.PlayOneShot(_doorSound);
    }
}
