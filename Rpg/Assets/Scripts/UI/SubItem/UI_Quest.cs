using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Quest : UI_Base
{
    enum GameObjects
    {
        CountNumber,
    }

    [SerializeField]
    private int _countNumber = 0;
    [SerializeField]
    private int _maxCount = 10;
    [SerializeField]
    private bool _state = false;
    
    public int CountNumber { get { return _countNumber; } set { _countNumber = value; } }
    public int MaxCountNumber { get { return _maxCount; } set { _maxCount = value; } }
    public bool QuestState { get { return _state; } set { _state = value; } }
    
    
    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Get<GameObject>((int)GameObjects.CountNumber).GetComponent<TextMeshProUGUI>().text = $"몬스터 처치 ({_countNumber} / {_maxCount} )";
    }

    private void Update()
    {
        QuestItem(Managers.Scene.CurrentScene.SceneType);
    }

    void QuestItem(Define.Scene scene)
    {
        switch (scene)
        {
            case Define.Scene.Game:
                if(_countNumber <  _maxCount)
                    Get<GameObject>((int)GameObjects.CountNumber).GetComponent<TextMeshProUGUI>().text = $"몬스터 처치 ({_countNumber} / {_maxCount} )";
                else
                {
                    Get<GameObject>((int)GameObjects.CountNumber).GetComponent<TextMeshProUGUI>().text = $"퀘스트 완료 ";

                    _state = true;
                }
                break;
            case Define.Scene.BossGameScene:
                if(_countNumber <  _maxCount)
                    Get<GameObject>((int)GameObjects.CountNumber).GetComponent<TextMeshProUGUI>().text = $"보스 처치 ({_countNumber} / {_maxCount} )";
                else
                {
                    Get<GameObject>((int)GameObjects.CountNumber).GetComponent<TextMeshProUGUI>().text = $"퀘스트 완료 ";

                    _state = true;
                }
                break;
        }
    }
}
