using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow_Regular : Arrow
{
    RaycastHit hit;
    
    public override void Init()
    {
        _penetrate = true;
        _rigidbody.velocity = transform.forward  * Time.time * 1.0f;
    }
    void DestrpyObject()
    {
        if (Physics.Raycast(transform.position, Vector3.down * 1f, out hit))
        {
            if (hit.transform.gameObject.layer != 9)
                Managers.Resource.Destroy(gameObject);                
        }
    }
}
