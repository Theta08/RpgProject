using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Panel : UI_Scene
{
    enum GameObjects
    {
        BottomPanel,
        TopPanel,
        SkillPanel,
        ChattingPanel,
    }

    enum Texts
    {
        NameText
    }

    public override void Init()
    {
        base.Init();

        #region Bind
        Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(Texts));

        Get<GameObject>((int) GameObjects.BottomPanel);
        Get<GameObject>((int) GameObjects.TopPanel);
        GetText((int)Texts.NameText).text = $"{Managers.Game.PlayerName}";
        #endregion

        #region GetObject
        
        GameObject bottomPanel = Get<GameObject>((int)GameObjects.BottomPanel);
        GameObject skillPanel = Get<GameObject>((int)GameObjects.SkillPanel);
        GameObject chattingPanel = Get<GameObject>((int)GameObjects.ChattingPanel);
        GameObject topPanel = Get<GameObject>((int)GameObjects.TopPanel);
        
        GameObject item = Managers.UI.MakeSubItem<Bottom_HPBar>(bottomPanel.transform).gameObject;
        GameObject item2 = Managers.UI.MakeSubItem<UI_Chatting>(chattingPanel.transform).gameObject;
        
        #endregion
        
        item.GetOrAddComponent<Bottom_HPBar>();
        item2.GetOrAddComponent<UI_Chatting>();
        
        if (Managers.Scene.CurrentScene.SceneType == Define.Scene.BossGameScene)
        {
            GameObject item3 = Managers.UI.MakeSubItem<Top_HPBar>(topPanel.transform).gameObject;
            item3.GetOrAddComponent<Top_HPBar>();
            
            Debug.Log($"y: {item3.GetComponent<RectTransform>().anchoredPosition.y}");
            Debug.Log($"y: {item3.GetComponent<RectTransform>().anchoredPosition.x}");
        }
        else
        {
            GameObject questItem = Managers.UI.MakeQuest<UI_Quest>(topPanel.transform).gameObject;
            
            questItem.GetOrAddComponent<UI_Quest>().CountNumber = 0;
            questItem.GetOrAddComponent<UI_Quest>().MaxCountNumber = 5;
        }

        // 스킬 수
        for (int i = 0; i < 2; i++)
        {
            GameObject skillItem = Managers.UI.MakeSubItem<UI_Skill_Item>(skillPanel.transform).gameObject;
            UI_Skill_Item skill = skillItem.GetComponent<UI_Skill_Item>();
            skill.IconImgUrl = "HealSkill";
            switch (i)
            {
                case 0:
                    skill.CoolTime = 3.0f;
                    skill.TextName = "Q";
                    skill.IconImgUrl = Managers.JobManager.JobSkillIcon(Managers.Game.PlayerJob);
                    break;
                case 1:
                    skill.CoolTime = 5.0f;
                    skill.TextName = "W";
                    break;
            }
        }
        
        Canvas canvs = gameObject.GetOrAddComponent<Canvas>();
        
        canvs.overrideSorting = true;
        canvs.sortingOrder = 5;
        
    }
    
}
