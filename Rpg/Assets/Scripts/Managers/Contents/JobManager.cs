using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JobManager
{
    public string JobSkillIcon(Define.CharacterJob job)
    {
        string result = "";
        
        switch (Managers.Game.PlayerJob)
        {
            case Define.CharacterJob.Sword:
                result = "SordSkill_1";
                break;
        
            case Define.CharacterJob.Archer:
                result = "ArcherSkill_1";
                break;
        }
        
        return result;
    }
    
    public string IntToStringPlayerJob(int job)
    {
    string result = "";
    
        switch (job)
        {
            case 0:
                result = Define.CharacterJob.Sword.ToString();
                break;
            case 1:
                result = Define.CharacterJob.Archer.ToString();
                break;
        }
        
        return result;
    }
    
    public string IntToStringMonsterJob(int job)
    {
        string result = "";
        
        switch (job)
        {
            case 0:
                result = Define.MonsterJob.Knight.ToString();
                break;
            case 1:
                result = Define.MonsterJob.UD_Spearman.ToString();
                break;
            case 2:
                // 일단 보스 
                result = Define.MonsterJob.UD_Boss.ToString();
                break;
        }
            
        return result;
    }
    
    public void PlayerJobSelect(GameObject go, Define.CharacterJob job)
    {
        Animator animator = go.GetComponent<Animator>();
        PlayerStat stat = go.GetComponent<PlayerStat>();
        
        stat.Job = (int) job;
        switch (job)
        {
            case Define.CharacterJob.Archer:
                animator.runtimeAnimatorController =
                    Managers.Resource.Load<RuntimeAnimatorController>("Art/PlayerArcherContorller");
                stat.AttackRange = 5.0f;
                break;
            case Define.CharacterJob.Sword:
                animator.runtimeAnimatorController =
                    Managers.Resource.Load<RuntimeAnimatorController>("Art/PlayerContorller");
                stat.AttackRange = 2.0f;
                break;
        }
    }
    
    /// <summary>
    /// 스폰할 캐릭터 타임, 직업  (카메라까지 있음)
    /// </summary>
    /// <param name="type">내 캐릭이면 MyPlayer </param>
    /// <param name="playerJob"></param>
    public void SelectCharacter(Define.WorldObject type, Define.CharacterJob playerJob)
    {
        Managers.Game.PlayerJob = playerJob;
        GameObject go = GameObject.Find("PlayerSpawn");
        GameObject player = Managers.Game.Spawn(type, playerJob.ToString());
        
        player.transform.position = go.transform.position;
        
        Camera.main.gameObject.GetOrAddComponent<CameraController>().SetPlayer(player);
    }
}