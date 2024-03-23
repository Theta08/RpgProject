using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 보스 레이저 포탈 생성
/// </summary>
public class LaserPortal : UI_Base
{
    [SerializeField]
    private int _countPortal;
    [SerializeField] 
    private float _timer;
    [SerializeField]
    private float _currentTime;
    
    Transform _goTransform;

    public int CountPortal { get { return _countPortal; } set { _countPortal = value; } }
    
    public void Update()
    {
        _currentTime += Time.deltaTime;
        
        if(_timer < _currentTime)
            Managers.Resource.Destroy(gameObject);
    }
    
    public override void Init()
    {
        // 초기화
        foreach (Transform child in transform)
            Managers.Resource.Destroy(child.gameObject);
        
        _goTransform = transform;
        
        SpawnPortals();
        _timer = 1.2f;
    }

    void SpawnPortals()
    {
        for (int i = -5; i < _countPortal; i += 3)
        {
           GameObject go = Managers.Resource.Instantiate("UI/Skill/PortalRed", _goTransform);
           
           go.transform.position = new Vector3(i, 1, 20);
           go.GetComponent<PortalRed>();
        }
        
    }
}
