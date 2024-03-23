using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Skill_Slash : UI_Base
{
    enum GameObjects
    {
        Sparks,
        Flash,
        StonesHit,
        Stones,
    }

    private Rigidbody _rigidbody;
    
    // [SerializeField]
    // private int _damage = 80;
    [SerializeField]
    private GameObject _meshObj;
    [SerializeField]
    private GameObject _effectObj;

    private Transform _postion;
    public Quaternion pos;
    
    public Transform GetTransform { get { return _postion;} set { _postion = value; } }
    
    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));

        Debug.Log($"{pos}");
        
        transform.position = transform.parent.position + new Vector3(0, 2, 0);
        // transform.rotation = transform.Translate(pos);
        // _meshObj = GetObject((int)GameObjects.MeshObject);
        // _effectObj = GetObject((int)GameObjects.Explosion);
        
        StartCoroutine("ExplosionEffect");
    }

    
    private void OnEnable()
    {
        // Debug.Log($"onEnable 실행");
        // Debug.Log($"{transform.parent.transform.position}");
        // Debug.Log($"{transform.parent.position}");
        //
        // // gameObject.SetActive(true);
        // //
        // if (transform.parent != null)
        // {
        //     Vector3 parentPos = transform.parent.position;
        //     transform.position = parentPos + Vector3.up;
        // }
    }

    IEnumerator ExplosionEffect()
    {
        // yield return new WaitForSeconds(0.3f);
        yield return null;
     
        // gameObject.SetActive(false);
        
        // _meshObj.SetActive(false);
        // _effectObj.SetActive(true);
        
        Vector3 pVec = transform.parent.transform.position;
        // RaycastHit[] rayHits = Physics.SphereCastAll(pVec, 3, 
        //     Vector3.up,
        //     0f, LayerMask.GetMask("Monster"));
        //
        // foreach (RaycastHit hitObj in rayHits)
        // {
        //     hitObj.transform.GetComponent<MonsterController>().OnSkillDamageEvnet(_damage, hitObj.transform.position);
        // }
        
        // Managers.Resource.Destroy(gameObject, 2.0f);
        Destroy(gameObject, 1.0f);
    }

}
