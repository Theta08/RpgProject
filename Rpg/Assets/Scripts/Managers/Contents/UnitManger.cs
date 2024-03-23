using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitManger : MonoBehaviour
{
    // 일단 Game
    // 플레이어 데이터 !!!
    Dictionary<int, GameObject> _players = new Dictionary<int, GameObject>();
    // 몬스터 데이터 !!!
    Dictionary<int, GameObject> _monsters = new Dictionary<int, GameObject>();
    
    public Dictionary<int, GameObject> Players { get { return _players; } }
    public Dictionary<int, GameObject> Monsters { get { return _monsters; } }
    
    public void ClearSceneObject()
    {
        // 포탈이동시 현재 맵의 모든 유닛 제거필요 Players, Monsters
        // 그후 이동한다는 메시지 서버로 전달 !!! -> 이건 포탈이동하는 쪽에서 ClearSceneObject()호출후에 
        foreach (KeyValuePair<int, GameObject> entry in _players)
        {
            if (!entry.Value.IsDestroyed())
                Destroy(entry.Value);
        }
        foreach (KeyValuePair<int, GameObject> entry in _monsters)
        {
            if (!entry.Value.IsDestroyed())
                Destroy(entry.Value);
        }
        
        _players.Clear();
        _monsters.Clear();
    }

    public bool IsPlayer(int code)
    {
        return _players.ContainsKey(code);
    }
    
    public bool IsMonster(int code)
    {
        return _monsters.ContainsKey(code);
    }

    public bool AddPlayer(int code, GameObject obj)
    {
        if (IsPlayer(code))
        {
            // 에러 존재하는 플레이어
            return false;
        }
        _players[code] = obj;
        return true;
    }

    public bool AddMonster(int code, GameObject obj)
    {
        if (IsMonster(code))
        {
            // 에러 존재하는 플레이어
            return false;
        }
        _monsters[code] = obj;
        return true;
    }

    public bool RemovePlayer(int code)
    {
        if (IsPlayer(code))
        {
            return _players.Remove(code);
        }

        return false;
    }

    public bool RemoveMonster(int code)
    {
        if (IsMonster(code))
        {
            return _monsters.Remove(code);
        }

        return false;
    }
}
