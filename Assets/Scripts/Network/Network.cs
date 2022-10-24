using System;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

public static class Network
{
    private static Socket _socket;
    private static readonly int _timeOut = 5000;
    private static LuaFunction _successCallBack;
    private static LuaFunction _failedCallBack;
    private static byte[] _head;
    private static short _dataLen;
    private static byte _sequence = 0;
    private static Thread _threadReceive;
    private static Thread _threadSend;
    private static AutoResetEvent _autoEvent = new AutoResetEvent(false);
    private static Queue _packetSendQueue = Queue.Synchronized(new Queue());
    private static Queue _packetReceiveQueue = Queue.Synchronized(new Queue());
    private static LuaState _luaState;
    private static LuaFunction _luaReceive;

    public static bool IsConnect { get { return _socket != null && _socket.Connected; } }

    public static void Start()
    {
        _luaState = LuaClient.GetMainState();
        _luaReceive = _luaState.GetFunction("Receive");
    }

    public static void Update()
    {
        lock (_packetReceiveQueue)
        {
            while (_packetReceiveQueue.Count > 0)
            {
                if (!(_packetReceiveQueue.Dequeue() is NetPacket netPacket))
                {
                    Debug.Log("网络异常: netPacket为空包!");
                    continue;
                }

                if (netPacket.Length == 0)
                {
                    Debug.Log("网络异常: 数据长度为0!");
                    continue;
                }

                if (netPacket.Data == null)
                {
                    Debug.Log("网络异常: 数据为null!");
                    continue;
                }
                DispatchServerMessage(netPacket.Length, netPacket.Data);
            }
        }
    }

    public static void OnDestroy()
    {
        Closed();
    }

    /// <summary>
    /// 连接服务器
    /// </summary>
    /// <param name="serverIp"></param>
    /// <param name="serverPort"></param>
    /// <param name="connectCallback"></param>
    /// <param name="connectFailedCallback"></param>
    public static void Connect(string serverIp, int serverPort, LuaFunction connectCallback, LuaFunction connectFailedCallback)
    {
        _successCallBack = connectCallback;
        _failedCallBack = connectFailedCallback;
        //采用TCP方式连接
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //服务器IP地址
        IPAddress address = IPAddress.Parse(serverIp);
        //服务器端口
        IPEndPoint endpoint = new IPEndPoint(address, serverPort);
        //异步连接
        IAsyncResult result = _socket.BeginConnect(endpoint, null, _socket);
        //超时监测
        bool success = result.AsyncWaitHandle.WaitOne(_timeOut, true);
        if (!success)
        {
            //关闭
            Closed();
            //回调lua
            CallLuaFunction(_failedCallBack);
        }
        else
        {
            //与socket建立连接成功，开启线程接收服务端数据
            _threadReceive = new Thread(new ThreadStart(ReceiveThread))
            {
                IsBackground = true
            };
            _threadReceive.Start();
            // 与socket建立连接成功，开启线程发送数据到服务端
            _threadSend = new Thread(new ThreadStart(SendThread))
            {
                IsBackground = true
            };
            _threadSend.Start();

            //打通网关
            _sequence = 0;
            byte[] b = Converter.String2ByteArray("tgw_l7_forward\r\nHost:" + serverIp + ":" + serverPort + "\r\n\r\n\0");
            _socket.Send(b);

            //回调lua
            CallLuaFunction(_successCallBack);
        }
    }

    /// <summary>
    /// 接收消息线程
    /// </summary>
    private static void ReceiveThread()
    {
        while (IsConnect)
        {
            if (!IsConnect)
            {
                //与服务器断开连接跳出循环
                Debug.Log("网络连接断开！");
                Closed();
                break;
            }
            try
            {
                //读取消息头部
                _head = new byte[2];
                _socket.Receive(_head);
                //消息包体长度
                _dataLen = Converter.ByteArray2Short(_head, 0);
                // 读取消息体
                byte[] recvBytesBody = new byte[_dataLen];
                short bodyLen = _dataLen;
                if (bodyLen == 0) continue;
                // 当前需要接收的字节数>0,则循环接收
                while (bodyLen > 0)
                {
                    byte[] recvBytes = new byte[bodyLen < 1024 ? bodyLen : 1024];
                    int isReadBytesBody = 0;
                    if (bodyLen >= recvBytes.Length)
                    {
                        isReadBytesBody = _socket.Receive(recvBytes, recvBytes.Length, 0);
                    }
                    else
                    {
                        isReadBytesBody = _socket.Receive(recvBytes, bodyLen, 0);
                    }
                    recvBytes.CopyTo(recvBytesBody, recvBytesBody.Length - bodyLen);
                    bodyLen -= (short)isReadBytesBody;
                }
                // 插入消息队列
                lock (_packetReceiveQueue)
                {
                    _packetReceiveQueue.Enqueue(new NetPacket(_dataLen, recvBytesBody));
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception e)
            {
                Debug.LogError("网络连接错误：" + e);
                Closed();
                break;
            }
        }
    }

    /// <summary>
    /// 发送消息线程
    /// </summary>
    private static void SendThread()
    {
        // 循环读取数据
        while (IsConnect)
        {
            //阻止发送线程循环操作，为了减少系统消耗.
            _autoEvent.WaitOne();
            if (!IsConnect)
            {
                Debug.Log("网络连接断开！");
                Closed();
                break;
            }
            try
            {
                lock (_packetSendQueue)
                {
                    while (_packetSendQueue.Count > 0)
                    {
                        NetPacket netPacket = _packetSendQueue.Dequeue() as NetPacket;
                        byte[] length = Converter.Short2ByteArray((short)netPacket.Data.Length);
                        // 发送
                        _socket.Send(length, SocketFlags.None);
                        _socket.Send(netPacket.Data, SocketFlags.None);
                    }
                }
            }
            catch (SocketException e)
            {
                Debug.LogError("网络消息发送错误：" + e);
                Closed();
                break;
            }
        }
    }

    /// <summary>
    /// Lua发送消息
    /// </summary>
    public static void LuaSend(params object[] args)
    {
        Send(true, args);
    }

    /// <summary>
    /// CSharp发送消息
    /// </summary>
    public static void CSharpSend(params object[] args)
    {
        object[] parameters = args;
        for (int i = 0; i < parameters.Length; i++)
        {
            if (parameters[i] == null ||
                parameters[i].GetType() == typeof(int) ||
                parameters[i].GetType() == typeof(short) ||
                parameters[i].GetType() == typeof(long) ||
                parameters[i].GetType() == typeof(float) ||
                parameters[i].GetType() == typeof(double) ||
                parameters[i].GetType() == typeof(bool) ||
                parameters[i].GetType() == typeof(string) ||
                parameters[i].GetType() == typeof(JArray) ||
                parameters[i].GetType() == typeof(JObject))
            {
                if (parameters[i].GetType() == typeof(JArray) || parameters[i].GetType() == typeof(JObject))
                {
                    parameters[i] = "_t" + JsonConvert.SerializeObject(parameters[i]);
                }
                else if (parameters[i].GetType() == typeof(float))
                {
                    parameters[i] = (double)parameters[i];
                }
            }
            else
            {
                Debug.LogError("存在网络消息不支持的参数类型！");
                return;
            }
        }
        Send(false, parameters);
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="isLua"></param>
    /// <param name="args"></param>
    private static void Send(bool isLua, params object[] args)
    {
        if (!IsConnect)
        {
            Debug.Log("网络连接断开！");
            return;
        };
        try
        {
            //消息编码
            if (isLua)
            {
                NetAnalysis.LuaEncode();
            }
            else
            {
                NetAnalysis.CSharpEncode(args);
            }
            //取出发送数据
            byte[] data = NetAnalysis.SendData;
            //取出发送数据的实际长度
            int realLen = NetAnalysis.SendDataLength + 1;
            //拷贝实际数据
            byte[] newData = new byte[realLen];
            for (int i = 1; i < realLen; i++)
            {
                newData[i] = data[i - 1];
            };
            newData[0] = _sequence;
            // 插入到发送队列
            lock (_packetSendQueue)
            {
                _packetSendQueue.Enqueue(new NetPacket((short)realLen, newData));
                //开启发送线程的操作
                _autoEvent.Set();
            }
            //设置消息队列码
            _sequence++;
        }
        catch (SocketException e)
        {
            Debug.LogError("网络消息发送错误：" + e);
        }
    }

    /// <summary>
    /// 关闭socket
    /// </summary>
    public static void Closed()
    {
        if (_threadReceive != null)
        {
            _threadReceive.Abort();
            _threadReceive = null;
        }
        if (_threadSend != null)
        {
            _threadSend.Abort();
            _threadSend = null;
        }

        if (_socket != null)
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
            _socket.Dispose();
            _socket = null;
        }

        if (_packetSendQueue != null)
        {
            _packetSendQueue.Clear();
            _packetSendQueue = Queue.Synchronized(new Queue());
        }

        if (_packetReceiveQueue != null)
        {
            _packetReceiveQueue.Clear();
            _packetReceiveQueue = Queue.Synchronized(new Queue());
        }

        if (_autoEvent != null)
        {
            _autoEvent.Close();
            _autoEvent.Dispose();
            _autoEvent = new AutoResetEvent(false);
        }
    }

    /// <summary>
    /// 分发服务器消息
    /// </summary>
    private static void DispatchServerMessage(int length, byte[] data)
    {
        //解码
        NetAnalysis.Decode(length, data);
        int count = NetAnalysis.ReceiveData.Count;
        if (count <= 0) return;
        List<object> receiveData = NetAnalysis.ReceiveData;
        if (receiveData.Count <= 0)
        {
            return;
        }
        string msgFun = receiveData[0].ToString();
        if (msgFun.Contains("CL_"))
        {
            //派发到CSharp和Lua
            DispatchServerMessageToCSharp(msgFun, count, Common.CopyObjectList(receiveData));
            DispatchServerMessageToLua(msgFun, count, Common.CopyObjectList(receiveData));
        }
        else if (msgFun.Contains("C_"))
        {
            //派发到CSharp
            DispatchServerMessageToCSharp(msgFun, count, receiveData);
        }
        else if (msgFun.Contains("L_"))
        {
            //派发到Lua
            DispatchServerMessageToLua(msgFun, count, receiveData);
        }

    }

    /// <summary>
    /// 分发服务器消息到CSharp
    /// </summary>
    private static void DispatchServerMessageToCSharp(string msgFun, int count, List<object> data)
    {
        //转换table数据类型
        for (int i = 1; i < data.Count; i++)
        {
            object param = data[i];
            if (param == null || param.GetType() != typeof(string))
            {
                continue;
            }
            string str = param.ToString();
            if (string.IsNullOrEmpty(str))
            {
                continue;
            }
            string first = str.Substring(0, 3);
            if (first == "_t[")
            {
                string json = str.Substring(2, str.Length - 2);
                data[i] = JsonConvert.DeserializeObject<JArray>(json);
            }
            else if (first == "_t{")
            {
                string json = str.Substring(2, str.Length - 2);
                data[i] = JsonConvert.DeserializeObject<JObject>(json);
            }
        }

        Type type = typeof(GasToGac);
        MethodInfo func = type.GetMethod(msgFun);
        if (func == null)
        {
            Debug.LogWarning("服务器消息：" + msgFun + "，没有接收函数。");
            return;
        }
        if (count > 10)
        {
            Debug.LogWarning("服务器消息参数已经超出限制！！！");
            return;
        }

        if (count == 1)
        {
            func.Invoke(null, new object[] { });
        }
        else if (count == 2)
        {
            func.Invoke(null, new object[] { data[1] });
        }
        else if (count == 3)
        {
            func.Invoke(null, new object[] { data[1], data[2] });
        }
        else if (count == 4)
        {
            func.Invoke(null, new object[] { data[1], data[2], data[3] });
        }
        else if (count == 5)
        {
            func.Invoke(null, new object[] { data[1], data[2], data[3], data[4] });
        }
        else if (count == 6)
        {
            func.Invoke(null, new object[] { data[1], data[2], data[3], data[4], data[5] });
        }
        else if (count == 7)
        {
            func.Invoke(null, new object[] { data[1], data[2], data[3], data[4], data[5], data[6] });
        }
        else if (count == 8)
        {
            func.Invoke(null, new object[] { data[1], data[2], data[3], data[4], data[5], data[6], data[7] });
        }
        else if (count == 9)
        {
            func.Invoke(null, new object[] { data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[8] });
        }
        else if (count == 10)
        {
            func.Invoke(null, new object[] { data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[8], data[9] });
        }
    }

    /// <summary>
    /// 分发服务器消息到Lua
    /// </summary>
    private static void DispatchServerMessageToLua(string msgFun, int count, List<object> data)
    {
        _luaReceive.BeginPCall();
        _luaReceive.Push(msgFun);
        for (int i = 1; i < count; i++)
        {
            _luaReceive.Push(data[i]);
        }
        _luaReceive.PCall();
        _luaReceive.EndPCall();
    }

    /// <summary>
    /// 回调lua
    /// </summary>
    /// <param name="luaFun"></param>
    private static void CallLuaFunction(LuaFunction luaFun)
    {
        if (luaFun != null)
        {
            luaFun.BeginPCall();
            luaFun.PCall();
            luaFun.EndPCall();
        }
    }
}