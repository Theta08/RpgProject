using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bottom_HPBar : UI_Base
{
    enum GameObjects
    {
        Bottom_HPBar
    }
    
    Stat _stat;
    
    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        
        _stat = Managers.Game.GetPlayer().GetComponent<Stat>();
    }

    void Update()
    {
        float ratio = _stat.Hp / (float)_stat.MaxHp;
        
        SetHpRatio(ratio);
    }
    
    public void SetHpRatio(float ratio)
    {
        GetObject((int)GameObjects.Bottom_HPBar).GetComponent<Slider>().value = ratio;
    }
}
