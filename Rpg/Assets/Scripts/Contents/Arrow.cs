using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Arrow : MonoBehaviour
{
    [SerializeField] protected int _damage;
    [SerializeField] protected bool _penetrate = false;
    protected Rigidbody _rigidbody;
    protected float _attackRange = 5.0f;
    protected Vector3 _startPosition;

    RaycastHit hit;

    public int Damage
    {
        get { return _damage; }
        set { _damage = value; }
    }

    public bool Peneetrate
    {
        get { return _penetrate; }
        set { _penetrate = value; }
    }

    protected void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Init();
    }
    
    public virtual void Init(){}
    
    protected virtual void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.down * 1f, out hit))
        {
            if (hit.transform.gameObject.layer != 9)
                Managers.Resource.Destroy(gameObject);
        }

        if ((_startPosition - transform.position).magnitude >= _attackRange)
        {
            Managers.Resource.Destroy(gameObject);
        }
    }

    public void Setting(Vector3 dir, GameObject itemTarget)
    {
        if (itemTarget != null)
        {
            transform.position += Vector3.up;
            Vector3 dir_ = itemTarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir_);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 1);

            _rigidbody.velocity = dir * Time.time * 1.2f;
        }
        else
        {
            _rigidbody.velocity = transform.forward * Time.time * 1.0f;
        }
    }

    public void SettingRange(float range, Vector3 start)
    {
        _attackRange = range;
        _startPosition = start;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Monster"))
            return;

        if (!_penetrate)
            Managers.Resource.Destroy(gameObject);
    }
}