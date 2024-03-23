using System;
using System.Collections;
using System.Collections.Generic;
using Protobuf.Unity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerController : BaseController
{
    private int _mask = (1 << (int) Define.Layer.Ground) | (1 << (int) Define.Layer.Monster);

    private bool _stopSkill = false;
    private float timer = 0;

    [SerializeField] private bool _myPlay = true;

    [SerializeField] private GameObject _grenade;
    [SerializeField] private GameObject _itemTarget;

    private UI_Skill_Item _skillItem;

    private int _targetCode = -1;

    private bool _updateAttackFlag = false;

    public bool UpdateAttackFlag
    {
        get { return _updateAttackFlag; }
        set { _updateAttackFlag = value; }
    }

    public bool MyPlay
    {
        get { return _myPlay; }
        set { _myPlay = value; }
    }

    public override void Init()
    {
        base.Init();

        _stat = gameObject.GetComponent<PlayerStat>();
        _stat.Hp = _stat.MaxHp;

        if (!_myPlay)
            return;

        WorldObjectType = Define.WorldObject.MyPlayer;

        State = Define.State.Idle;

        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;

        Managers.JobManager.PlayerJobSelect(gameObject, Managers.Game.PlayerJob);
        Managers.Game.PlayerInfo = gameObject.GetComponent<PlayerController>();

        if (gameObject.GetComponentInChildren<UI_Name>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_Name>(transform);

        MovePacket();
    }

    protected override void UpdateMoving()
    {
        if (_myPlay)
        {
            // 나일때
            // 몬스터가 내 사정거리보다 가까우면 공격
            if (_lockTarget != null)
            {
                _destPos = _lockTarget.transform.position;
                float distance = (_destPos - transform.position).magnitude;

                if (distance <= ((PlayerStat) _stat).AttackRange)
                {
                    State = Define.State.Skill;
                    _targetCode = _lockTarget.GetComponent<BaseController>().Stat.Code;
                    return;
                }
            }

            // 이동
            Vector3 dir = _destPos - transform.position;
            dir.y = 0;

            if (dir.magnitude < 0.1f)
            {
                State = Define.State.Idle;
                transform.position = _destPos;
                MovePacket(true);
            }
            else
            {
                // Debug.DrawRay(transform.position + Vector3.up * 0.5f, dir.normalized, Color.green);
                if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, LayerMask.GetMask("Block")))
                {
                    if (Input.GetMouseButton(0) == false)
                        State = Define.State.Idle;
                    return;
                }

                float moveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude);

                transform.position += dir.normalized * moveDist;
                transform.rotation =
                    Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
                MovePacket();
            }
        }
        else
        {
            base.UpdateMoving();
        }
    }

    protected override void UpdateSkill()
    {
        if (!_myPlay)
        {
            UpdateMoving();

            if (UpdateAttackFlag)
            {
                DamageItem(transform.position);
                _updateAttackFlag = false;
            }
            return;
        }

        if (_lockTarget != null)
        {
            Vector3 dir = _lockTarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 1);

            DamageItem(dir);
            // 한번 공격하면 onHitEvent에서 공격 진행되므로 _lockTarget더이상 필요가 없다. 그래서 한번 처리 !!!
            _lockTarget = null;
            AttackPacket((int) State, _targetCode);
        }
    }

    protected override void UpdateSkill1()
    {
        if (_myPlay)
        {
            AttackPacket((int) State);
        }

        if (State == Define.State.Skill1 && State != Define.State.Die)
        {
            if (Stat.Job == 1)
            {
                DamageItem2();
            }
            else if (Stat.Job == 0)
            {
                _grenade = Managers.UI.MakeSkill<Skill_Slash>(transform).gameObject;
                _grenade.GetOrAddComponent<Skill_Slash>();
            }


            Debug.Log($"{Stat.Job} q스킬 실행");
        }

        State = Define.State.Idle;
    }

    protected override void UpdateSkill2()
    {
        if (_myPlay)
        {
            AttackPacket((int) State);
        }

        if (State == Define.State.Skill2 || State != Define.State.Die)
        {
            GameObject go = Managers.UI.MakeSkill<Skill_Explosion2>(transform).gameObject;
            // go.GetOrAddComponent<Skill_Explosion2>().GetPostion = gameObject.transform.position + Vector3.up;
            UI_DamageText healText =
                Managers.UI.MakeWorldSpaceUI<UI_DamageText>(transform).GetComponent<UI_DamageText>();
            healText.SetDamage = 60;
            healText.SetColor = Color.green;
            _stat.Hp += 60;
        }

        State = Define.State.Idle;
    }


    protected override void UpdateDamage()
    {
    }

    public void NameChange(string name)
    {
        if (gameObject.GetComponentInChildren<UI_Name>() != null)
            gameObject.GetComponentInChildren<UI_Name>().SetName(name);
        else
        {
            _stat.ObjectName = name;
            Managers.UI.MakeWorldSpaceUI<UI_Name>(transform).SetName(name);
        }
    }

    void OnHitEvent()
    {
        if (!_myPlay)
            return;

        if (_stopSkill)
        {
            State = Define.State.Idle;
        }
        else
        {
            State = Define.State.Skill;

            _lockTarget = _itemTarget;

            AttackPacket((int) State, _targetCode);
        }
    }

    void DamageItem(Vector3 dir)
    {
        if (Stat.Job == 1)
        {
            if (!_myPlay)
            {
                GameObject go = Managers.Resource.Instantiate("AtackItem/Arrow");
                go.transform.position = transform.position + Vector3.up;
                go.transform.rotation = transform.rotation;
                go.GetComponent<Arrow>().Setting(dir, null);
                go.GetComponent<Arrow>().SettingRange(((PlayerStat) _stat).AttackRange, transform.position);
            }
            else
            {
                GameObject go = Managers.Resource.Instantiate("AtackItem/Arrow");
                go.transform.position = transform.position;
                go.GetComponent<Arrow>().Setting(dir, _itemTarget);
                go.GetComponent<Arrow>().SettingRange(((PlayerStat) _stat).AttackRange, transform.position);
            }
        }
    }

    void DamageItem2()
    {
        GameObject go = Managers.Resource.Instantiate("AtackItem/Arrow_Regular");
        go.transform.position = transform.position + Vector3.up;
        go.transform.rotation = transform.rotation;

        Arrow_Regular arrow = go.GetComponent<Arrow_Regular>();
        arrow.Peneetrate = true;
        arrow.Init();
        arrow.SettingRange(5.0f, transform.position);
    }

    public void OnDamageEvent(int damage)
    {
        _stat.OnAttacked(damage);
    }

    void OnMouseEvent(Define.MouseEvent evt)
    {
        if (!MyPlay)
            return;

        switch (State)
        {
            case Define.State.Idle:
                OnMouseEvent_IdleRun(evt);
                break;
            case Define.State.Moving:
                OnMouseEvent_IdleRun(evt);
                break;
            case Define.State.Skill:
            {
                if (evt == Define.MouseEvent.PointerUp)
                    _stopSkill = true;
            }
                break;
        }
    }

    void OnMouseEvent_IdleRun(Define.MouseEvent evt)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        bool raycastHit = Physics.Raycast(ray, out hit, 100.0f, _mask);

        // Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);
        // Debug.Log($"layer {hit.collider.gameObject.layer}");

        switch (evt)
        {
            case Define.MouseEvent.PointerDown:
            {
                if (raycastHit)
                {
                    _destPos = hit.point;
                    State = Define.State.Moving;
                    _stopSkill = false;

                    if (hit.collider.gameObject.layer == (int) Define.Layer.Monster)
                    {
                        _lockTarget = hit.collider.gameObject;
                        _itemTarget = hit.collider.gameObject;
                    }
                }
            }
                break;
            case Define.MouseEvent.Press:
            {
                if (_lockTarget == null && raycastHit)
                    _destPos = hit.point;
            }
                break;
            case Define.MouseEvent.PointerUp:
                _stopSkill = true;
                break;
        }
    }

    #region 서버 통신

    private void MovePacket(bool force = false)
    {
        if (!_myPlay)
            return;

        timer += Time.deltaTime;

        // 패킷 이동 메시지 전달.
        if (timer >= 0.2 || force)
        {
            Vector3 position = transform.position;
            SMove pkt = new SMove();
            // 플레이어코드 나중에 필요
            pkt.Code = 1;
            pkt.Position = new Position();
            pkt.Position.X = position.x;
            pkt.Position.Y = position.y;
            pkt.Position.Z = position.z;
            // 방향 일단 o
            pkt.Position.Yaw = transform.rotation.eulerAngles.y;
            Managers.SocketInstance.Send(pkt, (ushort) MessageCode.SMove.GetHashCode());
            timer = 0;
        }
    }

    private void AttackPacket(int skillCode = 4, int targetCode = -1)
    {
        // 패킷 공격 메시지 전달.
        CPlayerAttack pkt = new CPlayerAttack();
        // pkt.Code = 0 필요없음
        pkt.SkillCode = skillCode;
        pkt.TargetCode = targetCode;

        Vector3 position = transform.position;
        pkt.Position = new Position();
        pkt.Position.X = position.x;
        pkt.Position.Y = position.y;
        pkt.Position.Z = position.z;
        pkt.Position.Yaw = transform.rotation.eulerAngles.y;

        Managers.SocketInstance.Send(pkt, (ushort) MessageCode.CPlayerattack.GetHashCode());
        // 하는 이유 공격할때 move패킷처럼 보내므로써 move를 연속해서 안보내게끔 한다.!!! 
        timer = 0;
    }

    #endregion
}