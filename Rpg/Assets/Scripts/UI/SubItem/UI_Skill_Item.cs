using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class UI_Skill_Item : UI_Base
{
    enum GameObjects
    {
        SkillImg,
        SkillText,
        SkillCoolTimeText,
    }

    enum Texts
    {
        SkillText,
        SkillCoolTimeText
    }
    enum Imgs
    {
        SkillImg
    }

    #region skillCoolData
    [SerializeField]
    private float _coolTime;
    [SerializeField]
    private bool _isCoolTime = false;
    [SerializeField]
    private TextMeshProUGUI _coolTimeText;
    
    public float CoolTime { get { return _coolTime;} set { _coolTime = value; } }
    public bool IsCoolTime { get { return _isCoolTime;} set { _isCoolTime = value; } }
    #endregion
    
    private float _timeCurrent;
    private float _startTimer;
    private string _iconImgUrl;

    [SerializeField]
    private string _name;
    [SerializeField]
    private Image _image;
    
    public string TextName { get { return _name;} set { _name = value; } }
    public string IconImgUrl { get { return _iconImgUrl;} set { _iconImgUrl = value; } }
 
    
    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Imgs));
        
        GetText((int)Texts.SkillText).GetComponent<TextMeshProUGUI>().text = _name;
        GetText((int)Texts.SkillText).GetComponent<TextMeshProUGUI>().color = Color.white;
        
        _isCoolTime = false;
        _coolTimeText = GetText((int)Texts.SkillCoolTimeText);
        
        _image = GetImage((int)Imgs.SkillImg);
        _image = GetImage((int)GameObjects.SkillImg).gameObject.GetComponent<Image>();
        
        _image.type = Image.Type.Filled;
        _image.fillMethod = Image.FillMethod.Radial360;
        _image.fillOrigin = (int)Image.Origin360.Top;
        _image.fillClockwise = false;
        
        _coolTimeText.gameObject.SetActive(false);
        
        SetSkillIcon();
        
        Managers.Input.KeyAction -= OnKeyEvent;
        Managers.Input.KeyAction += OnKeyEvent;
    }



    void OnKeyEvent()
    {
        
        if (Input.inputString == _name.ToLower() &&
            Input.GetKeyDown(_name.ToLower()) && !_isCoolTime)
        {

            switch (_name)
            {
                case "Q":
                    Managers.Game.PlayerInfo.State = Define.State.Skill1;
                    break;
                case "W":
                    Managers.Game.PlayerInfo.State = Define.State.Skill2;
                    break;
            }
            
            _isCoolTime = true;
            _coolTimeText.gameObject.SetActive(true);
            
            StartCoroutine("ResetCoolTime");
        }
    }
    
    private IEnumerator ResetCoolTime()
    {
        _coolTimeText.gameObject.SetActive(true);
        
        _timeCurrent = _coolTime;

        while (_timeCurrent > 0)
        {
            _timeCurrent -= Time.deltaTime;
            _image.fillAmount = (_timeCurrent / _coolTime);

            _coolTimeText.text = _timeCurrent.ToString("F");
                
            yield return new WaitForFixedUpdate();
        }

        _coolTimeText.gameObject.SetActive(false);
        
        _image.fillAmount = 1;
        _isCoolTime = false;
        // isEnded = false;
    }

    void SetSkillIcon()
    {
        string url = $"Art/Skill_Icon/{_iconImgUrl}";
        Sprite setIcon = Managers.Resource.Load<Sprite>($"{url}");
        _image.sprite = setIcon;
    }
}
