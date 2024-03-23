using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Google.Protobuf;
using Protobuf.Unity;
using UnityEngine;

public class SocketGenerator
{
    public SocketGenerator()
    {
        Init();
    }

    ~SocketGenerator()
    {
        Debug.Log("~SocketGenerator !!!");
        // _disconnet();
    }

    private void Init()
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    public void Connect(string addr, int port)
    {
        IPAddress ipAddr = IPAddress.Parse(addr);
        IPEndPoint serverEndPoint = new IPEndPoint(ipAddr, port);

        // 연결 요청용 SocketAsyncEventArgs 객체입니다.
        // 완료 콜백 함수로 OnConnectCompleted()함수를 추가하고
        // 서버의 식별정보를 가진 IPEndPoint로 넣어줍니다.
        SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        args.Completed += OnConnectCompletedHandler;
        args.RemoteEndPoint = serverEndPoint;

        // 연결 요청용 SocketAsyncEventArgs 객체를 인수로 보내 비동기로 연결 요청을 합니다.
        _socket.ConnectAsync(args); // => 요거 아직 분석이 안됬다. 연구 필요 !!!
        _isConnecting = true;
    }

    public void OnConnectCompletedHandler(object obj, SocketAsyncEventArgs args)
    {
        if (args.SocketError == SocketError.Success)
        {
            _isConnected = true;
            Debug.Log("서버와 연결 성공 !!!");

            _socket.BeginReceive(_recvBuffer.GetBuffer(), _recvBuffer.ReadPos(), _recvBuffer.FreeSize(),
                SocketFlags.None,
                new AsyncCallback(OnReciveHandler),
                null); // SocketFlags, 마지막 파라미터 state 이거 같은 경우는 클래스 하나 파서 응답오면 체크 가능함 (사실살 이거 윈도우의 IOCP 개념이다. 서버에서 어떤 클라이언트 소켓인지 알아낼려고 만듬) . 근데 일단 제외
        }
        else
        {
            _isConnected = false;
            _socket.ConnectAsync(args);
            Debug.Log("서버와 연결 실패 !!!");
        }
    }

    public void OnReciveHandler(IAsyncResult result)
    {
        // result.AsyncState; // => 이거 BeginReceive에서 마지막 파라미터에 들은 클래스로 캐스팅해서 확인가능. 그러나 일단제외 (IOCP 개념)
        if (!_socket.Connected)
            return;
        int bytesRead = _socket.EndReceive(result);

        if (bytesRead > 0)
        {
            if (!_recvBuffer.OnWrite(bytesRead))
            {
                Disconnect();
                return;
            }

            int dataSize = _recvBuffer.DataSize();
            int offset = 0;

            while (true)
            {
                // header
                PacketHeader header =
                    PacketHandler.ParsePacketHandler(_recvBuffer.GetBuffer(), _recvBuffer.ReadPos() + offset);

                if (!PacketHandler.CheckPacketHeader(header, offset, bytesRead))
                    break;

                // 패킷 메시지
                HandlePacket(offset + 4, header);

                offset += header.Size;
            }

            if (!_recvBuffer.OnRead(offset) || offset <= 0 || dataSize < offset)
            {
                Disconnect();
                return;
            }

            _recvBuffer.Clean();

            _socket.BeginReceive(_recvBuffer.GetBuffer(), _recvBuffer.ReadPos(), _recvBuffer.FreeSize(),
                SocketFlags.None,
                new AsyncCallback(OnReciveHandler),
                null); // SocketFlags, 마지막 파라미터 state 이거 같은 경우는 클래스 하나 파서 응답오면 체크 가능함. 근데 일단 제외
        }
        else
        {
            Debug.Log("Error 서버 문제 있음 !!!"); // 읽어온 데이터가 0이면 문제 있음 !!!
            Disconnect();
        }
    }

    private void HandlePacket(int offset, PacketHeader header)
    {
        MessageCode code = (MessageCode) header.Id;
        switch (code)
        {
            case MessageCode.Login:
            {
            }
                break;
            case MessageCode.SLoad:
            {
                // 처음 시작할때 이미 존재했던 플레이어들 !!!
                SLoad pkt = SLoad.Parser.ParseFrom(_recvBuffer.GetBuffer(), _recvBuffer.ReadPos() + offset,
                    header.Size - 4);
                PacketCode pc = new PacketCode();

                pc.code = (ushort) MessageCode.SLoad;
                pc.pkt = pkt;
                _readPacketUtils.PushPacket(pc);
            }
                break;
            case MessageCode.SInsertplayer:
            {
                // 플레이중 새로 들어오는 플레이어 !!!
                SInsertplayer pkt = SInsertplayer.Parser.ParseFrom(_recvBuffer.GetBuffer(),
                    _recvBuffer.ReadPos() + offset, header.Size - 4);
                PacketCode pc = new PacketCode();

                pc.code = (ushort) MessageCode.SInsertplayer;
                pc.pkt = pkt;
                _readPacketUtils.PushPacket(pc);
            }
                break;
            case MessageCode.SMove:
            {
                // 이동되는 플레이어 (나 제외) !!!
                SMove pkt = SMove.Parser.ParseFrom(_recvBuffer.GetBuffer(), _recvBuffer.ReadPos() + offset,
                    header.Size - 4);
                PacketCode pc = new PacketCode();

                pc.code = (ushort) MessageCode.SMove;
                pc.pkt = pkt;
                _readPacketUtils.PushPacket(pc);

                // Debug.LogFormat("{0} 플레이어 이동  {1} / {2}", pkt.Code, pkt.Position.X, pkt.Position.Z);
            }
                break;
            case MessageCode.SChat:
            {
                SChat pkt = SChat.Parser.ParseFrom(_recvBuffer.GetBuffer(), _recvBuffer.ReadPos() + offset,
                    header.Size - 4);
                PacketCode pc = new PacketCode();

                pc.code = (ushort) MessageCode.SChat;
                pc.pkt = pkt;
                _readPacketUtils.PushPacket(pc);
            }
                break;
            case MessageCode.SPlayerdata:
            {
                SPlayerData pkt = SPlayerData.Parser.ParseFrom(_recvBuffer.GetBuffer(), _recvBuffer.ReadPos() + offset,
                    header.Size - 4);
                Debug.LogFormat("내 정보 {0} / {1} / {2} / {3}", pkt.Player.Unit.Name, pkt.Player.Unit.Code,
                    pkt.Player.Unit.Hp, pkt.Player.Unit.Type);

                // 플레이어 code 넣기 플레이어들 구분용
                // Managers.Game.PlayerCode = (int)pkt.Player.Code;
            }
                break;

            case MessageCode.SCloseplayer:
            {
                // 플레이중 종료하는 플레이어 제거 !!!
                SClosePlayer pkt = SClosePlayer.Parser.ParseFrom(_recvBuffer.GetBuffer(),
                    _recvBuffer.ReadPos() + offset, header.Size - 4);
                PacketCode pc = new PacketCode();

                pc.code = (ushort) MessageCode.SCloseplayer;
                pc.pkt = pkt;
                _readPacketUtils.PushPacket(pc);
            }
                break;
            case MessageCode.SUnitstates:
            {
                // 이동되는 플레이어 (나 제외) !!!
                SUnitStates pkt = SUnitStates.Parser.ParseFrom(_recvBuffer.GetBuffer(), _recvBuffer.ReadPos() + offset,
                    header.Size - 4);
                PacketCode pc = new PacketCode();

                pc.code = (ushort) MessageCode.SUnitstates;
                pc.pkt = pkt;
                _readPacketUtils.PushPacket(pc);
            }
                break;
            case MessageCode.SUnitattack:
            {
                SUnitAttack pkt = SUnitAttack.Parser.ParseFrom(_recvBuffer.GetBuffer(), _recvBuffer.ReadPos() + offset,
                    header.Size - 4);
                PacketCode pc = new PacketCode();

                pc.code = (ushort) MessageCode.SUnitattack;
                pc.pkt = pkt;
                _readPacketUtils.PushPacket(pc);
            }
                break;

            case MessageCode.SUnitbuff:
            {
                SUnitBuff pkt = SUnitBuff.Parser.ParseFrom(_recvBuffer.GetBuffer(), _recvBuffer.ReadPos() + offset,
                    header.Size - 4);
                PacketCode pc = new PacketCode();

                pc.code = (ushort) MessageCode.SUnitbuff;
                pc.pkt = pkt;
                _readPacketUtils.PushPacket(pc);
            }
                break;

            case MessageCode.SRoomquest:
            {
                SRoomQuest pkt = SRoomQuest.Parser.ParseFrom(_recvBuffer.GetBuffer(), _recvBuffer.ReadPos() + offset,
                    header.Size - 4);
                PacketCode pc = new PacketCode();

                pc.code = (ushort) MessageCode.SRoomquest;
                pc.pkt = pkt;
                _readPacketUtils.PushPacket(pc);
            }
                break;
        }
    }

    public void Send(IMessage msg, UInt16 id)
    {
        byte[] buffer = PacketHandler.MakePacketHandler(msg, id);
        _socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnSendHandler), null);
    }

    public void OnSendHandler(IAsyncResult result)
    {
        // result.AsyncState; // => 이거 BeginReceive에서 마지막 파라미터에 들은 클래스로 캐스팅해서 확인가능. 그러나 일단제외 (IOCP 개념)

        // Debug.Log("데이터 전송 완료 !!!"); // 굳지 비동기쓸필요가 없어보이네...
    }

    public void Disconnect(bool reuseSocket = true)
    {
        _socket.BeginDisconnect(reuseSocket, OnDisconnectCompletedHandler, null); // 마지막 파라미터 뭔지 모르겠음 연구 필요!!
    }

    public void OnDisconnectCompletedHandler(IAsyncResult result)
    {
        if (_socket.Connected)
        {
            _socket.EndDisconnect(result);
        }

        _socket.Close();
        _isConnecting = false;
        _isConnected = false;
        Debug.Log("서버와 연결 해제 !!!");
    }

    private void _disconnet(bool reuseSocket = true)
    {
        if (_socket.Connected)
            _socket.Close();
    }

    // 일단 일반화 썻지만 될지 모른다.... 근데 지금 안쓰니 문제 x
    public void SetSocketOpt<T>(SocketOptionLevel level, SocketOptionName name, T data)
    {
        _socket.SetSocketOption(level, name, data);
    }

    public bool IsConnectComplated()
    {
        if (_isConnecting && !_isConnected)
            Debug.Log("아직 서버와 연결중입니다 !!!");
        return _isConnecting && _isConnected;
    }

    public bool IsConnecting()
    {
        return _isConnecting;
    }

    public Socket Socket()
    {
        return _socket;
    }

    public PacketUtils SendPacket
    {
        get { return _sendPacketUtils; }
    }

    public PacketUtils ReadPacket
    {
        get { return _readPacketUtils; }
    }

    private RecvBuffer _recvBuffer = new RecvBuffer();

    private Socket _socket;
    private bool _isConnecting;
    private bool _isConnected;
    private PacketUtils _sendPacketUtils = new PacketUtils();
    private PacketUtils _readPacketUtils = new PacketUtils();
}

public class PacketCode
{
    public UInt16 code;
    public IMessage pkt;
}

// 모아서 한꺼번에 보낼때 쓰는용도 사실상 쓸일이 없을수도 있다.
public class PacketUtils
{
    public void PushPacket(PacketCode msg)
    {
        _packets.Enqueue(msg);
    }

    public PacketCode PopPacket()
    {
        return _packets.Dequeue();
    }

    public void ClearPackets(int id)
    {
        _packets.Clear();
    }

    public Queue<PacketCode> GetPackets
    {
        get { return _packets; }
    }

    // id가 해당하는것은 혹시나 게임서버이외의 커넥션이 있을수 있으므로(있지는 않겠지만)
    // id별로(서버 id) 메시지를 따로 두는거라고 생각하면됨
    private Queue<PacketCode> _packets = new Queue<PacketCode>();
}