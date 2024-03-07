using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class LivingBeing : MonoBehaviour
{
    public event Action<LivingBeing> OnDie;

    public int MaxHealth = 100;
    public int Health { get; protected set; }

    public event Action<int> OnHurt;

    private void OnEnable()
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
        if (Health <= 0) return;

        Health -= damage;
        OnHurt?.Invoke(Health);
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
            yield return new WaitForSeconds(1.5f);
        }
    }

    void Update()
    {
        if (transform.position.y <= -10) Die();
    }
}