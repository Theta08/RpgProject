using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Top_HPBar : UI_Base
{
    enum GameObjects
    {
        Top_HPBar,
        HpText,
    }

    Stat _stat;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        if (Managers.Game.BossMonsterInfo != null)
            _stat = Managers.Game.BossMonsterInfo.Stat;
    }

    private void OnEnable()
    {
        if (Managers.Game.BossMonsterInfo != null)
            _stat = Managers.Game.BossMonsterInfo.Stat;
    }

    void Update()
    {
        if (Managers.Game.BossMonsterInfo != null)
        {
            _stat = Managers.Game.BossMonsterInfo.Stat;
            float ratio = _stat.Hp / (float) _stat.MaxHp;

            SetHpRatio(ratio);
        }
    }

    public void SetHpRatio(float ratio)
    {
        GetObject((int) GameObjects.Top_HPBar).GetComponent<Slider>().value = ratio;
        Get<GameObject>((int) GameObjects.HpText).GetComponent<TextMeshProUGUI>().text =
            $"{_stat.Hp} / {(float) _stat.MaxHp}";
        
    }
}