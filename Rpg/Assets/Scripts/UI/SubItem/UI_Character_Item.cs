using System;
using System.Collections;
using System.Collections.Generic;
using Protobuf.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Character_Item : UI_Base
{
    enum GameObjects
    {
        CharacterIcon,
        CharacterName,
    }

    Define.CharacterJob _name;

    public override void Init()
    {
        Managers.UI.ShowPopupUI<UI_Player_Name>();
        GameObject nameUISelect = GameObject.Find("UI_Player_Name");
        
        nameUISelect.SetActive(false);
        
        Bind<GameObject>(typeof(GameObjects));
        
        Get<GameObject>((int)GameObjects.CharacterName).GetComponent<TextMeshProUGUI>().text = _name.ToString();
        Get<GameObject>((int)GameObjects.CharacterIcon).BindEvent((PointerEventData) =>
        {
            if (nameUISelect != null)
                nameUISelect.SetActive(true);

            Managers.Game.PlayerJob =  _name;
        });
    }

    public void SetInfo(Define.CharacterJob name)
    {
        _name = name;
    }
}
