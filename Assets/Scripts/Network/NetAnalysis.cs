using LuaInterface;
using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Framework;

/// <summary>
/// 网络消息编解码
/// </summary>
public class NetAnalysis
{
    private const int MAX_BYTE_LEN = 4096;
    private static int _sendRefNum = 0;
    private static int _useNum = 0;
    private static string _strKey = "";
    private static int _tabLen = 0;

    /// <summary>
    /// 存接收到的数据
    /// </summary>
    public static List<object> ReceiveData { get; private set; } = new List<object>();

    /// <summary>
    /// 获取发送数据（经过encode加密处理后的）
    /// </summary>
    /// <returns></returns>
    public static byte[] SendData { get; private set; }

    /// <summary>
    /// 获取发送数据在发送字节流数组中占用的大小
    /// </summary>
    /// <returns></returns>
    public static int SendDataLength { get; private set; } = 0;

    /// <summary>
    /// Lua编码
    /// </summary>
    /// <returns></returns>
    public static void LuaEncode()
    {
        _sendRefNum = 0;
        SendDataLength = 0;
        SendData = new byte[MAX_BYTE_LEN];
        LuaState luaState = LuaClient.GetMainState();

        do
        {
            luaState.LuaGetTop();
            int nCurIndex = 1;
            luaState.LuaCreateTable(0, 16);
            int nRefIndex = luaState.LuaGetTop();
            while (LuaEncodeAnalysis(luaState, nCurIndex, nRefIndex, SendData))
            {
                if (++nCurIndex == nRefIndex)
                {
                    break;
                }
            }
        } while (false);
        luaState.LuaPushInteger(SendDataLength);
    }
    private static bool LuaEncodeAnalysis(LuaState luaState, int nCurIndex, int nRefIndex, byte[] data)
    {
        LuaTypes t = luaState.LuaType(nCurIndex);
        if (t == LuaTypes.LUA_TNIL)
        {
            if (!EncodeQuery(1)) { return false; };
            data[SendDataLength] = 0;
            EncodeQuery(1, true);
        }
        else if (t == LuaTypes.LUA_TBOOLEAN)
        {
            if (!EncodeQuery(1)) { return false; };
            data[SendDataLength] = (byte)(luaState.LuaToBoolean(nCurIndex) ? 1 : 2);
            EncodeQuery(1, true);
        }
        else if (t == LuaTypes.LUA_TNUMBER)
        {
            double V = luaState.LuaToNumber(nCurIndex);
            long v = (long)V;
            if (v != V)
            {
                if (!EncodeQuery(9)) { return false; };
                data[SendDataLength] = 4;
                //内存拷贝
                byte[] b = BitConverter.GetBytes(V);
                data[SendDataLength + 1] = b[0];
                data[SendDataLength + 2] = b[1];
                data[SendDataLength + 3] = b[2];
                data[SendDataLength + 4] = b[3];
                data[SendDataLength + 5] = b[4];
                data[SendDataLength + 6] = b[5];
                data[SendDataLength + 7] = b[6];
                data[SendDataLength + 8] = b[7];
                EncodeQuery(9, true);
            }
            else if (v == v << 58 >> 58)
            {
                if (!EncodeQuery(1)) { return false; };
                data[SendDataLength] = (byte)(64 | (v & 0x3F));
                EncodeQuery(1, true);
            }
            else if (v == (short)v)
            {
                if (!EncodeQuery(3)) { return false; };
                data[SendDataLength] = 5;
                byte[] b = BitConverter.GetBytes((short)v);
                data[SendDataLength + 1] = b[0];
                data[SendDataLength + 2] = b[1];
                EncodeQuery(3, true);
            }
            else if (v == (int)v)
            {
                if (!EncodeQuery(5)) { return false; };
                data[SendDataLength] = 6;
                byte[] b = BitConverter.GetBytes((int)v);
                data[SendDataLength + 1] = b[0];
                data[SendDataLength + 2] = b[1];
                data[SendDataLength + 3] = b[2];
                data[SendDataLength + 4] = b[3];
                EncodeQuery(5, true);
            }
            else
            {
                if (!EncodeQuery(9)) { return false; };
                data[SendDataLength] = 7;
                byte[] b = BitConverter.GetBytes(v);
                data[SendDataLength + 1] = b[0];
                data[SendDataLength + 2] = b[1];
                data[SendDataLength + 3] = b[2];
                data[SendDataLength + 4] = b[3];
                data[SendDataLength + 5] = b[4];
                data[SendDataLength + 6] = b[5];
                data[SendDataLength + 7] = b[6];
                data[SendDataLength + 8] = b[7];
                EncodeQuery(9, true);
            }
        }
        else if (t == LuaTypes.LUA_TSTRING)
        {
            int nStrLen;
            IntPtr pStr = luaState.LuaToLString(nCurIndex, out nStrLen);
            byte[] b1 = Encoding.Default.GetBytes(Marshal.PtrToStringAnsi(pStr));
            nStrLen = b1.Length;

            int res = LuaEncodeRefAnalysis(luaState, nCurIndex, nRefIndex, data);
            if (res == 2)
            {
                if (nStrLen < 64)
                {
                    if (!EncodeQuery(1)) { return false; };
                    data[SendDataLength] = (byte)(-128 | nStrLen);
                    EncodeQuery(1, true);
                }
                else
                {
                    if (!EncodeQuery(3)) { return false; };
                    data[SendDataLength] = 9;
                    byte[] b = BitConverter.GetBytes((short)nStrLen);
                    data[SendDataLength + 1] = b[0];
                    data[SendDataLength + 2] = b[1];
                    EncodeQuery(3, true);
                }

                if (!EncodeQuery(nStrLen)) { return false; };
                for (int i = 0; i < nStrLen; i++)
                {
                    data[SendDataLength + i] = b1[i];
                }
                EncodeQuery(nStrLen, true);
            }
            else if (res == 0)
            {
                return false;
            }
            else {  /*res == 1 */  }
        }
        else if (t == LuaTypes.LUA_TTABLE)
        {
            int res = LuaEncodeRefAnalysis(luaState, nCurIndex, nRefIndex, data);
            if (res == 2)
            {
                int nKeyIndex = luaState.LuaGetTop() + 1;
                int nValueIndex = nKeyIndex + 1;
                if (!EncodeQuery(2)) { return false; };
                data[SendDataLength] = 16;
                data[SendDataLength + 1] = 0;
                EncodeQuery(2, true);
                luaState.LuaPushNil();
                while (luaState.LuaNext(nCurIndex))
                {
                    if (!LuaEncodeAnalysis(luaState, nKeyIndex, nRefIndex, data))
                    {
                        return false;
                    }
                    if (!LuaEncodeAnalysis(luaState, nValueIndex, nRefIndex, data))
                    {
                        return false;
                    }
                    luaState.LuaPop(1);
                }
                if (!EncodeQuery(1)) { return false; };
                data[SendDataLength] = 0;
                EncodeQuery(1, true);
            }
            else if (res == 0)
            {
                return false;
            }
            else /*res == 1*/ { }
        }
        else
        {
            return false;
        }
        return true;
    }
    private static int LuaEncodeRefAnalysis(LuaState luaState, int nCurIndex, int nRefIndex, byte[] data)
    {
        luaState.LuaPushValue(nCurIndex);
        luaState.LuaRawGet(nRefIndex);
        int nRefID = (int)(luaState.LuaToNumber(-1));
        luaState.LuaPop(1);
        if (nRefID == 0)
        {
            luaState.LuaPushValue(nCurIndex);
            luaState.LuaPushInteger(++_sendRefNum);
            luaState.LuaRawSet(nRefIndex);
            return 2;
        }
        else if (nRefID < 64)
        {
            if (!EncodeQuery(1)) { return 0; };
            data[SendDataLength] = (byte)(-64 | nRefID);
            EncodeQuery(1, true);
        }
        else if (nRefID == (short)nRefID)
        {
            if (!EncodeQuery(3)) { return 0; };
            data[SendDataLength] = 13;
            byte[] b = BitConverter.GetBytes((short)nRefID);
            data[SendDataLength + 1] = b[0];
            data[SendDataLength + 2] = b[1];
            EncodeQuery(3, true);
        }
        else
        {
            if (!EncodeQuery(5)) { return 0; };
            data[SendDataLength] = 14;
            byte[] b = BitConverter.GetBytes(nRefID);
            data[SendDataLength + 1] = b[0];
            data[SendDataLength + 2] = b[1];
            data[SendDataLength + 3] = b[2];
            data[SendDataLength + 4] = b[3];
            EncodeQuery(5, true);
        }
        return 1;
    }
    private static bool EncodeQuery(int num, bool bol = false)
    {
        if (!bol)
        {
            if (SendDataLength + num > MAX_BYTE_LEN)
            {
                return false;
            }
            return true;
        }
        else
        {
            SendDataLength += num;
        }
        return true;
    }

    /// <summary>
    /// CSharp编码
    /// </summary>
    public static void CSharpEncode(params object[] args)
    {
        object[] data;
        string msgFunc = args[0].ToString();
        if (msgFunc.Contains("G_"))
        {
            data = new object[args.Length + 1];
            data[0] = "K_Ts";
            for (int i = 1; i < data.Length; i++)
            {
                data[i] = args[i - 1];
            }
        }
        else
        {
            data = args;
        }

        SendDataLength = 0;
        SendData = new byte[MAX_BYTE_LEN];
        for (int i = 0; i < data.Length; i++)
        {
            CSharpEncodeAnalysis(data[i], SendData);
        }
    }
    private static bool CSharpEncodeAnalysis(object param, byte[] data)
    {
        Type type = param.GetType();
        if (param == null)
        {
            if (!EncodeQuery(1)) { return false; };
            data[SendDataLength] = 0;
            EncodeQuery(1, true);
        }
        else if (type == typeof(bool))
        {
            if (!EncodeQuery(1)) { return false; };
            data[SendDataLength] = (byte)((bool)param ? 1 : 2);
            EncodeQuery(1, true);
        }
        else if (type == typeof(int))
        {
            if (!EncodeQuery(5)) { return false; };
            data[SendDataLength] = 6;
            byte[] b = BitConverter.GetBytes((int)param);
            data[SendDataLength + 1] = b[0];
            data[SendDataLength + 2] = b[1];
            data[SendDataLength + 3] = b[2];
            data[SendDataLength + 4] = b[3];
            EncodeQuery(5, true);
        }
        else if (type == typeof(short))
        {
            if (!EncodeQuery(3)) { return false; };
            data[SendDataLength] = 5;
            byte[] b = BitConverter.GetBytes((short)param);
            data[SendDataLength + 1] = b[0];
            data[SendDataLength + 2] = b[1];
            EncodeQuery(3, true);
        }
        else if (type == typeof(long))
        {
            if (!EncodeQuery(9)) { return false; };
            data[SendDataLength] = 7;
            byte[] b = BitConverter.GetBytes((long)param);
            data[SendDataLength + 1] = b[0];
            data[SendDataLength + 2] = b[1];
            data[SendDataLength + 3] = b[2];
            data[SendDataLength + 4] = b[3];
            data[SendDataLength + 5] = b[4];
            data[SendDataLength + 6] = b[5];
            data[SendDataLength + 7] = b[6];
            data[SendDataLength + 8] = b[7];
            EncodeQuery(9, true);
        }
        else if (type == typeof(double))
        {
            if (!EncodeQuery(9)) { return false; };
            data[SendDataLength] = 4;
            byte[] b = BitConverter.GetBytes((double)param);
            data[SendDataLength + 1] = b[0];
            data[SendDataLength + 2] = b[1];
            data[SendDataLength + 3] = b[2];
            data[SendDataLength + 4] = b[3];
            data[SendDataLength + 5] = b[4];
            data[SendDataLength + 6] = b[5];
            data[SendDataLength + 7] = b[6];
            data[SendDataLength + 8] = b[7];
            EncodeQuery(9, true);
        }
        else if (type == typeof(string))
        {
            byte[] bytes = Encoding.Default.GetBytes(param.ToString());
            int nStrLen = bytes.Length;
            if (nStrLen < 64)
            {
                if (!EncodeQuery(1)) { return false; };
                data[SendDataLength] = (byte)(-128 | nStrLen);
                EncodeQuery(1, true);
            }
            else
            {
                if (!EncodeQuery(3)) { return false; };
                data[SendDataLength] = 9;
                byte[] b = BitConverter.GetBytes((short)nStrLen);
                data[SendDataLength + 1] = b[0];
                data[SendDataLength + 2] = b[1];
                EncodeQuery(3, true);
            }
            if (!EncodeQuery(nStrLen)) { return false; };
            for (int i = 0; i < nStrLen; i++)
            {
                data[SendDataLength + i] = bytes[i];
            }
            EncodeQuery(nStrLen, true);
        }
        else
        {
            Debug.LogError("发现不存在的编码类型：" + type.ToString());
        }
        return true;
    }

    /// <summary>
    /// 解码
    /// </summary>
    /// <param name="length"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static void Decode(int length, byte[] data)
    {
        ReceiveData.Clear();
        do
        {
            int nMax = length;
            _useNum = 0;
            while (DecodeAnalysis(data, nMax))
            {
                if (_useNum == nMax)
                {
                    break;
                }
            }
        } while (false);
    }
    private static bool DecodeAnalysis(byte[] data, int nMax)
    {
        if (!DecodeQuery(1, nMax)) { return false; }
        byte flag = data[_useNum];
        DecodeQuery(1, nMax, true);

        if (flag == 0)
        {
            ReceiveData.Add(null);
        }
        else if (flag == 1)
        {
            ReceiveData.Add(true);
        }
        else if (flag == 2)
        {
            ReceiveData.Add(false);
        }
        else
        {
            byte flag1 = (byte)(flag & 0xC0);
            byte flag2 = (byte)(flag & 0xFC);

            if ((flag1 == 64) || (flag2 == 4))
            {
                if (flag1 == 64)
                {
                    ReceiveData.Add(flag << 26 >> 26);
                }
                else if (flag == 5)
                {
                    if (!DecodeQuery(2, nMax)) { return false; };
                    ReceiveData.Add(Converter.ByteArray2Short(data, _useNum));
                    DecodeQuery(2, nMax, true);
                }
                else if (flag == 6)
                {
                    if (!DecodeQuery(4, nMax)) { return false; };
                    ReceiveData.Add(Converter.ByteArray2Int(data, _useNum));
                    DecodeQuery(4, nMax, true);
                }
                else if (flag == 7)
                {
                    if (!DecodeQuery(8, nMax)) { return false; };
                    ReceiveData.Add(Converter.ByteArray2Double(data, _useNum));
                    DecodeQuery(8, nMax, true);
                }
                else if (flag == 4)
                {
                    if (!DecodeQuery(8, nMax)) { return false; };
                    ReceiveData.Add(Converter.ByteArray2Double(data, _useNum));
                    DecodeQuery(8, nMax, true);
                }
                else { return false; }
            }
            else if ((flag1 == 128) || (flag2 == 8))
            {
                int strLen = 0;
                if (flag1 == 128)
                {
                    strLen = flag & 0x3F;
                }
                else if (flag == 9)
                {
                    if (!DecodeQuery(2, nMax)) { return false; };
                    strLen = Converter.ByteArray2Short(data, _useNum);
                    DecodeQuery(2, nMax, true);
                }
                else
                {
                    return false;
                }
                if (!DecodeQuery(strLen, nMax)) { return false; };
                byte[] str = new byte[strLen];
                for (int i = 0; i < strLen; i++)
                {
                    str[i] = data[_useNum + i];
                }
                DecodeQuery(strLen, nMax, true);
                ReceiveData.Add(Converter.ByteArray2String(str));
            }else if (flag2 == 16)
            {
                int counts = 0;
                if (flag == 16)
                {
                    if (!DecodeQuery(1, nMax)){
                        return false;
                    }
                    counts = (int) Converter.ByteArray2Char(data, _useNum);
                    DecodeQuery(1, nMax, true);

                    //创建对象
                    JObject o = new JObject();
                    while (true) {
                        // key
                        if (!DecodeTable(ref o, data, nMax, true)) { return false; };
                        // 如果解析LuaTable时，当KEY值为null时，说明这个Lua-table已经解析完毕。退出
                        if (_strKey == null) {
                            break;
                        }
                        // value 
                        if (!DecodeTable(ref o, data, nMax, false)) { return false; };
                    }

                    JArray a = Common.JObject2JArray(o);
                    ReceiveData.Add(a);
                }
                // else if(flag == 17)
                // {
                //     if (!DecodeQuery(2, nMax)){
                //         return false;
                //     }
                //     counts = Converter.ByteArray2Short(data, _useNum);
                //     DecodeQuery(2, nMax, true);
                // }
                // else if(flag == 18)
                // {
                //     if (!DecodeQuery(4, nMax)){
                //         return false;
                //     }
                //     counts = Converter.ByteArray2Int(data, _useNum);
                //     DecodeQuery(4, nMax, true);
                // }
                else
                {
                    return false;
                }
            }else
            {
                return false;
            }
        }
        return true;
    }


    private static bool DecodeTable(ref JObject obj, byte[] data, int nMax, bool bIsKey){
         if (!DecodeQuery(1, nMax)) { return false; }
        byte flag = data[_useNum];
        DecodeQuery(1, nMax, true);

        if (flag == 0)
        {
            if (bIsKey){
                _strKey = null;
            }else{
                // obj.Add(null);
                obj.Add(_strKey, null);
            }
        }
        else if (flag == 1)
        {
            if (bIsKey){
                _strKey = "true";
            }else{
                // obj.Add(true);
                obj.Add(_strKey, true);
            }
        }
        else if (flag == 2)
        {
            if (bIsKey){
                _strKey = "false";
            }else{
                // obj.Add(false);
                obj.Add(_strKey, false);
            }
        }
        else
        {
            byte flag1 = (byte)(flag & 0xC0);
            byte flag2 = (byte)(flag & 0xFC);

            if ((flag1 == 64) || (flag2 == 4))
            {
                int nNumber = 0;
                if (flag1 == 64)
                {
                    nNumber = flag << 26 >> 26;
                }
                else if (flag == 5)
                {
                    if (!DecodeQuery(2, nMax)) { return false; };
                    nNumber = Converter.ByteArray2Short(data, _useNum);
                    DecodeQuery(2, nMax, true);
                }
                else if (flag == 6)
                {
                    if (!DecodeQuery(4, nMax)) { return false; };
                    nNumber = Converter.ByteArray2Int(data, _useNum);
                    DecodeQuery(4, nMax, true);
                }
                else if (flag == 7)
                {
                    if (!DecodeQuery(8, nMax)) { return false; };
                    nNumber = (int) Converter.ByteArray2Double(data, _useNum);
                    DecodeQuery(8, nMax, true);
                }
                else if (flag == 4)
                {
                    if (!DecodeQuery(8, nMax)) { return false; };
                    nNumber = (int) Converter.ByteArray2Double(data, _useNum);
                    DecodeQuery(8, nMax, true);
                }
                else { return false; }

                if (bIsKey){
                    _strKey = nNumber.ToString();
                }else{
                    // obj.Add(nNumber);
                    obj.Add(_strKey, nNumber);
                }
            }
            else if ((flag1 == 128) || (flag2 == 8))
            {
                int strLen = 0;
                if (flag1 == 128)
                {
                    strLen = flag & 0x3F;
                }
                else if (flag == 9)
                {
                    if (!DecodeQuery(2, nMax)) { return false; };
                    strLen = Converter.ByteArray2Short(data, _useNum);
                    DecodeQuery(2, nMax, true);
                }
                else
                {
                    return false;
                }
                if (!DecodeQuery(strLen, nMax)) { return false; };
                byte[] str = new byte[strLen];
                for (int i = 0; i < strLen; i++)
                {
                    str[i] = data[_useNum + i];
                }
                DecodeQuery(strLen, nMax, true);
                string strRet = Converter.ByteArray2String(str);
                if (bIsKey){
                    _strKey = strRet;
                }else{
                    // obj.Add(strRet);
                    obj.Add(_strKey, strRet);
                }
            }else if (flag2 == 16)
            {
                if (!DecodeQuery(1, nMax)) { return false; };
                DecodeQuery(1, nMax, true);
                //创建对象
                JObject o = new JObject();
                obj.Add(_strKey, o);

                while (true) {
                    // key
                    if (!DecodeTable(ref o, data, nMax, true)) { return false; };
                    if (_strKey == null) {
                        break;
                    };
                    // value
                    if (!DecodeTable(ref o, data, nMax, false)) { return false; };
                }

            }else
            {
                return false;
            }
        }
        return true;   
    }


    private static bool DecodeQuery(int num, int nMax, bool bol = false)
    {
        if (!bol)
        {
            if (_useNum + num > nMax)
            {
                return false;
            }
            return true;
        }
        else
        {
            _useNum += num;
        }
        return true;
    }
}