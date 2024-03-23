using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawningPool : MonoBehaviour
{
    [SerializeField]
    int _monsterCount = 0;
    int _reserveCount = 0;

    [SerializeField]
    int _keepMonsterCount = 0;

    [SerializeField]
    Vector3 _spawnPos;
    [SerializeField]
    float _spawnRadius = 8.5f;
    [SerializeField]
    float _spawnTime = 5.0f;

    public void AddMonsterCount(int value) { _monsterCount += value; }
    public void SetKeepMonsterCount(int count) { _keepMonsterCount = count; }
    public void SetSpawnPos(Vector3 pos) { _spawnPos = pos;}

    void Start()
    {
        Managers.Game.OnSpawnEvent -= AddMonsterCount;
        Managers.Game.OnSpawnEvent += AddMonsterCount;
    }

    void Update()
    {
        if (_reserveCount + _monsterCount < _keepMonsterCount)
        {
           
            StartCoroutine("ReserveSpawn");
        }
    }

    IEnumerator ReserveSpawn()
    {
        int random = Random.Range(0, 2);
        string monsterName = "";
        
        _reserveCount++;
        yield return new WaitForSeconds(Random.Range(0, _spawnTime));
        
        switch (random)
        {
            case 0: monsterName = "Knight";
                break;
            case 1: monsterName = "UD_Spearman";
                break;
        }
        
        GameObject obj = Managers.Game.Spawn(Define.WorldObject.Monster, monsterName);
        NavMeshAgent nma = obj.GetOrAddComponent<NavMeshAgent>();

        Vector3 randPos;
        // 랜덤 생성
        while (true)
        {
            Vector3 randDir = Random.insideUnitSphere * Random.Range(0, _spawnRadius);
			randDir.y = 0;
			randPos = _spawnPos + randDir;

            // 갈 수 있나
            NavMeshPath path = new NavMeshPath();
            if (nma.CalculatePath(randPos, path))
                break;
		}

        obj.transform.position = randPos;
        _reserveCount--;
    }
}
