using System;
using System.Collections.Generic;

public class RecvBuffer
{
    enum SIZE
    {
        BUFFER_COUNT = 10
    }

    public RecvBuffer(int bufferSize = 4096)
    {
        _capacity = bufferSize * (int) SIZE.BUFFER_COUNT;
        _buffer = new byte[_capacity];
        _bufferSize = bufferSize;
    }

    public void Clean()
    {
        int dataSize = DataSize();
        if (dataSize == 0)
        {
            // 딱 마침 읽기+쓰기 커서가 동일한 위치라면, 둘 다 리셋.
            _readPos = _writePos = 0;
        }
        else
        {
            // 여유 공간이 버퍼 2개 크기 미만이면, 데이터를 앞으로 땅긴다.
            if (FreeSize() < _bufferSize * 2)
            {
                byte[] temp = new byte[dataSize];
                Buffer.BlockCopy(_buffer, _readPos, temp, 0, dataSize);;
                for (int i = 0; i < dataSize; i++)
                {
                    _buffer[i] = temp[i];
                }
                _readPos = 0;
                _writePos = dataSize;
            }
        }
    }

    public bool OnRead(int readSize)
    {
        if (readSize > DataSize())
            return false;
        _readPos += readSize;
        return true;
    }

    public bool OnWrite(int writeSize)
    {
        if (writeSize > FreeSize())
            return false;
        _writePos += writeSize;
        return true;
    }

    public int ReadPos()
    {
        return _readPos;
    }

    public int WritePos()
    {
        return _writePos;
    }

    public int DataSize()
    {
        return _writePos - _readPos;
    }

    public int FreeSize()
    {
        return _capacity - _writePos;
    }

    public byte[] GetBuffer()
    {
        return _buffer;
    }
    
    private Byte[] _buffer;
    private int _bufferSize;
    private int _capacity = 0;
    private int _readPos = 0;
    private int _writePos = 0;
}