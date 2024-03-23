using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum WorldObject
    {
        Unknown,
        Player,
        MyPlayer,
        Monster,
    }

    public enum State
    {
        Idle,
        Die,
        Damage,
        Moving,
        Skill,
        Skill1,
        Skill2,
        AttackWait = 14,
    }

    public enum Layer
    {
        Monster = 8,
        Ground = 9,
        Player = 10,
        Block = 11,
    }

    public enum Scene
    {
        Unknown,
        Login,
        Lobby,
        Game,
        BossGameScene,
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }

    public enum UIEvent
    {
        Click,
        Drag,
    }

    public enum MouseEvent
    {
        Press,
        PointerDown,
        PointerUp,
        Click,
    }

    public enum CameraMode
    {
        QuarterView,
    }

    public enum CharacterJob
    {
        Sword,
        Archer,
    }

    public enum MonsterJob
    {
        Knight,
        UD_Spearman,
        UD_Boss,
    }
}