using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerEx
{
    // 기본 0
    // private string character = "";
    private string _playerName = "";
    private string _playerId = "";
    private int _roomId = 0;
    
    GameObject _player;
    public Action<int> OnSpawnEvent;
    
    private PlayerController _playerController;
    private BossMonsterController _bossMonsterController;
    
    public Define.CharacterJob PlayerJob { get; set; } = Define.CharacterJob.Sword;
    
    public GameObject GetPlayer()
    {
        return _player;
    }

    public string PlayerName
    {
        get { return _playerName; }
        set { _playerName = value; }
    }

    public string PlayerId
    {
        get { return _playerId; }
        set { _playerId = value; }
    }

    public int RoomId
    {
        get { return _roomId; }
        set { _roomId = value; }
    }

    public PlayerController PlayerInfo
    {
        get { return _playerController; }
        set { _playerController = value; }
    }
    
    public BossMonsterController BossMonsterInfo
    {
        get { return _bossMonsterController; }
        set { _bossMonsterController = value; }
    }

    public GameObject Spawn(Define.WorldObject type, string path, Transform parent = null)
    {
        GameObject go = Managers.Resource.Instantiate(path, parent);

        switch (type)
        {
            case Define.WorldObject.Monster:
                // _monsters.Add(go);
                // if (OnSpawnEvent != null)
                    //     OnSpawnEvent.Invoke(1);
                break;
            case Define.WorldObject.Player:
                go.GetComponent<PlayerController>().MyPlay = false;
                break;
            case Define.WorldObject.MyPlayer:
                _player = go;
                break;
            default:
                // _monsters.Add(go);
                if (OnSpawnEvent != null)
                    OnSpawnEvent.Invoke(1);
                break;
        }

        return go;
    }
    
    public GameObject Spawn(Define.WorldObject type, string path, Vector3 pos, Quaternion rot, Transform parent = null)
    {
        GameObject go = Managers.Resource.Instantiate(path, pos, rot, parent);

        switch (type)
        {
            case Define.WorldObject.Monster:
                // _monsters.Add(go);
                // if (OnSpawnEvent != null)
                //     OnSpawnEvent.Invoke(1);
                break;
            case Define.WorldObject.Player:
                go.GetComponent<PlayerController>().MyPlay = false;
                break;
            case Define.WorldObject.MyPlayer:
                _player = go;
                break;
            default:
                // _monsters.Add(go);
                if (OnSpawnEvent != null)
                    OnSpawnEvent.Invoke(1);
                break;
        }

        return go;
    }

    public Define.WorldObject GetWorldObjectType(GameObject go)
    {
        BaseController bc = go.GetComponent<BaseController>();
        if (bc == null)
            return Define.WorldObject.Unknown;

        return bc.WorldObjectType;
    }

}