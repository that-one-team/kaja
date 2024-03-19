using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KilLVolume : Volume
{
    public override void OnEnter()
    {
        Player.Instance.Damage(1000);
    }
}
