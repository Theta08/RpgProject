using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlaneRaycast : MonoBehaviour
{
    [SerializeField]
    int _mask = (1 << (int) Define.Layer.Player);
    [SerializeField]
    private Vector3 boxSize; // 스킬 범위
    [SerializeField]
    private int _damage = 80;
    
    private void Start()
    {
        boxSize = gameObject.GetComponent<BoxCollider>().size;
        boxSize = Vector3.Scale(boxSize, transform.localScale);
    }
    
    /// <summary>
    /// 플레이어 지정 및 스킬 시전
    /// </summary>
    public void CheackRanage()
    {
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, boxSize / 2, Vector3.down,Quaternion.identity,Mathf.Infinity, _mask);
        
        foreach (RaycastHit hit in hits)
        {
            Debug.Log("Object above: " + hit.collider.gameObject.name);
            GameObject _grenade = Managers.UI.MakeSkill<Boss_Skill1>(transform).gameObject;
            _grenade.GetOrAddComponent<Boss_Skill1>();
            _grenade.GetComponent<Boss_Skill1>().GetTransform = hit.transform;
            _grenade.transform.position += Vector3.up;
            
            hit.transform.GetComponent<PlayerController>().OnDamageEvent(_damage);
        }
    }
    
    // 박스의 크기를 시각적으로 나타내기 위해 OnDrawGizmos 함수를 사용합니다.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
    
}
