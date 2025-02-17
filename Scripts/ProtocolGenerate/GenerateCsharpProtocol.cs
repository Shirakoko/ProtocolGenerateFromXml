using System.IO;
using System.Xml;
using UnityEngine;


public class GenerateCsharpProtocol
{
    // 协议保存路径
    private string SAVE_PATH = Application.dataPath + "/Scripts/Protocol/";
    // 生成枚举
    public void GenerateEnum(XmlNodeList nodes)
    {
        string nameSpaceStr = "";
        string enumNameStr = "";
        string fieldStr = "";

        foreach(XmlNode enumNode in nodes)
        {
            nameSpaceStr = enumNode.Attributes["namespace"].Value;
            enumNameStr = enumNode.Attributes["name"].Value;

            XmlNodeList enumFields = enumNode.SelectNodes("field");
            fieldStr = "";
            foreach(XmlNode enumField in enumFields)
            {
                fieldStr += "\t\t" + enumField.Attributes["name"].Value;
                if(enumField.InnerText != "")
                {
                    fieldStr += " = " + enumField.InnerText;
                }
                fieldStr += ",\r\n";
            }

            string enumStr = $"namespace {nameSpaceStr}\r\n" + 
                            "{\r\n" + 
                                $"\tpublic enum {enumNameStr}\r\n" +
                                "\t{\r\n" + 
                                    $"{fieldStr}" +
                                "\t}\r\n" + 
                            "}";

            string path = SAVE_PATH + nameSpaceStr + "/Enum/";
            if(Directory.Exists(path) == false) {
                Directory.CreateDirectory(path);
            }

            // 字符串保存为枚举脚本文件
            File.WriteAllText(path + enumNameStr + ".cs", enumStr);
        }
    }
    // 生成数据结构类
    public void GenerateData(XmlNodeList nodes)
    {
        string nameSpaceStr = "";
        string classNameStr = "";
        string allFieldsStr = "";
        string getBytesLengthStr = "";
        string getConvertToByteArrayStr = "";
        string readFromByteArrayStr = "";

        foreach(XmlNode dataNode in nodes)
        {
            nameSpaceStr = dataNode.Attributes["namespace"].Value;
            classNameStr = dataNode.Attributes["name"].Value;
            XmlNodeList fields = dataNode.SelectNodes("field");
            allFieldsStr = GetAllFieldsStr(fields);
            getBytesLengthStr = GetGetBytesNumStr(fields);
            getConvertToByteArrayStr = GetConvertToByteArray(fields);
            readFromByteArrayStr = GetReadFromByteArrayStr(fields);

            string dataStr = "using System;\r\n" + 
                            "using System.Collections.Generic;\r\n" + 
                            "using System.Text;\r\n" + 
                            $"namespace {nameSpaceStr}\r\n" + 
                            "{\r\n" +
                            $"\tpublic class {classNameStr} : BaseData\r\n" + 
                            "\t{\r\n" + 
                                $"{allFieldsStr}" +
                                "\r\n\t\tpublic override int GetBytesLength()\r\n" + 
                                "\t\t{\r\n" +
                                    "\t\t\tint num = 0;\r\n" + 
                                    $"{getBytesLengthStr}" +
                                "\t\t\treturn num;\r\n" + 
                                "\t\t}\r\n" + 
                                "\r\n\t\tpublic override byte[] ConvertToByteArray()\r\n" +
                                "\t\t{\r\n" +
                                    "\t\t\tint index = 0;\r\n" +
                                    "\t\t\tbyte[] bytes = new byte[GetBytesLength()];\r\n" +
                                    $"{getConvertToByteArrayStr}" + 
                                    "\t\t\treturn bytes;\r\n" +
                                "\t\t}\r\n" +
                                "\r\n\t\tpublic override int ReadFromByteArray(byte[] bytes, int beginIndex = 0)\r\n" +
                                "\t\t{\r\n" +
                                    "\t\t\tint index = beginIndex;\r\n" +
                                    $"{readFromByteArrayStr}" + 
                                    "\t\t\treturn index - beginIndex;\r\n" +
                                "\t\t}\r\n" +
                            "\t}\r\n" +
                            "}\r\n";
            
            string path = SAVE_PATH + nameSpaceStr + "/Data/";
            if(Directory.Exists(path) == false) {
                Directory.CreateDirectory(path);
            }

            // 字符串保存为数据结构脚本文件
            File.WriteAllText(path + classNameStr + ".cs", dataStr);
        }
    }
    //生成消息类
    public void GenerateMsg(XmlNodeList nodes)
    {
        string idStr = "";
        string namespaceStr = "";
        string classNameStr = "";
        string fieldStr = "";
        string getBytesNumStr = "";
        string writingStr = "";
        string readingStr = "";

        foreach (XmlNode dataNode in nodes)
        {
            idStr = dataNode.Attributes["id"].Value;
            namespaceStr = dataNode.Attributes["namespace"].Value;
            classNameStr = dataNode.Attributes["name"].Value;
            XmlNodeList fields = dataNode.SelectNodes("field");
            fieldStr = GetFieldStr(fields);
            getBytesNumStr = GetGetBytesNumStr(fields);
            writingStr = GetWritingStr(fields);
            readingStr = GetReadingStr(fields);

            string dataStr = "using System;\r\n" +
                             "using System.Collections.Generic;\r\n" +
                             "using System.Text;\r\n" +
                             $"namespace {namespaceStr}\r\n" +
                              "{\r\n" +
                              $"\tpublic class {classNameStr} : BaseMsg\r\n" +
                              "\t{\r\n" +
                                    $"{fieldStr}" +
                                    "\t\tpublic override int GetBytesNum()\r\n" +
                                    "\t\t{\r\n" +
                                        "\t\t\tint num = 8;\r\n" + //这个8代表的是 消息ID的4个字节 + 消息体长度的4个字节
                                        $"{getBytesNumStr}" +
                                        "\t\t\treturn num;\r\n" +
                                    "\t\t}\r\n" +
                                    "\t\tpublic override byte[] Writing()\r\n" +
                                    "\t\t{\r\n" +
                                        "\t\t\tint index = 0;\r\n" +
                                        "\t\t\tbyte[] bytes = new byte[GetBytesNum()];\r\n" +
                                        "\t\t\tWriteInt(bytes, GetID(), ref index);\r\n" +
                                        "\t\t\tWriteInt(bytes, bytes.Length - 8, ref index);\r\n" +
                                        $"{writingStr}" +
                                        "\t\t\treturn bytes;\r\n" +
                                    "\t\t}\r\n" +
                                    "\t\tpublic override int Reading(byte[] bytes, int beginIndex = 0)\r\n" +
                                    "\t\t{\r\n" +
                                        "\t\t\tint index = beginIndex;\r\n" +
                                        $"{readingStr}" +
                                        "\t\t\treturn index - beginIndex;\r\n" +
                                    "\t\t}\r\n" +
                                    "\t\tpublic override int GetID()\r\n" +
                                    "\t\t{\r\n" +
                                        "\t\t\treturn " + idStr + ";\r\n" +
                                    "\t\t}\r\n" +
                              "\t}\r\n" +
                              "}";

            string path = SAVE_PATH + namespaceStr + "/Msg/";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            File.WriteAllText(path + classNameStr + ".cs", dataStr);

            // 生成消息处理类
            if(File.Exists(path+classNameStr+"Handler.cs"))
            {
                continue;
            }
            string handlerStr = $"namespace {namespaceStr}\r\n" + 
                    "{\r\n" + 
                        $"\tpublic class {classNameStr}Handler : BaseHandler\r\n" +
                        "\t{\r\n" + 
                            "\t\tpublic override void MsgHandle()\r\n" +
                            "\t\t{\r\n" +
                                $"\t\t\t{classNameStr} msg = message as {classNameStr};\r\n" +
                                "\t\t\t// TODO: 添加处理逻辑\r\n" + // 提示添加处理逻辑
                            "\t\t}\r\n" +
                        "\t}\r\n" + 
                    "}\r\n";
            // 保存Csharp脚本
            File.WriteAllText(path + classNameStr + "Handler.cs", handlerStr);
        }
        Debug.Log("消息类生成结束");
    }

    // 生成消息池
    public void GenerateMsgPool(XmlNodeList nodes)
    {
        List<string> ids = new List<string>();
        List<string> names = new List<string>();
        List<string> nameSpaces = new List<string>();

        foreach(XmlNode dataNode in nodes)
        {
            string id = dataNode.Attributes["id"].Value;
            if(!ids.Contains(id)) {
                ids.Add(id);
            } else {
                Debug.Log("配置文件中存在相同ID的消息：" + id);
            }

            string name = dataNode.Attributes["name"].Value;
            if(!names.Contains(name)) {
                names.Add(name);
            } else {
                Debug.Log("存在类名相同的消息: " + name);
            }

            string nameSpace = dataNode.Attributes["namespace"].Value;
            if(!nameSpaces.Contains(nameSpace)) {
                nameSpaces.Add(nameSpace);
            }
        }

        string nameSpaceStr = "";
        for(int i=0; i<nameSpaces.Count; i++)
        {
            nameSpaceStr += $"using {nameSpaces[i]};\r\n";
        }

        string registerStr = "";
        for(int i=0; i<ids.Count; i++)
        {
            registerStr += $"\t\tRegister({ids[i]}, typeof({names[i]}), typeof({names[i]}Handler));\r\n";
        }

        string msgPoolStr = "using System;\r\n" + "using System.Collections.Generic;\r\n" +
                            nameSpaceStr + 
                            "public class MsgPool\r\n" +
                            "{\r\n" + 
                                "\tprivate Dictionary<int, Type> messages = new Dictionary<int, Type>();\r\n" + 
                                "\tprivate Dictionary<int, Type> handlers = new Dictionary<int, Type>();\r\n" + 
                                "\tpublic MsgPool()\r\n" + 
                                "\t{\r\n" + 
                                    registerStr + 
                                "\t}\r\n" + 
                                "\tprivate void Register(int mesageID, Type messageType, Type handlerType)\r\n" + 
                                "\t{\r\n" + 
                                    "\t\tmessages.Add(mesageID, messageType);\r\n" + 
                                    "\t\thandlers.Add(mesageID, handlerType);\r\n" + 
                                "\t}\r\n" + 
                                "\tpublic BaseMsg GetMessage(int messageID)\r\n" + 
                                "\t{\r\n" + 
                                    "\t\tif(!messages.ContainsKey(messageID))\r\n" +
                                    "\t\t{\r\n" + 
                                        "\t\t\treturn null;\r\n" + 
                                    "\t\t}\r\n" + 
                                    "\t\treturn Activator.CreateInstance(messages[messageID]) as BaseMsg;\r\n" + 
                                "\t}\r\n" + 
                                "\tpublic BaseHandler GetHandler(int messageID)\r\n" + 
                                "\t{\r\n" + 
                                    "\t\tif(!handlers.ContainsKey(messageID))\r\n" +
                                    "\t\t{\r\n" + 
                                        "\t\t\treturn null;\r\n" + 
                                    "\t\t}\r\n" + 
                                    "\t\treturn Activator.CreateInstance(handlers[messageID]) as BaseHandler;\r\n" + 
                                "\t}\r\n" + 
                            "}\r\n"; 

        string path = SAVE_PATH + "/Pool/";
        if(!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }

        File.WriteAllText(path + "MsgPool.cs", msgPoolStr);
        Debug.Log("消息池生成结束");
    }

    // 获取所有成员变量声明内容
    private string GetAllFieldsStr(XmlNodeList fields)
    {
        string fieldStr = "";
        foreach(XmlNode field in fields)
        {
            string type = field.Attributes["type"].Value;
            string fieldName = field.Attributes["name"].Value;
            if(type == "list")
            {
                string T = field.Attributes["T"].Value;
                fieldStr += "\t\tpublic " + "List<" + T + "> ";
            }
            else if(type == "array")
            {
                string element = field.Attributes["element"].Value;
                fieldStr += "\t\tpublic " + element + "[] ";
            }
            else if(type == "dict")
            {
                string Tkey = field.Attributes["Tkey"].Value;
                string TValue = field.Attributes["Tvalue"].Value;
                fieldStr += "\t\tpublic Dictionary<" + Tkey + ", " + TValue + "> ";
            }
            else if(type == "enum")
            {
                string enumType = field.Attributes["enumType"].Value;
                fieldStr += "\t\tpublic " + enumType + " ";
            }
            else
            {
                fieldStr += "\t\tpublic " + type + " ";
            }

            fieldStr += fieldName + ";\r\n";
        }

        return fieldStr;
    }

    // 获取字节数组长度的函数
    private string GetGetBytesNumStr(XmlNodeList fields)
    {
        string getBytesLengthStr = "";
        string type = "";
        string name = "";
        foreach(XmlNode field in fields)
        {
            type = field.Attributes["type"].Value;
            name = field.Attributes["name"].Value;

            if(type == "list")
            {
                string T = field.Attributes["T"].Value;
                getBytesLengthStr += "\r\n\t\t\tnum += sizeof(int);\r\n";
                getBytesLengthStr += $"\t\t\tfor(int i=0; i<{name}.Count; i++)\r\n" + "\t\t\t{\r\n";
                getBytesLengthStr += "\t\t\t\tnum += " + GetValueBytesNum(T, name + "[i]") + ";\r\n";
                getBytesLengthStr += "\t\t\t}\r\n";
            }
            else if(type == "array")
            {
                string T = field.Attributes["element"].Value;
                getBytesLengthStr += "\r\n\t\t\tnum += sizeof(int);\r\n";
                getBytesLengthStr += $"\t\t\tfor(int i=0; i<{name}.Length; i++)\r\n" + "\t\t\t{\r\n";
                getBytesLengthStr += "\t\t\t\tnum += " + GetValueBytesNum(T, name + "[i]") + ";\r\n";
                getBytesLengthStr += "\t\t\t}\r\n";
            }
            else if(type == "dict")
            {
                string Tkey = field.Attributes["Tkey"].Value;
                string Tvalue = field.Attributes["Tvalue"].Value;
                getBytesLengthStr += "\r\n\t\t\tnum += sizeof(int);\r\n";
                getBytesLengthStr += $"\t\t\tforeach({Tkey} key in {name}.Keys)\r\n" + "\t\t\t{\r\n";
                getBytesLengthStr += "\t\t\t\tnum += " + GetValueBytesNum(Tkey, "key") + ";\r\n";
                getBytesLengthStr += "\t\t\t\tnum += " + GetValueBytesNum(Tvalue, name + "[key]") + ";\r\n";
                getBytesLengthStr += "\t\t\t}\r\n";
            }
            else
            {
                getBytesLengthStr += "\t\t\tnum += " + GetValueBytesNum(type, name) + ";\r\n";
            }
        }

        return getBytesLengthStr;
    }

    // 获取序列化字节数组的函数
    private string GetConvertToByteArray(XmlNodeList fields)
    {
        string writingStr = "";
        string type = "";
        string name = "";
        foreach(XmlNode field in fields)
        {
            type = field.Attributes["type"].Value;
            name = field.Attributes["name"].Value;

            if(type == "list")
            {
                string T = field.Attributes["T"].Value;
                writingStr += "\t\t\tWriteInt(bytes, " + name + ".Count, ref index);\r\n";
                writingStr += "\t\t\tfor (int i=0; i<" + name +".Count; i++)\r\n";
                writingStr += "\t\t\t\t" + GetFieldWritingStr(T, name + "[i]") + "\r\n";
            }
            else if(type == "array")
            {
                string element = field.Attributes["element"].Value;
                writingStr += "\t\t\tWriteInt(bytes, " + name + ".Length, ref index);\r\n";
                writingStr += "\t\t\tfor (int i=0; i<" + name +".Length; i++)\r\n";
                writingStr += "\t\t\t\t" + GetFieldWritingStr(element, name + "[i]") + "\r\n";
            }
            else if(type == "dict")
            {
                string Tkey = field.Attributes["Tkey"].Value;
                string Tvalue = field.Attributes["Tvalue"].Value;
                writingStr += "\t\t\tWriteInt(bytes, " + name + ".Count, ref index);\r\n";
                writingStr += "\t\t\tforeach(" + Tkey + " key in " + name + ".Keys)\r\n";
                writingStr += "\t\t\t{\r\n";
                writingStr += "\t\t\t\t" + GetFieldWritingStr(Tkey, "key") + "\r\n";
                writingStr += "\t\t\t\t" + GetFieldWritingStr(Tvalue, name + "[key]") + "\r\n";
                writingStr += "\t\t\t}\r\n";
            }
            else
            {
                writingStr += "\t\t\t" + GetFieldWritingStr(type, name) + "\r\n";
            }

            writingStr += "\r\n";
        }
        return writingStr;
    }

    // 获取反序列化方法的函数
    private string GetReadFromByteArrayStr(XmlNodeList fields)
    {
        string readingStr = "";

        string type = "";
        string name = "";
        foreach(XmlNode field in fields)
        {
            type = field.Attributes["type"].Value;
            name = field.Attributes["name"].Value;

            if(type == "list")
            {
                string T = field.Attributes["T"].Value;
                readingStr += "\t\t\t" + name + " = new List<" +T +">();\r\n";
                readingStr += "\t\t\tint " + name + "Count = ReadInt(bytes, ref index);\r\n";
                readingStr += "\t\t\tfor(int i=0; i<" + name + "Count; i++)\r\n";
                readingStr += "\t\t\t\t" + name + ".Add(" + GetFieldReadingStr(T) + ");\r\n";
            }
            else if(type == "array")
            {
                string element = field.Attributes["element"].Value;
                readingStr += "\t\t\tint " + name + "Length = ReadInt(bytes, ref index);\r\n";
                readingStr += "\t\t\t" + name + " = new " + element + "[" + name + "Length];\r\n";
                readingStr += "\t\t\tfor(int i=0; i<" + name + "Length; i++)\r\n";
                readingStr += "\t\t\t\t" + name + "[i] = " + GetFieldReadingStr(element) + ";\r\n";
            }
            else if(type == "dict")
            {
                string Tkey = field.Attributes["Tkey"].Value;
                string Tvalue = field.Attributes["Tvalue"].Value;
                readingStr += "\t\t\t" + name + "= new Dictionary<" + Tkey + ", " + Tvalue + ">();\r\n";
                readingStr += "\t\t\tint " + name + "Count = ReadInt(bytes, ref index);\r\n";
                readingStr += "\t\t\tfor(int i=0; i<" + name + "Count; i++)\r\n";
                readingStr += "\t\t\t\t" + name + ".Add(" + GetFieldReadingStr(Tkey) + ", " + GetFieldReadingStr(Tvalue) + ");\r\n";
            }
            else if(type == "enum")
            {
                string enumType = field.Attributes["enumType"].Value;
                readingStr += "\t\t\t" + name + "= (" + enumType + ")ReadInt(bytes, ref index);\r\n";
            }
            else
            {
                readingStr += "\t\t\t" + name + " = " + GetFieldReadingStr(type) + ";\r\n";
            }

            readingStr += "\r\n";
        }

        return readingStr;
    }
    private string GetFieldReadingStr(string type)
    {
        switch(type)
        {
            case "byte":
                return "ReadByte(bytes, ref index)";
            case "int":
                return "ReadInt(bytes, ref index)";
            case "float":
                return "ReadFloat(bytes, ref index)";
            case "short":
                return "ReadShort(bytes, ref index)";
            case "long":
                return "ReadLong(bytes, ref index)";
            case "bool":
                return "ReadBool(bytes, ref index)";
            case "string":
                return "ReadString(bytes, ref index)";
            default:
                return "ReadObject<" + type + ">(bytes, ref index)";
        }
    }
    
    private string GetFieldWritingStr(string type, string name)
    {
        switch (type)
        {
            case "byte":
                return "WriteByte(bytes, " + name + ", ref index);";
            case "int":
                return "WriteInt(bytes, " + name + ", ref index);";
            case "float":
                return "WriteFloat(bytes, " + name + ", ref index);";
            case "short":
                return "WriteShort(bytes, " + name + ", ref index);";
            case "long":
                return "WriteLong(bytes, " + name + ", ref index);";
            case "bool":
                return "WriteLong(bytes, " + name + ", ref index);";
            case "string":
                return "WriteString(bytes, " + name + ", ref index);";
            case "enum":
                return "WriteInt(bytes, Convert.ToInt32(" + name + "), ref index);"; 
            default:
                return "WriteObject(bytes, " + name + ", ref index);";
        }
    }

    private string GetValueBytesNum(string type, string name)
    {
        switch(type)
        {
            case "int":
            case "float":
            case "byte":
            case "bool":
            case "short":
            case "long":
                return $"sizeof({type})";
            case "enum":
                return "sizeof(int)";
            case "string":
                return $"sizeof(int) + Encoding.UTF8.GetBytes({name}).Length";
            default:
                return name + ".GetBytesLength()";
        }
    }
}
