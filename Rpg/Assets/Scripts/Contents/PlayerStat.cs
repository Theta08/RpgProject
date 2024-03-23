using System.Collections;
using System.Collections.Generic;
using Protobuf.Unity;
using UnityEngine;

public class PlayerStat : Stat
{
    [SerializeField] 
    protected int _exp;
    [SerializeField] 
    float _attackRange = 1;

    public float AttackRange
    {
        get { return _attackRange; }
        set { _attackRange = value; }
    }

	private void Start()
	{
		_level = 1;
		_moveSpeed = 5.0f;
		SetStat(_level);
	}

	public void SetStat(int level)
	{
		Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
		Data.Stat stat = dict[level];
		Debug.Log($"HP {stat.maxHp}");
		_hp = stat.maxHp;
		_maxHp = stat.maxHp;
		_attack = stat.attack;
	}

	protected override void OnDead(Stat attacker)
	{
		// Debug.Log("Player Dead");
	}
}