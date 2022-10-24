using System;

/// <summary>
/// 转换器
/// </summary>
public class Converter
{
    public static int GetBigEndian(int value)
    {
        if (BitConverter.IsLittleEndian)
        {
            return SwapByteOrder(value);
        }
        else
        {
            return value;
        }
    }

    public static ushort GetBigEndian(ushort value)
    {
        if (BitConverter.IsLittleEndian)
        {
            return SwapByteOrder(value);
        }
        else
        {
            return value;
        }
    }

    public static uint GetBigEndian(uint value)
    {
        if (BitConverter.IsLittleEndian)
        {
            return SwapByteOrder(value);
        }
        else
        {
            return value;
        }
    }

    public static long GetBigEndian(long value)
    {
        if (BitConverter.IsLittleEndian)
        {
            return SwapByteOrder(value);
        }
        else
        {
            return value;
        }
    }

    public static double GetBigEndian(double value)
    {
        if (BitConverter.IsLittleEndian)
        {
            return SwapByteOrder(value);
        }
        else
        {
            return value;
        }
    }

    public static float GetBigEndian(float value)
    {
        if (BitConverter.IsLittleEndian)
        {
            return SwapByteOrder((int)value);

        }
        else
        {
            return value;
        }
    }

    public static int GetLittleEndian(int value)
    {
        if (BitConverter.IsLittleEndian)
        {
            return value;
        }
        else
        {
            return SwapByteOrder(value);
        }
    }

    public static uint GetLittleEndian(uint value)
    {
        if (BitConverter.IsLittleEndian)
        {
            return value;
        }
        else
        {
            return SwapByteOrder(value);
        }
    }

    public static ushort GetLittleEndian(ushort value)
    {
        if (BitConverter.IsLittleEndian)
        {
            return value;
        }
        else
        {
            return SwapByteOrder(value);
        }
    }

    public static double GetLittleEndian(double value)
    {
        if (BitConverter.IsLittleEndian)
        {
            return value;
        }
        else
        {
            return SwapByteOrder(value);
        }
    }

    private static int SwapByteOrder(int value)
    {
        uint uvalue = (uint)value;
        uint swap = ((0x000000FF) & (uvalue >> 24)
            | (0x0000FF00) & (uvalue >> 8)
            | (0x00FF0000) & (uvalue << 8)
            | (0xFF000000) & (uvalue << 24));
        return (int)swap;
    }

    private static long SwapByteOrder(long value)
    {
        ulong uvalue = (ulong)value;
        ulong swap = ((0x00000000000000FF) & (uvalue >> 56)
        | (0x000000000000FF00) & (uvalue >> 40)
        | (0x0000000000FF0000) & (uvalue >> 24)
        | (0x00000000FF000000) & (uvalue >> 8)
        | (0x000000FF00000000) & (uvalue << 8)
        | (0x0000FF0000000000) & (uvalue << 24)
        | (0x00FF000000000000) & (uvalue << 40)
        | (0xFF00000000000000) & (uvalue << 56));
        return (long)swap;
    }

    private static ushort SwapByteOrder(ushort value)
    {
        return (ushort)((0x00FF & (value >> 8))
            | (0xFF00 & (value << 8)));
    }

    private static uint SwapByteOrder(uint value)
    {
        uint swap = ((0x000000FF) & (value >> 24)
            | (0x0000FF00) & (value >> 8)
            | (0x00FF0000) & (value << 8)
            | (0xFF000000) & (value << 24));
        return swap;
    }

    private static double SwapByteOrder(double value)
    {
        byte[] buffer = BitConverter.GetBytes(value);
        Array.Reverse(buffer, 0, buffer.Length);
        return BitConverter.ToDouble(buffer, 0);
    }

    /// <summary>
    /// Int转byte[]
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static byte[] Int2ByteArray(int num)
    {
        return BitConverter.GetBytes(num);
    }

    /// <summary>
    /// short 转 byte[]
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public static byte[] Short2ByteArray(short val)
    {
        return BitConverter.GetBytes(val);
    }

    /// <summary>
    /// float 转 byte[]
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    public static byte[] Float2ByteArray(float f)
    {
        return BitConverter.GetBytes(f);
    }

    /// <summary>
    /// string 转 byte[]
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static byte[] String2ByteArray(string s)
    {
        return System.Text.Encoding.ASCII.GetBytes(s);
    }

    /// <summary>
    /// double 转 byte[]
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public static byte[] Double2ByteArray(double val)
    {
        return BitConverter.GetBytes(val);
    }

    /// <summary>
    /// char 转 byte[]
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public static byte[] Char2ByteArray(char val)
    {
        return BitConverter.GetBytes(val);
    }

    /// <summary>
    /// boolean 转 byte[]
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public static byte[] Bool2ByteArray(bool val)
    {
        return BitConverter.GetBytes(val);
    }

    /// <summary>
    /// byte[] 转 bool
    /// </summary>
    /// <param name="bt"></param>
    /// <param name="startIndex"></param>
    /// <returns></returns>
    public static bool ByteArray2Bool(byte[] bt, int startIndex)
    {
        return BitConverter.ToBoolean(bt, startIndex);
    }

    /// <summary>
    /// byte[] 转 string
    /// </summary>
    /// <param name="bt"></param>
    /// <returns></returns>
    public static string ByteArray2String(byte[] bt)
    {
        return System.Text.Encoding.Default.GetString(bt);
    }

    /// <summary>
    /// byte[] 转 double
    /// </summary>
    /// <param name="bt"></param>
    /// <returns></returns>
    public static double ByteArray2Double(byte[] bt, int startIndex)
    {
        return BitConverter.ToDouble(bt, startIndex);
    }

    /// <summary>
    /// byte[] 转 char
    /// </summary>
    /// <param name="bt"></param>
    /// <returns></returns>
    public static double ByteArray2Char(byte[] bt, int startIndex)
    {
        return BitConverter.ToChar(bt, startIndex);
    }

    /// <summary>
    /// byte[] 转 short
    /// </summary>
    /// <param name="bt"></param>
    /// <param name="startIndex"></param>
    /// <returns></returns>
    public static short ByteArray2Short(byte[] bt, int startIndex)
    {
        return BitConverter.ToInt16(bt, startIndex);
    }

    /// <summary>
    /// byte[] 转 float
    /// </summary>
    /// <param name="bt"></param>
    /// <param name="starIndex"></param>
    /// <returns></returns>
    public static float ByteArray2Float(byte[] bt, int starIndex)
    {
        return BitConverter.ToSingle(bt, starIndex);
    }

    /// <summary>
    /// byte[] 转int
    /// </summary>
    /// <param name="bt"></param>
    /// <param name="startIndex"></param>
    /// <returns></returns>
    public static int ByteArray2Int(byte[] bt, int startIndex)
    {
        return BitConverter.ToInt32(bt, startIndex);
    }
}