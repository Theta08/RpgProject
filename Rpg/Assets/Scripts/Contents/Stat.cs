using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Stat : MonoBehaviour
{
    #region Stat
    [SerializeField]
    protected int _level;
    [SerializeField]
    protected int _hp;
    [SerializeField]
    protected int _maxHp;
    [SerializeField]
    protected int _attack;
    [SerializeField]
    protected int _defense;
    [SerializeField]
    protected float _moveSpeed;
    [SerializeField]
    private string _objectName = "";
    [SerializeField]
    private int _job = 0;
    [SerializeField]
    private int _code = 0; // uuid임 !!!
    #endregion

    #region Stat GetSet
    public int Level { get { return _level; } set { _level = value; } }
    public int Hp { get { return _hp; } set { _hp = value; } }
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; } }
    public int Attack { get { return _attack; } set { _attack = value; } }
    public int Defense { get { return _defense; } set { _defense = value; } }
    public float MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }
    public string ObjectName { get { return _objectName; } set { _objectName = value; } }
    public int Job { get { return _job; } set { _job = value; } }
    public int Code { get { return _code; } set { _code = value; } } // uuid임 !!!
    #endregion

    // 포지션 로테이트 정보 추가
    protected Vector3 _position;
    public Vector3 Position { get { return _position; } set { _position = value; } }
    protected Quaternion _rotate;
    public Quaternion Rotate { get { return _rotate; } set { _rotate = value; } }
    
    private void Start()
    {
        _hp = 100;
        _maxHp = 100;
        _attack = 10;
        _moveSpeed = 3.0f;
    }

    public virtual void OnAttacked(Stat attacker)
    {
		int damage = Mathf.Max(0, attacker.Attack - Defense);
		Hp -= damage;
        
        Managers.UI.MakeWorldSpaceUI<UI_DamageText>(transform).SetDamage = damage;
        // gameObject.transform.parent.GetOrAddComponent<BaseController>().State = Define.State.Damage;
        
        GameObject effect = Managers.UI.MakeSkill<Damage_Hit>(transform).gameObject;
        effect.GetOrAddComponent<Damage_Hit>();
        
        if (Hp <= 0)
        {
            Hp = 0;
            OnDead();
        }

    }
    
    public virtual void OnAttacked(int attacker)
    {
        int damage = Mathf.Max(0, attacker - Defense);
        Hp -= damage;
        
        Managers.UI.MakeWorldSpaceUI<UI_DamageText>(transform).SetDamage = damage;
        
        GameObject effect = Managers.UI.MakeSkill<Damage_Hit>(transform).gameObject;
        effect.GetOrAddComponent<Damage_Hit>();
        
        if (Hp <= 0)
        {
            Hp = 0;
            OnDead();
        }

    }

    public virtual void OnDead()
    {
	
        if (Managers.Scene.CurrentScene.SceneType == Define.Scene.Game)
        {
            // GameObject goQuest = GameObject.Find("UI_Quest");
            // UI_Quest quest = goQuest.GetOrAddComponent<UI_Quest>();
            //
            // quest.CountNumber++;
            //
            // Debug.Log($"sate {quest.CountNumber} ");
        } 

        BaseController myObject = gameObject.GetComponent<BaseController>();
        myObject.State = Define.State.Die;
    }
    
    protected virtual void OnDead(Stat attacker)
    {
        PlayerStat playerStat = attacker as PlayerStat;
        if (playerStat != null)
        {
            // playerStat.Exp += 15;
            
            // GameObject goQuest = GameObject.Find("UI_Quest");
            // UI_Quest quest = goQuest.GetOrAddComponent<UI_Quest>();
            //
            // quest.CountNumber++;
            // Debug.Log(quest.CountNumber);
        }

        BaseController myObject = gameObject.GetComponent<BaseController>();
        myObject.State = Define.State.Die;
        // Managers.Game.Despawn(gameObject);
        }
}
