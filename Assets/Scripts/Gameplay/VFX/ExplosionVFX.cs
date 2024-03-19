using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ExplosionVFX : GibVFX
{
    public override void DoGib(AudioClip clip)
    {
        base.DoGib(clip);
        transform.localScale = Vector3.zero;
        transform.DOPunchScale(Vector3.one * 3f, 0.5f, 0, 1);
    }
}
