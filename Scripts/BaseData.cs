using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// 可序列化类的基类
public abstract class BaseData
{
    /// <summary>
    /// 获取字节数组容器的长度
    /// </summary>
    /// <returns>字节数组的长度</returns>
    public abstract int GetBytesLength();

    /// <summary>
    /// 把成员变量序列化存入字节数组并返回
    /// </summary>
    /// <returns>成员变量序列化后的字节数组</returns>
    public abstract byte[] ConvertToByteArray();
 
    /// <summary>
    /// 从指定位置开始读取字节数组，反序列化到成员变量中
    /// </summary>
    /// <param name="bytes">字节数组</param>
    /// <param name="beginIndex">开始读取的位置</param>
    /// <returns>该类对象对应的字节数组的长度</returns>
    public abstract int ReadFromByteArray(byte[] bytes, int beginIndex);

    #region 将各类型的数据转成字节数组存入到指定容器中
    protected void WriteInt(byte[] bytes, int value, ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, index);
        index += sizeof(int);
    }

    protected void WriteShort(byte[] bytes, short value, ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, index);
        index += sizeof(short);
    }

    protected void WriteLong(byte[] bytes, long value, ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, index);
        index += sizeof(long);
    }

    protected void WriteFloat(byte[] bytes, float value, ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, index);
        index += sizeof(float);
    }

    protected void WriteBool(byte[] bytes, bool value, ref int index)
    {
        BitConverter.GetBytes(value).CopyTo(bytes, index);
        index += sizeof(bool);
    }

    protected void WriteString(byte[] bytes, string value, ref int index)
    {
        byte[] strBytes = Encoding.UTF8.GetBytes(value);
        WriteInt(bytes, strBytes.Length, ref index);
        strBytes.CopyTo(bytes, index);
        index += strBytes.Length;
    }

    protected void WriteObject(byte[] bytes, BaseData data, ref int index)
    {
        data.ConvertToByteArray().CopyTo(bytes, index);
        index += data.GetBytesLength();
    }
    #endregion   

    #region 将各类型数据对应的字节数组转换成原始数据结构或类对象
    protected int ReadInt(byte[] bytes, ref int index)
    {
        int value = BitConverter.ToInt32(bytes, index);
        index += sizeof(int);
        return value;
    }

    protected float ReadFloat(byte[] bytes, ref int index)
    {
        float value = BitConverter.ToSingle(bytes, index);
        index += sizeof(float);
        return value;
    }

    protected short ReadShort(byte[] bytes, ref int index)
    {
        short value = BitConverter.ToInt16(bytes, index);
        index += sizeof(short);
        return value;
    }

    protected long ReadLong(byte[] bytes, ref int index)
    {
        long value = BitConverter.ToInt64(bytes, index);
        index += sizeof(long);
        return value;
    }

    protected bool ReadBool(byte[] bytes, ref int index)
    {
        bool value = BitConverter.ToBoolean(bytes, index);
        index += sizeof(float);
        return value;
    }

    protected string ReadString(byte[] bytes, ref int index)
    {
        int length = ReadInt(bytes, ref index); // 得到字符串对应的字节数组的长度
        string value = Encoding.UTF8.GetString(bytes, index, length);
        index += length;
        return value;
    }

    protected T ReadObject<T>(byte[] bytes, ref int index) where T: BaseData, new()
    {
        T value = new T();
        index += value.ReadFromByteArray(bytes, index);
        return value;
    }
    #endregion
}