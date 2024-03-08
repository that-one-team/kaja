using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Tag]
    [SerializeField] string _undamageable;
    [SerializeField] float _speedRequirement;
    [SerializeField] float _explosionRadius = 5f;
    [SerializeField] GameObject _hitVFX;
    int _damage = 10;
    [SerializeField] float _knockbackForce;

    Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Spawn(int damage, Vector3 shootDir)
    {
        Start();
        _damage = damage;
        _rb.useGravity = false;

        _rb.AddForce(shootDir, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        _rb.useGravity = true;

        if (other.collider.TryGetComponent(out LivingBeing being) && !other.collider.CompareTag(_undamageable))
            being.Damage(_damage);

        if (_hitVFX != null)
            Instantiate(_hitVFX, transform.position, Quaternion.identity);


        if (_knockbackForce != 0)
            DoExplosion();
    }

    void DoExplosion()
    {
        Collider[] cols = new Collider[10];
        int count = Physics.OverlapSphereNonAlloc(transform.position, _explosionRadius, cols);
        for (int i = 0; i < count; i++)
        {
            var obj = cols[i];
            print("explode");
            if (obj.TryGetComponent(out Rigidbody rb))
                rb.AddExplosionForce(_knockbackForce * 0.1f, transform.position, _explosionRadius, 0.5f, ForceMode.Impulse);

            if (obj.TryGetComponent(out LivingBeing being) && !obj.CompareTag(_undamageable))
            {
                float relativeDamage = Mathf.Lerp(_damage, 1, Vector3.Distance(transform.position, obj.transform.position) / _explosionRadius);
                being.Damage((int)relativeDamage);
            }
        }

        Destroy(gameObject);

    }
}
