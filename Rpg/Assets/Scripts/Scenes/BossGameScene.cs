using System.Collections;
using System.Collections.Generic;
using Protobuf.Unity;
using Unity.VisualScripting;
using UnityEngine;

public class BossGameScene : BaseScene
{
    
    void Start()
    {
        base.Init();

        SceneType = Define.Scene.BossGameScene;
        
        Managers.UI.ShowSceneUI<UI_Panel>();
        Managers.JobManager.SelectCharacter(Define.WorldObject.MyPlayer, Managers.Game.PlayerJob);
        
        gameObject.GetOrAddComponent<CursorController>();
        
        Managers.Sound.Clear();
        Managers.Sound.Play("BossSceneBGM",Define.Sound.Bgm);
        
        CMovePotal sendPkt = new CMovePotal();
        sendPkt.PreRoomId = Managers.Game.RoomId;
        
        // 일단 설정값 없어서 하드코딩함 (물론 맵은 0, 1 두개임)
        sendPkt.NextRoomId = 1;
        Managers.SocketInstance.Send(sendPkt, (ushort) MessageCode.CMovepotal.GetHashCode());
    }

    public override void Clear()
    {
        Debug.Log("BossGameScene Clear!");
    }
}
