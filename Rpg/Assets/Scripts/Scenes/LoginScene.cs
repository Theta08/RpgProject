using System;
using System.Collections;
using System.Collections.Generic;
using Protobuf.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Login;

        Managers.Sound.Clear();
        Managers.Sound.Play("LoginBGM_01",Define.Sound.Bgm); 
        
        // 팝업 호출
        Managers.UI.ShowSceneUI<UI_Player_Select>();
        gameObject.GetOrAddComponent<CursorController>();

    }
    
    public override void Clear()
    {
        Debug.Log("LoginScene Clear!");
    }
}