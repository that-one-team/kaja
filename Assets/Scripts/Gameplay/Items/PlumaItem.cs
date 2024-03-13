using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlumaItem : MonoBehaviour
{
    int _plumaLeft = 4;
    [SerializeField] GameObject _vfx;
    [SerializeField] AudioClip _sfx;

    AudioSource _audio;
    private void Start()
    {
        _audio = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        transform.localScale = Vector3.one * 2;
        transform.DOScale(1, 0.2f);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && _plumaLeft > 0)
        {
            var seq = DOTween.Sequence();
            var pluma = transform.GetChild(_plumaLeft - 1);
            seq.Join(pluma.DOLocalMove(Vector3.zero, 0.05f));
            seq.Join(pluma.DOScale(0, 0.05f));
            seq.Play().OnComplete(() => pluma.gameObject.SetActive(false));
            _audio.PlayOneShot(_sfx, 0.5f);

            var point = Camera.main.transform.position + Camera.main.transform.forward * 10000;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 10000, ~LayerMask.GetMask("Ignore Raycast", "Hand")))
            {
                point = hit.point;
                Instantiate(_vfx, transform.position, Quaternion.identity).GetComponent<ItemVFX>().DoVFX(null, Camera.main.transform.position + Camera.main.transform.forward, point);

                if (hit.collider.TryGetComponent(out LivingBeing being))
                {
                    being.Damage(100);
                }

            }

            _plumaLeft--;

            if (_plumaLeft <= 0)
                PlayerInventory.Instance.RemoveItem("Rizal's Feathered Pluma");
        }
    }
}
