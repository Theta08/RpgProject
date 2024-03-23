using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Skill_Explosion2 : UI_Base
{
    private Vector3 _postion;

    public Vector3 GetPostion{ set { _postion = value; } }
    
    public override void Init()
    {
       transform.position = transform.parent.transform.position;
        
        StartCoroutine("ExplosionEffect");

    }

    IEnumerator ExplosionEffect()
    {
        yield return null;
        
        Managers.Resource.Destroy(gameObject,1.2f);
    }

}
