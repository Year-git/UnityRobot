using System;

public class OStream
{
    protected int _current;
    protected byte[] _buffer;
    protected byte[] _memCache = new byte[8];
    public byte[] Cache { get { return _buffer; } }
    public int Length { get { return _current; } }

    public OStream() : this(1024)
    {

    }

    public OStream(int initialSize)
    {
        _buffer = new byte[initialSize];
    }

    public void Reset()
    {
        _current = 0;
    }

    public void Append(byte[] value)
    {
        Append(value, 0, value.Length);
    }

    public void Append(sbyte value)
    {
        _memCache[0] = (byte)value;
        Append(_memCache, 0, 1);
    }

    public void Append(byte value)
    {
        EnsureBuffer(1);
        _buffer[_current++] = value;
    }

    public void Append(short value)
    {
        _memCache[0] = (byte)value;
        _memCache[1] = (byte)(value >> 8);
        Append(_memCache, 0, 2);
    }

    public void Append(ushort value)
    {
        _memCache[0] = (byte)value;
        _memCache[1] = (byte)(value >> 8);
        Append(_memCache, 0, 2);
    }

    public void Append(int value)
    {
        _memCache[0] = (byte)value;
        _memCache[1] = (byte)(value >> 8);
        _memCache[2] = (byte)(value >> 16);
        _memCache[3] = (byte)(value >> 24);
        Append(_memCache, 0, 4);
    }

    public void Append(uint value)
    {
        _memCache[0] = (byte)value;
        _memCache[1] = (byte)(value >> 8);
        _memCache[2] = (byte)(value >> 16);
        _memCache[3] = (byte)(value >> 24);
        Append(_memCache, 0, 4);
    }

    public void Append(long value)
    {
        _memCache[0] = (byte)value;
        _memCache[1] = (byte)(value >> 8);
        _memCache[2] = (byte)(value >> 16);
        _memCache[3] = (byte)(value >> 24);
        _memCache[4] = (byte)(value >> 32);
        _memCache[5] = (byte)(value >> 40);
        _memCache[6] = (byte)(value >> 48);
        _memCache[7] = (byte)(value >> 56);
        Append(_memCache, 0, 8);
    }

    public void Append(ulong value)
    {
        _memCache[0] = (byte)value;
        _memCache[1] = (byte)(value >> 8);
        _memCache[2] = (byte)(value >> 16);
        _memCache[3] = (byte)(value >> 24);
        _memCache[4] = (byte)(value >> 32);
        _memCache[5] = (byte)(value >> 40);
        _memCache[6] = (byte)(value >> 48);
        _memCache[7] = (byte)(value >> 56);
        Append(_memCache, 0, 8);
    }

    public void Append(float value)
    {
        Append((int)(value * 100));
    }

    public void Append(double value)
    {
        Append((long)(value * 100));
    }

    public void Append(string value)
    {
        value = value.Trim();
        int size = value.Length * 2;
        if (value.Length < 255)
        {
            Append((byte)value.Length);
        }
        else
        {
            Append((byte)255);
            Append((uint)value.Length);
        }
        EnsureBuffer(size);
        for (int i = 0; i < value.Length; i++)
        {
            char ch = value[i];
            _buffer[_current + 2 * i] = (byte)ch;
            _buffer[_current + 2 * i + 1] = (byte)(ch >> 8);
        }
        _current += size;
    }

    public void Append(byte[] value, int offset, int count)
    {
        EnsureBuffer(count);
        Buffer.BlockCopy(value, offset, _buffer, _current, count);
        _current += count;
    }

    public void Append(OStream stream)
    {
        int len = stream.Length;
        Append(len);
        Append(stream._buffer, 0, stream._current);
    }

    protected void EnsureBuffer(int count)
    {
        if (count > (_buffer.Length - _current))
        {
            byte[] dst = new byte[((_buffer.Length * 2) > (_buffer.Length + count)) ? (_buffer.Length * 2) : (_buffer.Length + count)];
            Buffer.BlockCopy(_buffer, 0, dst, 0, _current);
            _buffer = dst;
        }
    }

    public void _SetValue(int offset, byte[] value, int cnt)
    {
        for (int i = 0; i < cnt; i++)
        {
            _buffer[offset + i] = value[i];
        }
    }
}