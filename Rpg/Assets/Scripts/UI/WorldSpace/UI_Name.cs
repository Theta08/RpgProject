using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Name : UI_Base
{
    enum GameObjects
    {
        NameText
    }

    Stat _stat;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(GameObjects));
        
        _stat = transform.parent.GetComponent<Stat>();
        
        if(_stat.ObjectName.Length > 0)
            GetText((int)GameObjects.NameText).text = $"{_stat.ObjectName}";
        else
            GetText((int)GameObjects.NameText).text = $"{Managers.Game.PlayerName}"; 
        
    }

    private void Update()
    {
        Transform parent = transform.parent;
        transform.position = parent.position + Vector3.up * 1.25f * (parent.GetComponent<Collider>().bounds.size.y);
        transform.rotation = Camera.main.transform.rotation;
        
    }

    public void SetName(string name)
    {
        if(GetText((int)GameObjects.NameText) != null)
            GetText((int)GameObjects.NameText).text = $"{name}";
    }
    
}
