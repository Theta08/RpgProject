using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : BaseController
{
    public override void Init()
    {
        base.Init();
        State = Define.State.Idle;
        WorldObjectType = Define.WorldObject.Monster;
        
        if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);
    }

    protected override void UpdateSkill()
    {
        base.UpdateSkill();
        // if (_lockTarget != null)
        // {
        //     Vector3 dir = _lockTarget.transform.position - transform.position;
        //     Quaternion quat = Quaternion.LookRotation(dir);
        //     transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        // }
    }

    protected override void UpdateDamage()
    {
        // Debug.Log("HITTED !!!");
    }

    protected override void UpdateDie()
    {
        // Managers.Game.Despawn(gameObject);
        Debug.Log("DIE !!!");
        _stat.OnDead();
        Managers.UnitManger.RemoveMonster(_stat.Code);
        Destroy(this.GameObject());
    }

    public void OnDamage(int damage)
    {
        _stat.OnAttacked(damage);
    }
    
    public void OnSkillDamageEvnet(int damage, Vector3 explosionPos)
    {
        _stat.OnAttacked(damage);
    }

    void OnHitEvent()
    {
    }
}