using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VTUnpacker
{
    public class CNode
    {
        private string name;
        private readonly Dictionary<string, string> values;
        private readonly List<CNode> nodes;
        private readonly static int specialCharCount = "={}<>\"".Length;
        private readonly  static string decodedString ="CustomScenarioCAMPAIGNVTMapCustom";
        private static byte[] readEncodeBuffer = new byte[0];
        private struct ConfigValue
        {
            public ConfigValue(string name, string value)
            {
                this.name = name;
                this.value = value;
            }
            public string name;
            public string value;
        }
        public CNode()
        {
            this.name = "NODE";
            this.values = new Dictionary<string, string>();
            this.nodes = new List<CNode>();
        }
        private List<ConfigValue> GetValues()
        {
            List<ConfigValue> list = new List<ConfigValue>();
            foreach (string text in this.values.Keys)
            {
                list.Add(new ConfigValue(text, this.values[text]));
            }
            return list;
        }
        private void SetValueString(string name, string value)
        {
            if (this.values.ContainsKey(name))
            {
                this.values[name] = value;
                return;
            }
            this.values.Add(name, value);
        }
        private void AddNode(CNode node)
        {
            this.nodes.Add(node);
        }
        private static CNode LoadFromFile(string filePath, bool logErrors = true)
        {
            filePath = filePath.Replace('\\', '/');
            CNode configNode;
            try
            {
                configNode = ParseNode(File.ReadAllText(filePath));
            }
            catch (Exception ex)
            {
                if (logErrors)
                {
                    Console.WriteLine(ex);
                }
                configNode = null;
            }
            return configNode;
        }
        private static string WriteNode(CNode rootNode, int indent)
        {
            string text = string.Empty;
            text += GetIndent(indent);
            text += rootNode.name;
            text = text + "\n" + GetIndent(indent) + "{";
            foreach (ConfigValue configValue in rootNode.GetValues())
            {
                text = text + "\n" + GetIndent(indent + 1);
                string text2 = configValue.name;
                string text3 = configValue.value;
                if (!string.IsNullOrEmpty(text3))
                {
                    text3 = text3.Replace("\n", "///n");
                    if (HasSpecialChars(text2) || HasSpecialChars(text3))
                    {
                        text2 = "{" + text2 + "}";
                        text3 = "{" + text3 + "}";
                    }
                }
                text = text + text2 + " = " + text3;
            }
            text += "\n";
            foreach (CNode configNode in rootNode.nodes)
            {
                text += WriteNode(configNode, indent + 1);
            }
            text = text + "\n" + GetIndent(indent) + "}\n";
            return text.Replace("\n\n", "\n");
        }
        private static CNode ParseNode(string nodeString)
        {
            CNode configNode = new CNode();
            string[] array = nodeString.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            configNode.name = array[0].Replace(" ", string.Empty).Replace("\t", string.Empty);
            int num = 0;
            for (int i = 2; i < array.Length; i++)
            {
                string text = array[i];
                if (!text.Contains("="))
                {
                    num = i;
                    break;
                }
                if (text.Contains("{"))
                {
                    int num2 = 0;
                    int num3 = 0;
                    int num4 = 0;
                    int num5 = 0;
                    int num6 = 0;
                    bool flag = false;
                    for (int j = 0; j < text.Length; j++)
                    {
                        if (text[j] == '{')
                        {
                            if (num6 == 0)
                            {
                                if (flag)
                                {
                                    num4 = j + 1;
                                }
                                else
                                {
                                    num2 = j + 1;
                                }
                            }
                            num6++;
                        }
                        else if (text[j] == '}')
                        {
                            num6--;
                            if (num6 == 0)
                            {
                                if (flag)
                                {
                                    num5 = j;
                                }
                                else
                                {
                                    num3 = j;
                                    flag = true;
                                }
                            }
                        }
                    }
                    string text2 = text[0..^(num5 - num2)];
                    string text3 = text[0..^(num5 - num4)];
                    text3 = text3.Replace("///n", "\n");
                    configNode.SetValueString(text2, text3);
                }
                else
                {
                    text = text.Replace("= ", "=");
                    string[] array2 = text.Split(new char[] { '=' });
                    string text4 = array2[0].Replace(" ", string.Empty).Replace("\t", string.Empty);
                    string text5 = array2[1];
                    text5 = text5.Replace("///n", "\n");
                    configNode.SetValueString(text4, text5);
                }
            }
            if (array[num].Contains("}"))
            {
                return configNode;
            }
            int num7 = 0;
            int num8 = -1;
            for (int k = num; k < array.Length; k++)
            {
                if (!array[k].Contains("="))
                {
                    if (array[k].Contains("{"))
                    {
                        num7++;
                        if (num7 == 1)
                        {
                            num8 = k - 1;
                        }
                    }
                    else if (array[k].Contains("}"))
                    {
                        if (num7 == 1 && num8 >= 0)
                        {
                            int num9 = k + 1;
                            string text6 = string.Join("\n", array, num8, num9 - num8);
                            configNode.AddNode(ParseNode(text6));
                            num8 = -1;
                        }
                        num7--;
                    }
                }
            }
            return configNode;
        }
        private static string GetIndent(int indent)
        {
            string text = string.Empty;
            for (int i = 0; i < indent; i++)
            {
                text += "\t";
            }
            return text;
        }
        private static bool HasSpecialChars(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return false;
            }
            int length = s.Length;
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < specialCharCount; j++)
                {
                    if (s[i] == "={}<>\""[j])
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private static void WorkshopDecode(byte[] byteArray)
        {
            for (int i = 0; i < byteArray.Length; i++)
            {
                byteArray[i] = WorkshopDecode((int)byteArray[i]);
            }
        }
        private static string ReadFirstLine(string path)
        {
            if (!File.Exists(path))
            {
                return string.Empty;
            }
            return File.ReadLines(path).First<string>();
        }
        public static CNode ReadWorkshopConfig(string path, bool removeOriginal = true)
        {
            string text = ReadFirstLine(path);
            if (decodedString.Contains(text))
            {
                Console.WriteLine("Workshop config file isn't packet! (" + path + ")");
                return LoadFromFile(path, true);
            }
            CNode configNode;
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                int num = (int)stream.Length;
                if (readEncodeBuffer.Length < num)
                {
                    readEncodeBuffer = new byte[num];
                }
                byte[] array = readEncodeBuffer;
                int num2 = 0;
                int num3;
                while ((num3 = stream.ReadByte()) != -1)
                {
                    array[num2] = WorkshopDecode(num3);
                    num2++;
                }
                configNode = ParseNode(Encoding.UTF8.GetString(array, 0, num));
            }
            if (removeOriginal)
            {
                File.Delete(path);
            }
            return configNode;
        }
        public static void WorkshopDecode(string filepath, bool removeOriginal = true)
        {

            byte[] array = File.ReadAllBytes(filepath);
            WorkshopDecode(array);
            File.WriteAllBytes(filepath[0..^1], array);
            if (removeOriginal)
            {
                File.Delete(filepath);
            }

        }
        public static byte WorkshopDecode(int data)
        {
            return (byte)((data - 88) % 256);
        }
        public void SaveToFile(string filePath)
        {
            File.WriteAllLines(filePath, new string[] { WriteNode(this, 0) });
        }
    }
}
