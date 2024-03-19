using System.Collections;
using UnityEngine;

public class Santelmo : Enemy
{
    [SerializeField] float _explosionRadius;
    [SerializeField] AudioClip _chargeSFX;
    [SerializeField] AudioClip _explosionSFX;
    [SerializeField] GameObject _explosionGib;

    protected override void OnHurt(int changed, int remainingHealth)
    {
        base.OnHurt(changed, remainingHealth);

        ChangeState(EnemyState.ATTACKING);
    }

    protected override IEnumerator AttackState()
    {
        Anim.IsFrozen = true;
        Audio.PlayOneShot(_chargeSFX);
        yield return new WaitForSeconds(0.2f);
        Anim.IsFrozen = false;
        Anim.SetAnimation(AnimationIndex.ATTACK, 4);
        yield return new WaitForSeconds(0.3f);

        Instantiate(_explosionGib, transform.position, Quaternion.identity).GetComponent<GibVFX>().DoGib(_explosionSFX);

        Collider[] cols = new Collider[10];
        int count = Physics.OverlapSphereNonAlloc(transform.position, _explosionRadius, cols);

        for (int i = 0; i < count; i++)
        {
            var obj = cols[i];

            if (obj.TryGetComponent(out Rigidbody rb))
                rb.AddExplosionForce(50 * 0.1f, transform.position, _explosionRadius, 0.5f, ForceMode.Impulse);

            if (obj.TryGetComponent(out LivingBeing being) && !obj.CompareTag("Enemy"))
            {
                float relativeDamage = Mathf.Lerp(Data.Damage, 1, Vector3.Distance(transform.position, obj.transform.position) / (_explosionRadius * 3f));
                being.Damage((int)relativeDamage);
            }
        }
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
