using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;
        
        Managers.UI.ShowSceneUI<UI_Panel>();

        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        
        Managers.JobManager.SelectCharacter(Define.WorldObject.MyPlayer, Managers.Game.PlayerJob);
        
        gameObject.GetOrAddComponent<CursorController>();
        
        Managers.Sound.Clear();
        Managers.Sound.Play("GameSceneBGM",Define.Sound.Bgm); 
        
        GameObject go = new GameObject { name = "SpawningPool" };
        SpawningPool pool = go.GetOrAddComponent<SpawningPool>();
        
    }
    

    public override void Clear()
    {
        
    }
    
}
