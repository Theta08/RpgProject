using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Google.Protobuf;
using UnityEngine;

public class PacketHandler
{
    public static bool CheckPacketHeader(PacketHeader header, int offset, int len)
    {
        if (header == null)
            return false;

        int dataSize = len - offset;

        if (dataSize < 4)
            return false;

        if (dataSize < header.Size)
            return false;

        return true;
    }
    
    public static PacketHeader ParsePacketHandler(byte[] buffer, int offset)
    {
        // 일단 헤더는 2+2 = 4 바이트라 하드코딩 둔다.
        UInt16 id = BitConverter.ToUInt16(buffer, offset);
        UInt16 size = BitConverter.ToUInt16(buffer, offset + 2);
        PacketHeader header = new PacketHeader(id, size);

        return header;
    }

    public static byte[] MakePacketHandler(IMessage pkt, UInt16 id)
    {
        UInt16 pktSize = (UInt16) pkt.CalculateSize();

        byte[] headerBuffer = MakeHeaderPacketHandler(id, pktSize);
        byte[] pktBuffer = pkt.ToByteArray();

        byte[] sendBuffer = new byte[headerBuffer.Length + pktBuffer.Length];
        headerBuffer.CopyTo(sendBuffer, 0);
        pktBuffer.CopyTo(sendBuffer, headerBuffer.Length);

        return sendBuffer;
    }

    public static byte[] MakeHeaderPacketHandler(UInt16 pktId, UInt16 pktSize)
    {
        // 일단 헤더는 2+2 = 4 바이트라 하드코딩 둔다.
        UInt16 packetSize = (UInt16) (pktSize + PacketHeader.Len);
        
        PacketHeader header = new PacketHeader(pktId, packetSize);

        byte[] headerArr = new byte[PacketHeader.Len];
        BitConverter.GetBytes(header.Id).CopyTo(headerArr, 0);
        BitConverter.GetBytes(header.Size).CopyTo(headerArr, 2);
        
        // Debug.LogFormat("header size : {0}", headerArr.Length);
        return headerArr;
    }

    public static byte[] ObjectToByteArr(object obj)
    {
        if (obj == null)
            return null;

        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }
}

[Serializable]
public class PacketHeader
{
    public PacketHeader(UInt16 id, UInt16 size)
    {
        Id = id;
        Size = size;
    }
    public UInt16 Id { get; set; }
    public UInt16 Size { get; set; }

    public static readonly UInt16 Len = 4;
}