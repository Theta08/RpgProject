using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BossMonsterController : BaseController
{
    [SerializeField] private GameObject _planeObject; // _onSkill쓰기위한 오브젝트 호출
    public override void Init()
    {
        base.Init();

        State = Define.State.Idle;
        WorldObjectType = Define.WorldObject.Monster;

        _stat = gameObject.GetComponent<BossMonsterStat>();
        _stat.Hp = _stat.MaxHp;

        Managers.Game.BossMonsterInfo = gameObject.GetComponent<BossMonsterController>();

        _planeObject = GameObject.Find("Plane");
    }

    #region 테스트용 삭제해야함

    protected override void UpdateIdle()
    {
        GameObject player = Managers.Game.GetPlayer();

        if (player == null)
            return;

        float distance = (player.transform.position - transform.position).magnitude;

        // if (distance <= _scanRange)
        // {
        //     // Debug.Log($"d__ : {distance}");
        //     _lockTarget = player;
        //     State = Define.State.Moving;
        //     return;
        // }
    }

    protected override void UpdateMoving()
    {
        // 플레이어가 내 사정거리보다 가까우면 공격
        if (_lockTarget != null)
        {
            _destPos = _lockTarget.transform.position;
            float distance = (_destPos - transform.transform.position).magnitude;

            /*
            if (distance <= _attackrange)
            {
                // 밀려나는거
                NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
                nma.SetDestination(transform.position);

                State = Define.State.AttackWait;
                return;
            }
             */
        }
    }

    #endregion

    protected override void UpdateSkill()
    {
        base.UpdateSkill();
    }

    protected override void UpdateDamage()
    {
    }

    protected override void UpdateDie()
    {
        _stat.OnDead();
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

    /// <summary>
    /// 공격에니메이션 전 범위 설정
    /// </summary>
    void OnDamageRange()
    {
        Managers.UI.MakeSkill<DamageCircle>(transform);
    }

    void OnAttackWait()
    {
        State = Define.State.Skill;
    }


    protected override void UpdateFuniton()  { }


    protected override void UpdateAttackWait()
    {
        // Managers.UI.MakeSkill<DamageCircle>(transform);
    }

    protected override void UpdateSkill1()
    {
        _planeObject.GetComponent<PlaneRaycast>().CheackRanage();
        State = Define.State.Idle;
    }

    protected override void UpdateSkill2()
    {
        Managers.UI.MakeSkill<LaserPortal>().CountPortal = 5;
        State = Define.State.Idle;
    }
}