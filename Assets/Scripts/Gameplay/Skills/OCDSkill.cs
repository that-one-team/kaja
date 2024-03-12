using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OCDSkill : Skill
{
    float _timer;
    PlayerInventory _inventory;
    Transform _player;

    GameObject _target;

    private void Start()
    {
        _inventory = PlayerInventory.Instance;
        _player = PlayerController.Instance.transform;
    }

    protected override void OnSkillStart()
    {
        _timer = Data.Duration;
        base.OnSkillStart();
    }

    public override void OnSkillUpdate()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length == 0) return;

            foreach (var enemy in enemies)
            {
                var dir = Vector3.Normalize(enemy.transform.position - _player.position);
                var dot = Vector3.Dot(Camera.main.transform.forward, dir);

                if (dot > 0.8f)
                {
                    _target = enemy;
                    _inventory.CurrentWeapon.Target = _target;
                }
            }
        }
        else if (IsSkillActive)
        {
            _inventory.ClearWeaponTargets();
            OnSkillEnd();
        }
    }
}
