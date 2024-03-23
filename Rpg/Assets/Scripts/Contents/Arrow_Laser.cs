using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Arrow_Laser : Arrow
{

    public override void Init()
    {
        _penetrate = true;
    }

    protected override void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            other.gameObject.GetComponent<PlayerController>().OnDamageEvent(_damage);

    }
}
