using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Skill_Laser : UI_Base
{
    enum GameObjects
    {
        Line,
    }
    
    
    [SerializeField]
    private GameObject _effectObj;
    
    
    private int _mask = (1 << (int) Define.Layer.Monster);
    private Vector3 _postion;
    
    public Vector3 GetPostion{ set { _postion = value; } }
    // private Transform _postion;
    // public Transform GetTransform { set { _postion = value; } }

    public Vector3 boxSize = new Vector3(1, 1, 3); // 박스의 크기
    public float maxDistance = 3f; // 박스캐스트의 최대 거리
    
    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        
        transform.position = transform.parent.position + Vector3.up;
        
        Vector3 currentAngle = transform.rotation.eulerAngles;
        currentAngle.y += transform.parent.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(currentAngle);
    }
    
    void Update()
    {
        BoxCastAll();
    }
    

    
    
    void OnDrawGizmosSelected()
    {
        // 스킬 범위를 시각적으로 표시
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + transform.forward * (boxSize.z / 2), boxSize);
    }

    void BoxCastAll()
    {
        // 마우스 클릭 지점에서 박스캐스트를 수행합니다.
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, boxSize / 2, transform.forward, Quaternion.identity, maxDistance, _mask);

        // 박스캐스트가 충돌한 모든 오브젝트에 대해 처리
        foreach (RaycastHit hit in hits)
        {
            // 여기서는 감지된 오브젝트에 대한 처리를 수행할 수 있습니다.
            Debug.Log("Skill hit object: " + hit.collider.gameObject.name);
        }
        
        // Debug.DrawRay(transform.position, transform.forward * maxDistance, Color.red);
    }
    
}
