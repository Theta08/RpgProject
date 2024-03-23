using Protobuf.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Chatting : UI_Scene
{
    enum Buttons
    {
        ChattingButton,
    }

    enum InputField
    {
        ChattingInputField,
    }

    enum GameObjects
    {
        ChattingPanel,
        Content,
    }

    [SerializeField] private TMP_InputField _inputField;

    [SerializeField] private GameObject textChatPrefab; // 대화가 출력하는 Text UI

    [SerializeField] private Transform parentContent; // 대화가 출력되는 ScrollView의 Content
    [SerializeField] private string _playerId;

    public string PlayerId
    {
        get { return _playerId; }
        set { _playerId = value; }
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        Bind<TMP_InputField>(typeof(InputField));

        GetButton((int) Buttons.ChattingButton).gameObject.BindEvent(OnButtonClicked);
        
        _inputField = Get<TMP_InputField>((int) InputField.ChattingInputField);
        _inputField.onEndEdit.AddListener(arg0 => OnEndEditeEventMethod());
        textChatPrefab = Managers.Resource.Instantiate($"UI/SubItem/UI_TextChat");

        // GameObject go = Get<GameObject>((int) GameObjects.ChattingPanel);
        // GameObject go2 = Util.FindChild(go, "ScrollView");
        // GameObject go3 = Util.FindChild(go2, "Viewport");
        // // GameObject go4 = Util.FindChild(go3, "Content");
        GameObject go4 = GetObject((int)GameObjects.Content).gameObject;
        
        parentContent = go4.transform;

        Managers.Chatting.InputField = _inputField;
        Managers.Chatting.ParentContent = parentContent;
        Managers.Chatting.TextChatPrefab = textChatPrefab;
    }

    void Update()
    {
        // 대화 입력창이 포커스 되어있지 않을 때 Enter키를 누르면
        if (Input.GetKeyDown(KeyCode.Return) && _inputField.isFocused == false)
        {
            // 대화 입력창의 포커스 활성화
            _inputField.ActivateInputField();
        }

        //  받을 체팅 목록 있으면 호출 해야함
        // UpdateChat();
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

        // 대화 내용 출력을 위해 Text UI 생성 (textChatPrefab을 복제 생성 -> parentContent의 자식으로 배치)
        GameObject clone = Instantiate(textChatPrefab, parentContent);

        // 대화 입력창에 있는 내용을 대화창에 출력 ( Id : 내용)
        // if() 자기 자신이면 
        clone.GetComponent<TextMeshProUGUI>().text
            = $"<color=#7F7E83FF>{Managers.Game.PlayerName}</color> : {_inputField.text}";
        ChatPacket(_inputField.text);
        _inputField.text = "";
    }
    
    public void OnButtonClicked(PointerEventData data)
    {
        UpdateChat();
    }

    public void ChatPacket(string text)
    {
        // 패킷 이동 메시지 전달.
        SChat pkt = new SChat();
        pkt.Code = 0;
        pkt.Text = text;
        pkt.Type = 1;
        Managers.SocketInstance.Send(pkt, (ushort) MessageCode.SChat.GetHashCode());
    }
}