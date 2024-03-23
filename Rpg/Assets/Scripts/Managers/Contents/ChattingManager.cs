using System.Collections;
using System.Collections.Generic;
using Protobuf.Unity;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ChattingManager 
{
      private TMP_InputField _inputField;
   private GameObject _textChatPrefab; // 대화가 출력하는 Text UI
   private Transform _parentContent; // 대화가 출력되는 ScrollView의 Content
  
   public TMP_InputField InputField
    {
        get { return _inputField; }
        set { _inputField = value; }
    }

   public GameObject TextChatPrefab
    {
        get { return _textChatPrefab; }
        set { _textChatPrefab = value; }
    }

   public Transform ParentContent
    {
        get { return _parentContent; }
        set { _parentContent = value; }
   }
   
   public void UpdateChat(string name, string text)
   {
      GameObject clone = GameObject.Instantiate(_textChatPrefab, _parentContent);
      clone.GetComponent<TextMeshProUGUI>().text = $"<color=yellow>{name}</color> : {text}";
   }
      
   public void ChatPacket(string text)
   {
      // 채팅 패킷 전달
      SChat pkt = new SChat();
      pkt.Code = 0;
      pkt.Text = text;
      pkt.Type = 1;
      Managers.SocketInstance.Send(pkt, (ushort) MessageCode.SChat.GetHashCode());
   }
}