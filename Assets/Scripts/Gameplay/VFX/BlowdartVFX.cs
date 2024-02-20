using System.Collections;
using System.Collections.Generic;
using System.Net;
using DG.Tweening;
using UnityEngine;

public class BlowdartVFX : ItemVFX
{
    LineRenderer _lineRend;

    public override void DoVFX(ItemData data, params dynamic[] args)
    {
        _lineRend = GetComponent<LineRenderer>();
        var startPos = (Vector3)args[0];
        var endpoint = (Vector3)args[1];
        _lineRend.SetPositions(new Vector3[] { startPos, endpoint });
        var startColor = new Color2(_lineRend.startColor, _lineRend.endColor);
        var endColor = new Color2(new Color(1, 1, 1, 0), new Color(1, 1, 1, 0));

        _lineRend.DOColor(startColor, endColor, 5f).OnComplete(() => Destroy(gameObject));
    }

}
