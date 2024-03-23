using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_DamageText : UI_Base
{
    [SerializeField]
    private float _moveSpeed = 1.0f;
    [SerializeField]
    private float _alphaSpeed = 2.0f;
    [SerializeField]
    private int _damage;
    [SerializeField]
    private Color _color = Color.red;
    private Color alpha;
    
    enum GameObjects
    {
        DamageText,
    }
    
    public int SetDamage { set { _damage = value; } }
    public Color SetColor { set { _color = value; } }
    
    public override void Init()
    {
        Transform parent = transform.parent;
        Bind<TextMeshProUGUI>(typeof(GameObjects));
        
        alpha = GetText((int)GameObjects.DamageText).color = _color;
        
        transform.position = parent.position + Vector3.up * (((parent.GetComponent<Collider>().bounds.size.y)) * 1.3f);
        
    }
    

    void Update()
    {
        transform.Translate(new Vector3(0, _moveSpeed * Time.deltaTime,0));
        transform.rotation = Camera.main.transform.rotation;
        
        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * _alphaSpeed);

        GetText((int)GameObjects.DamageText).text = $"{_damage}";
        GetText((int)GameObjects.DamageText).color = alpha;
        
        if(alpha.a <= 0.4f)
            Destroy(gameObject);
    }

}
