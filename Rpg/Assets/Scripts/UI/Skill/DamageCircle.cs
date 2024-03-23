using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCircle : UI_Base
{

    public override void Init()
    {
        transform.position = transform.parent.transform.position;
        StartCoroutine("ExplosionEffect");
    }
    
    [Obsolete("Obsolete")]
    IEnumerator ExplosionEffect()
    {
        yield return null;
        
        Destroy(gameObject, 0.5f);
    }
}
