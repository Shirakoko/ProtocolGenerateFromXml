using System.Xml;
using UnityEditor;
using UnityEngine;

public class ProtocolGenerate
{
    // 配置文件所在路径
    private static string PROTO_INFO_PATH = Application.dataPath + "/Editor/ProtocolGenerate/Test.xml";
    private static GenerateCsharpProtocol _generator = new GenerateCsharpProtocol();

    [MenuItem("ProtolGenerate/生成C#脚本")]
    private static void GenerateCSharp()
    {
        Debug.Log("生成C#代码");
        XmlNode root = GetRoot("messages");
        _generator.GenerateEnum(GetChildrenNodes(root, "enum"));
        _generator.GenerateData(GetChildrenNodes(root, "data"));
        _generator.GenerateMsg(GetChildrenNodes(root, "message"));
        _generator.GenerateMsgPool(GetChildrenNodes(root, "message"));

        AssetDatabase.Refresh(); // 刷新Assets文件夹
    }

    private static XmlNode GetRoot(string rootName)
    {
        XmlDocument xml = new XmlDocument();
        xml.Load(PROTO_INFO_PATH);
        XmlNode root = xml.SelectSingleNode(rootName);

        return root;
    }

    private static XmlNodeList GetChildrenNodes(XmlNode node, string nodeName)
    {
        return node.SelectNodes(nodeName);
    }
}
