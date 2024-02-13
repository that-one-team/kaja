using System;
using System.Collections;
using UnityEngine;

public class LivingBeing : MonoBehaviour
{
    public event Action<LivingBeing> OnDie;

    public int MaxHealth = 100;
    public int Health { get; protected set; }

    private void Start()
    {
        Health = MaxHealth;
    }

    public void Heal(int health)
    {
        Health += health;

        if (Health > MaxHealth)
            StartCoroutine(Overheal());
    }

    public void Damage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
            Die();
    }

    /// <summary>
    /// Default behaviour is destroy, please replace :D
    /// </summary>
    public virtual void Die()
    {
        Destroy(gameObject);
        OnDie?.Invoke(this);
    }

    // slowly return health back to max
    IEnumerator Overheal()
    {
        while (Health > MaxHealth)
        {
            Health--;
            yield return new WaitForSeconds(1.5f);
        }
    }
}