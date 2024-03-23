using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class PortalRed : UI_Base
{
    
    [SerializeField] 
    private float _timer;
    [SerializeField]
    private float _currentTime;
    
    void Start()
    {
        Init();
    }

    public override void Init()
    { 
        // 초기화
        foreach (Transform child in transform)
            Managers.Resource.Destroy(child.gameObject);
        
        GameObject go = Managers.Resource.Instantiate("AtackItem/Arrow_Laser", transform);
        go.transform.position = Vector3.zero;
        go.transform.position += new Vector3(transform.position.x,1, 0);
        
        
        // go.transform.position = transform.position;
        // go.transform.position += new Vector3(0,0, -20);
        
        Arrow_Laser arrow = go.GetOrAddComponent<Arrow_Laser>();
        arrow.Damage = 30;

        _timer = 1.2f;
    }

    public void Update()
    {
        _currentTime += Time.deltaTime;
        
        if(_timer < _currentTime)
            Managers.Resource.Destroy(gameObject);
    }


}
