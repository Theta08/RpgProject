using System;
using Protobuf.Unity;
using UnityEngine;

public class SocketComponent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        _socket = Managers.SocketInstance;
    }

    // Update is called once per frame
    void Update()
    {
        // read 체크

        int count = _socket.ReadPacket.GetPackets.Count;
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                PacketCode pc = _socket.ReadPacket.PopPacket();
                if (pc == null)
                    break;

                if (pc.pkt.CalculateSize() == 0)
                    continue;
                // ?
                MessageCode code = (MessageCode) pc.code;

                switch (code)
                {
                    case MessageCode.SLoad:
                    {
                        SLoad pkt = (SLoad) pc.pkt;

                        Managers.Game.RoomId = pkt.RoomId;
                        for (int j = 0; j < pkt.Player.Count; j++)
                        {
                            Player player = pkt.Player[j];
                            SpawnPlayer(player);
                        }

                        for (int j = 0; j < pkt.Monster.Count; j++)
                        {
                            Monster monster = pkt.Monster[j];
                            SpawnMonster(monster);
                        }
                    }
                        break;

                    case MessageCode.SMove:
                    {
                        SMove smove = (SMove) pc.pkt;
                        MoveHandler(smove.Code, smove.IsMonster, smove.Position);
                    }
                        break;

                    case MessageCode.SInsertplayer:
                    {
                        SInsertplayer pkt = (SInsertplayer) pc.pkt;
                        Player player = pkt.Player;
                        SpawnPlayer(player);
                    }
                        break;

                    case MessageCode.SChat:
                    {
                        SChat pkt = (SChat) pc.pkt;
                        string name = "";

                        if (Managers.UnitManger.IsPlayer(pkt.Code))
                        {
                            // 다른사람 !!!
                            name = Managers.UnitManger.Players[pkt.Code].GetComponent<PlayerController>().Stat
                                .ObjectName;
                            Managers.Chatting.UpdateChat(name, pkt.Text);
                        }
                        else
                        {
                            // 나 !!!
                            name = Managers.Game.PlayerName;
                        }

                        Debug.LogFormat("{0} 플레이어  채팅 : {1}", name, pkt.Text);
                        // 여기서 UpdateChat(string text, string name) => 요거 호출하도록 해야됨 !!! 자기자신도 여기서 처리할지는 알아서~~~
                    }
                        break;

                    case MessageCode.SCloseplayer:
                    {
                        SClosePlayer pkt = (SClosePlayer) pc.pkt;
                        int playerCode = pkt.Code;

                        if (Managers.UnitManger.IsPlayer(playerCode))
                        {
                            Managers.Resource.Destroy(Managers.UnitManger.Players[playerCode]);
                            Managers.UnitManger.RemovePlayer(playerCode);
                            Debug.LogFormat("플레이어 {0} 번 종료", playerCode);
                        }
                    }
                        break;

                    case MessageCode.SUnitstates:
                    {
                        SUnitStates sUnitStates = (SUnitStates) pc.pkt;
                        for (int j = 0; j < sUnitStates.UnitState.Count; j++)
                        {
                            UnitState unitState = (UnitState) sUnitStates.UnitState[j];
                            if (unitState.IsMonster)
                            {
                                Monster monster = unitState.Monster;
                                Unit unit = monster.Unit;
                                int unitCode = unit.Code;
                                if (Define.State.Moving == (Define.State) monster.State)
                                {
                                    MoveHandler(unitCode, unitState.IsMonster, unit.Position);
                                }
                                else if (Define.State.Damage == (Define.State) monster.State)
                                {
                                    int damage = unitState.Demage;
                                    // Managers.UnitManger.Monsters[unitCode].GetComponent<BaseController>().Stat.Hp = (int)unit.Hp;

                                    // 일단 
                                    if (Managers.UnitManger.Monsters[unitCode].GetComponent<BaseController>().Stat.Job == 2)
                                    {
                                        Managers.UnitManger.Monsters[unitCode].GetComponent<BossMonsterController>()
                                            .OnDamage(damage);
                                    }
                                    else
                                    {
                                        Managers.UnitManger.Monsters[unitCode].GetComponent<MonsterController>()
                                            .OnDamage(damage);
                                    }
                                    Debug.LogFormat("몬스터 {0} 번 피격됨 {1} 체력 {2}", unitCode, damage, unit.Hp);
                                }
                                else if (Define.State.Die == (Define.State) monster.State)
                                {
                                    Debug.LogFormat("몬스터 {0} 번 사망", unitCode);
                                }
                                else if (Define.State.Idle == (Define.State) monster.State)
                                {
                                    SpawnMonster(monster);
                                    Debug.LogFormat("몬스터 {0} 번 리스폰", unitCode);
                                }

                                if (Managers.UnitManger.IsMonster(unitCode))
                                {
                                    Managers.UnitManger.Monsters[unitCode].GetComponent<BaseController>().State =
                                        (Define.State) monster.State;
                                }
                            }
                            else
                            {
                                Player player = unitState.Player;
                                Unit unit = player.Unit;
                                int unitCode = unit.Code;
                                int damage = unitState.Demage;
                                if (unitCode == Managers.Game.PlayerInfo.Stat.Code)
                                {
                                    // 내 데미지 띄움 일단 이거 안되는거 같기도함 확인안함. 로그는 나옴
                                    Managers.Game.PlayerInfo.State = Define.State.Damage;
                                }

                                Debug.LogFormat("플레이어 {0} 번 피격됨 {1} 체력 {2}", unitCode, damage, unit.Hp);

                                if (Managers.UnitManger.IsPlayer(unitCode))
                                {
                                    Managers.UnitManger.Players[unitCode].GetComponent<PlayerController>()
                                        .OnDamageEvent(damage);
                                    Managers.UnitManger.Players[unitCode].GetComponent<PlayerController>().Stat.Hp = (int) unit.Hp;
                                }
                                else
                                {
                                    Managers.Game.PlayerInfo.OnDamageEvent(damage);
                                    Managers.Game.PlayerInfo.Stat.Hp = (int) unit.Hp;
                                }
                            }
                        }
                    }
                        break;

                    case MessageCode.SUnitattack:
                    {
                        SUnitAttack sUnitAttack = (SUnitAttack) pc.pkt;
                        for (int j = 0; j < sUnitAttack.Attack.Count; j++)
                        {
                            int playerCode = sUnitAttack.Attack[j].Code;
                            int skillCode = sUnitAttack.Attack[j].SkillCode;
                            bool isMonster = sUnitAttack.Attack[j].IsMonster;
                            
                            MoveHandler(playerCode, isMonster, sUnitAttack.Attack[j]. Position);
                            
                            Debug.LogFormat("플레이어 {0} 번 공격 스킬 {1}", playerCode, skillCode);
                            Managers.UnitManger.Players[playerCode].GetComponent<BaseController>().State =
                                (Define.State) skillCode;
                            Managers.UnitManger.Players[playerCode].GetComponent<PlayerController>().UpdateAttackFlag =
                                true;
                        }
                    }
                        break;

                    case MessageCode.SUnitbuff:
                    {
                        SUnitBuff sUnitBuff = (SUnitBuff) pc.pkt;
                        for (int j = 0; j < sUnitBuff.Buff.Count; j++)
                        {
                            Buff buff = sUnitBuff.Buff[j];

                            if (!buff.IsMonster)
                            {
                                int heal = buff.Heal;
                                int hp = buff.Hp;
                                int playerCode = buff.Code;
                                int skillCode = buff.SkillCode;
                                Debug.LogFormat("플레이어 {0} 번 회복량 {1} 체력 {2}", playerCode, heal, hp);
                                if (Managers.UnitManger.IsPlayer(playerCode))
                                {
                                    Managers.UnitManger.Players[playerCode].GetComponent<BaseController>().State =
                                        (Define.State) skillCode;
                                }
                                else
                                {
                                    // 나 !!! => 나 체크 좀 if 하나 넣어주는게 좋음
                                    if (Managers.Game.PlayerInfo.Stat.Code == playerCode)
                                    {
                                        Managers.Game.PlayerInfo.State = (Define.State) skillCode;
                                    }
                                    else
                                    {
                                        Debug.LogFormat("플레이어 {0} 번 해당 플레이어는 없습니다. !!!", playerCode);
                                    }
                                }
                            }
                        }
                    }
                        break;

                    case MessageCode.SRoomquest:
                    {
                        SRoomQuest sRoomQuest = (SRoomQuest) pc.pkt;
                        bool isClear = sRoomQuest.IsClear;
                        int killCount = sRoomQuest.KillCount;
                        int sumKill = sRoomQuest.SumKill;

                        GameObject goQuest = GameObject.Find("UI_Quest");
                        if (goQuest != null)
                        {
                            UI_Quest quest = goQuest.GetOrAddComponent<UI_Quest>();

                            quest.CountNumber = killCount;

                            Debug.LogFormat("퀘스트 완료 : {0}, 처리된 몬스터 : {1}, 처리해야되는 몬스터 숫자 {2}. !!!", isClear, killCount,
                                sumKill);

                            if (isClear)
                            {
                                // 여기서 그냥 isClear == ture 
                                Managers.UnitManger.ClearSceneObject();
                                // 포탈이동 날린다.
                                Managers.Scene.LoadScene(Define.Scene.BossGameScene);
                                // 여기서 바로 이동 해야됨 현재 몬스터 플레이어 destroy랑 함께 !!!
                            }
                        }
                    }
                        break;
                }
            }
        }
    }

    private void SetUnitData(Unit unit, BaseController controller)
    {
        Position playerPosition = unit.Position;
        float x = 0;
        float y = 0; // 버려도 된다 !!! ( 높이축 안쓴다.)
        float z = 0;
        float yaw = 0f;
        if (playerPosition != null)
        {
            x = playerPosition.X;
            y = playerPosition.Y; // 버려도 된다 !!! ( 높이축 안쓴다.)
            z = playerPosition.Z;
            yaw = playerPosition.Yaw;
        }

        uint type = unit.Type;
        uint hp = unit.Hp;
        int code = unit.Code;
        string unitName = unit.Name;

        if (controller != null)
        {
            controller.SetStat(x, y, z, yaw);
            Vector3 pos = controller.Stat.Position;
            pos.x = x;
            pos.z = z;

            controller.Stat.Position = pos;
            Quaternion rot = controller.Stat.Rotate;
            rot.y = yaw;
            controller.Stat.Rotate = rot;

            controller.Stat.ObjectName = unitName;
            controller.Stat.MaxHp = (int) hp;
            controller.Stat.Hp = (int) hp;
            controller.Stat.Job = (int) type;
            controller.Stat.Code = (int) code;
        }
    }

    private void SpawnMonster(Monster monster)
    {
        int code = monster.Unit.Code;
        uint type = monster.Unit.Type;
        uint state = monster.State;

        if (!Managers.UnitManger.IsMonster(code))
        {
            if (Define.State.Die == (Define.State) state)
            {
                return;
            }

            // 플레이어 code 넣기 플레이어들 구분용
            // Managers.Game.PlayerCode = (int)player.Code;
            string jobString = Managers.JobManager.IntToStringMonsterJob((int) type);
            Managers.UnitManger.Monsters[code] =
                Managers.Game.Spawn(Define.WorldObject.Monster, jobString);
            SetUnitData(monster.Unit, Managers.UnitManger.Monsters[code].GetComponent<BaseController>());
            Managers.UnitManger.Monsters[code].GetComponent<BaseController>().State = (Define.State) state;

            Debug.LogFormat("몬스터 {0} 번 스폰", code);
        }
    }

    private void SpawnPlayer(Player player)
    {
        int code = player.Unit.Code;
        uint type = player.Unit.Type;

        if (!Managers.UnitManger.IsPlayer(code))
        {
            // 플레이어 code 넣기 플레이어들 구분용
            // Managers.Game.PlayerCode = (int)player.Code;

            string jobString = Managers.JobManager.IntToStringPlayerJob((int) type);
            
            Vector3 pos = new Vector3(player.Unit.Position.X, player.Unit.Position.Y, player.Unit.Position.Z);
            Quaternion rot = Quaternion.Euler(0f, player.Unit.Position.Yaw, 0f);
            Managers.UnitManger.AddPlayer(code, Managers.Game.Spawn(Define.WorldObject.Player, jobString, pos, rot));
            SetUnitData(player.Unit, Managers.UnitManger.Players[code].GetComponent<PlayerController>());
            
            Managers.UnitManger.Players[code].GetComponent<PlayerController>().NameChange($"{player.Unit.Name}");
            Managers.UnitManger.Players[code].GetComponent<PlayerController>().State = Define.State.Moving;
            if (player.Unit.Type == 0)
            {
                ((PlayerStat)Managers.UnitManger.Players[code].GetComponent<PlayerController>().Stat).AttackRange = 2.0f;
            }
            else
            {
                ((PlayerStat)Managers.UnitManger.Players[code].GetComponent<PlayerController>().Stat).AttackRange = 5.0f;
            }
            Managers.UnitManger.Players[code].GetComponent<PlayerController>().State = Define.State.Moving;
            Debug.LogFormat("플레이어 {0} 번 스폰", code);
        }
    }

    private void MoveHandler(int code, bool isMonster, Position playerPosition)
    {
        float x = playerPosition.X;
        float y = playerPosition.Y; // 버려도 된다 !!! ( 높이축 안쓴다.)
        float z = playerPosition.Z;
        float yaw = playerPosition.Yaw;
        
        BaseController controller = null;
        if (isMonster)
        {
            if (Managers.UnitManger.IsMonster(code))
            {
                controller = Managers.UnitManger.Monsters[code].GetComponent<BaseController>();
            }
        }
        else
        {
            if (Managers.UnitManger.IsPlayer(code))
            {
                controller = Managers.UnitManger.Players[code].GetComponent<BaseController>();
            }
        }

        if (controller != null)
        {
            Vector3 pos = controller.Stat.Position;
            pos.x = x;
            pos.z = z;

            controller.Stat.Position = pos;
            Quaternion rot = controller.Stat.Rotate;
            rot.y = yaw;
            controller.Stat.Rotate = rot;
            controller.State = Define.State.Moving;
        }
    }

    private SocketGenerator _socket = null;
}