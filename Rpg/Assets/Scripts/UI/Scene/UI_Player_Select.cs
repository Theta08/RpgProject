using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Player_Select : UI_Scene
{
    enum GameObjects
    {
        HorizonPanel
    }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));

        GameObject horizonPanel = Get<GameObject>((int)GameObjects.HorizonPanel);
        
        // 초기화
        foreach (Transform child in horizonPanel.transform)
            Managers.Resource.Destroy(child.gameObject);
        
        string[] jobNames = System.Enum.GetNames(typeof(Define.CharacterJob));
        
        // 캐릭터 직업 선택
        for (int i = 0; i < jobNames.Length; i++)
        {
            GameObject item = Managers.UI.MakeSubItem<UI_Character_Item>(horizonPanel.transform).gameObject;            
            UI_Character_Item character = item.GetOrAddComponent<UI_Character_Item>();
            
            // 직업 선택
            Define.CharacterJob job = (Define.CharacterJob)Enum.Parse(typeof(Define.CharacterJob), jobNames[i]);
            
            character.SetInfo(job);
        }
        
    }
}
