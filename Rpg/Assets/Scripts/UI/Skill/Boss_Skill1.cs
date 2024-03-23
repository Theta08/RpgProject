using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss_Skill1 : UI_Base
{
    enum GameObjects
    {
        Boss_Skill1,
    }

    private Rigidbody _rigidbody;
    
    [SerializeField]
    private GameObject _effectObj;

    private Transform _postion;

    public Transform GetTransform { set { _postion = value; } }
    
    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));

        gameObject.transform.position = _postion.position;
        
        _effectObj = GetObject((int)GameObjects.Boss_Skill1);
        StartCoroutine("ExplosionEffect");
    }

    
    IEnumerator ExplosionEffect()
    {
        yield return new WaitForSeconds(0.5f);
        
        _effectObj.SetActive(true);
        Managers.Resource.Destroy(gameObject,1.2f);

    }

}
