using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance; // 유일성이 보장된다
    static Managers Instance { get { Init(); return s_instance; } } // 유일한 매니저를 갖고온다

	#region Contents
	GameManagerEx _game = new GameManagerEx();
    ChattingManager _chatting = new ChattingManager();
    JobManager _jobManager = new JobManager();
    UnitManger _unitManger = new UnitManger();
    
    public static GameManagerEx Game {  get { return Instance._game; } }
    public static ChattingManager Chatting {  get { return Instance._chatting; } }
    public static JobManager JobManager {  get { return Instance._jobManager; } }
	public static UnitManger UnitManger { get { return Instance._unitManger; } }
    #endregion

	#region Core
	DataManager _data = new DataManager();
    InputManager _input = new InputManager();
    PoolManager _pool = new PoolManager();
    ResourceManager _resource = new ResourceManager();
    SceneManagerEx _scene = new SceneManagerEx();
    SoundManager _sound = new SoundManager();
    UIManager _ui = new UIManager();

    public static DataManager Data { get { return Instance._data; } }
    public static InputManager Input { get { return Instance._input; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    public static SoundManager Sound { get { return Instance._sound; } }
    public static UIManager UI { get { return Instance._ui; } }
	#endregion

    #region Network
    SocketGenerator _socket = new SocketGenerator();
    
    public static SocketGenerator SocketInstance { get { return Instance._socket; } }
    #endregion

	void Start()
    {
        Init();

        // 소켓 관련 Init 랑 합쳐도 상관 X !!!
        InitSocket();

        SetResolution();
    }

    void Update()
    {
        _input.OnUpdate();
    }

    static void Init()
    {
        if (s_instance == null)
        {
			GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();

            s_instance._data.Init();
            s_instance._pool.Init();
            s_instance._sound.Init();
        }		
	}

    public static void Clear()
    {
        Input.Clear();
        Sound.Clear();
        Scene.Clear();
        UI.Clear();
        Pool.Clear();
    }

    void OnDestroy()
    {
        // 일단 추가합니다.
        if (_socket.IsConnectComplated())
            _socket.Disconnect();
        else 
            _socket.Socket().Close(); // 연결안되어있으면 일단 그냥 죽일게요 !!!
    }
    
    public void InitSocket()
    {
        if (!s_instance._socket.IsConnecting())
        {
            // 서버 포트, 서버 ip 설정 따로 빼두됨 !!!
            const string host = "127.0.0.1";
            const int port = 12128;
            s_instance._socket = new SocketGenerator();
            s_instance._socket.Connect(host, port);
        }
        else
        {
            Debug.Log("이미 연결중입니다 !!!");
        }
    }    
    
    /// <summary>
    /// 해상도 고정 함수 !!!
    /// </summary>
    public void SetResolution()
    {
        int setWidth = 800; // 화면 너비
        int setHeight = 600; // 화면 높이

        //해상도를 설정값에 따라 변경
        //3번째 파라미터는 풀스크린 모드를 설정 > true : 풀스크린, false : 창모드
        Screen.SetResolution(setWidth, setHeight, false);
    }
}
