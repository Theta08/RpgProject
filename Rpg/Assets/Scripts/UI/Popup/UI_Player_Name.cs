using System;
using System.Collections;
using System.Collections.Generic;
using Protobuf.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Player_Name : UI_Popup
{
    enum Buttons
    {
        Check,
        Cancel,
    }

    enum GameObjects
    {
        TestPanel
    }
    
    enum InputField
    {
        UI_InputField,
    }

    private TMP_InputField _inputField;
    private bool isActive = true;
        
    public override void Init()
    {
        base.Init();
        
        gameObject.SetActive(isActive);
        
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        Bind<TMP_InputField>(typeof(InputField));
        
        _inputField = Get<TMP_InputField>((int) InputField.UI_InputField);
        _inputField.onEndEdit.AddListener(arg0 => OnEndEditeEventMethod());
        
        GetButton((int) Buttons.Check).gameObject.BindEvent(OnButtonClicked);
        GetButton((int) Buttons.Cancel).gameObject.BindEvent(OnButtonCancel);
    }

    private void Update()
    {
        gameObject.SetActive(isActive);
            
        if (Input.GetKeyDown(KeyCode.Return) && _inputField.isFocused == false)
            _inputField.ActivateInputField();
    }

    public void OnButtonCancel(PointerEventData data)
    {
        _inputField.text = "";
        gameObject.SetActive(false);
    }
    
    public void OnButtonClicked(PointerEventData data)
    {
            UpdateChat();
    }
    
    public void OnEndEditeEventMethod()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            UpdateChat();
    }
    
    public void UpdateChat()
    {
        if (_inputField.text.Equals(""))
            return;
        
        if (Managers.SocketInstance.IsConnectComplated())
        {
            // Managers.Game.PlayerJob =  _name;
            Managers.Game.PlayerName = _inputField.text;
                
            Login log = new Login();
                
            // 이름 연동
            log.Text = _inputField.text;
            log.Type = (int) Managers.Game.PlayerJob;

            PacketCode pc = new PacketCode();
            
            pc.code = 0;
            pc.pkt = log;
                
            Managers.SocketInstance.SendPacket.PushPacket(pc);
            Managers.SocketInstance.Send(Managers.SocketInstance.SendPacket.PopPacket().pkt, 0);
                
            Managers.Scene.LoadScene(Define.Scene.Game);
        }
        
        _inputField.text = "";
    }
}