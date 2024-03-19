using System;
using System.Collections;
using UnityEngine;

public delegate void HealthChangedEvent(int changed, int current);
public class LivingBeing : MonoBehaviour
{
    public int MaxHealth = 100;
    public int Health { get; protected set; }

    public event HealthChangedEvent OnHealthChanged;
    public event Action<LivingBeing> OnDie;

    private void OnEnable()
    {
        Health = MaxHealth;
    }

    public void Heal(int health)
    {
        Health += health;

        if (Health > MaxHealth)
            StartCoroutine(Overheal());

        OnHealthChanged?.Invoke(health, Health);
    }

    public void Damage(int damage)
    {
        if (Health <= 0) return;

        Health -= damage;
        OnHealthChanged(-damage, Health);
        if (Health <= 0)
            Die();
    }

    /// <summary>
    /// Default behaviour is destroy, please replace :D
    /// </summary>
    public virtual void Die()
    {
        OnDie?.Invoke(this);
        Destroy(gameObject);
    }

    // slowly return health back to max
    IEnumerator Overheal()
    {
        while (Health > MaxHealth)
        {
            Health--;
            OnHealthChanged?.Invoke(Health, Health);
            yield return new WaitForSeconds(1.5f);
        }
    }

    public virtual void Revive()
    {
        Health = MaxHealth;
        OnHealthChanged?.Invoke(Health, Health);
    }

    void Update()
    {
        if (transform.position.y <= -10) Die();
    }
}