using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
	[SerializeField]
	protected Vector3 _destPos;

	[SerializeField]
	protected Define.State _state = Define.State.Idle;

	[SerializeField]
	protected GameObject _lockTarget;

	[SerializeField]
	protected Stat _stat;

	public Define.WorldObject WorldObjectType { get; protected set; } = Define.WorldObject.Unknown;

	public virtual Define.State State
	{
		get { return _state; }
		set
		{
			_state = value;

			Animator anim = GetComponent<Animator>();
			switch (_state)
			{
				case Define.State.Die:
					anim.CrossFade("DEATH", 0.1f);
					break;
				case Define.State.Damage:
					anim.CrossFade("DAMAGE", 0.1f);
					break;
				case Define.State.Idle:
					anim.CrossFade("WAIT", 0.1f);
					break;
				case Define.State.AttackWait:
					anim.CrossFade("ATTACK_WAIT", 0.1f);
					break;
				case Define.State.Moving:
					anim.CrossFade("RUN", 0.1f);
					break;
				case Define.State.Skill:
					anim.CrossFade("ATTACK", 0.1f, -1, 0);
					break;
				case Define.State.Skill1:
					anim.CrossFade("DAMAGE", 0.1f, -1, 0);
					break;
				case Define.State.Skill2:
					anim.CrossFade("DAMAGE", 0.1f, -1, 0);
					break;
			}
		}
	}

	private void Start()
	{
		Init();
	}

	void Update()
	{

		switch (State)
		{
			case Define.State.Die:
				Invoke("UpdateDie",.9f);
				break;
			case Define.State.Damage:
				UpdateDamage();
				break;
			case Define.State.Moving:
				UpdateMoving();
				break;
			case Define.State.Idle:
				UpdateIdle();
				break;
			case Define.State.AttackWait:
				UpdateAttackWait();
				break;
			case Define.State.Skill:
				UpdateSkill();
				break;
			case Define.State.Skill1:
				UpdateSkill1();
				break;
			case Define.State.Skill2:
				UpdateSkill2();
				break;
		}

		UpdateFuniton();
	}

	public virtual void Init() {}

	protected virtual void UpdateDie() { }
	protected virtual void UpdateDamage() { }
	
	protected virtual void UpdateMoving()
	{
		// 다른 플레이어들
		// 일단 좌표로 순간이동 부터 시킨다.

		// 이동
		Vector3 dir = _stat.Position - transform.position;
		dir.y = 0;

		if (dir.magnitude == 0)
		{
			// 마지막이면 그냥 로테이션 설정하고
			State = Define.State.Idle;
                
			transform.rotation = Quaternion.Euler(0, _stat.Rotate.y, 0);
		}
		else
		{
			float moveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude);
			
			transform.position += dir.normalized * moveDist;
			transform.rotation = Quaternion.Euler(0, _stat.Rotate.y, 0);
		}
	}
	protected virtual void UpdateIdle() { }
	protected virtual void UpdateAttackWait() { }
	protected virtual void UpdateSkill() { }
	protected virtual void UpdateSkill1() { }
	protected virtual void UpdateSkill2() { }
	protected virtual void UpdateFuniton() {}
	
	public Stat Stat
	{
		get { return _stat; }
		set { _stat = value; }
	}

	public void SetStat(float startX, float startY, float startZ, float yew)
	{
		if (_stat == null)
		{
			_stat = gameObject.GetComponent<Stat>();

			Vector3 newStart = new Vector3(startX, startY, startZ);
			transform.position = newStart;
            
			transform.rotation = Quaternion.Euler(0, yew, 0);
		}
	}
}
