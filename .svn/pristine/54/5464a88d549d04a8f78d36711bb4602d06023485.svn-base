using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Documents;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
namespace SopMessageView.Libs
{

    public class RcdHelper
    {
        private RcdHelper()
        {
            
        }

        private static readonly Encoding SopEncoding =  Encoding.GetEncoding("GB18030");
        private static readonly string FIXED = "fixed";
        private static readonly string VAR = "var";
        /// <summary>
        /// 返回Rcd中Request中的所有的field元素：
        /// 如果field是固定长度，扩展属性设为fixed
        /// 如果field是Var长度，扩展属性设为var
        /// 如果field是表格对象中的字段，扩展属性设为表格对象的名字
        /// </summary>
        /// <param name="rcd">rcd文件全路径</param>
        /// <returns>Request下的field元素</returns>
        private static IEnumerable<XElement> GetRequestFields(string rcd)
        {
            if (!File.Exists(rcd))
                throw new FileNotFoundException("rcd文件不存在。");
            var document = XDocument.Load(rcd);
            var fields =
                document.Descendants("Category").
                Where(o => o.Attribute("Name").Value == "Request").
                First().
                Descendants("Field");
            foreach (var xElement in fields)
            {
                if (xElement.Attribute("FieldLength").Value.Contains("var"))
                {
                    if (xElement.Parent.Name == "Loop")
                    {
                        xElement.SetAttributeValue("Type", xElement.Parent.Attribute("Name").Value);

                    }
                    else
                    {
                        xElement.SetAttributeValue("Type",VAR);
                    }
                }
                else
                {
                    xElement.SetAttributeValue("Type",FIXED);
                }

            }
            return fields;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rcd"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static List<IMessageNode> GetRequestMessageNodes(string rcd,byte[] msg )
        {
            var elements = GetRequestFields(rcd);
            List<IMessageNode> list=new List<IMessageNode>();
            MemoryStream stream = new MemoryStream(msg);
            BinaryReader br = new BinaryReader(stream, Encoding.Default);
            var fixedObjs = from m in elements
                            select new
                            {
                                FieldName = m.Attribute("FieldName").Value,
                                FieldLength = GetLength(m.Attribute("FieldLength").Value),
                                FieldDescription = m.Attribute("FieldDescription").Value,
                                FieldType = m.Attribute("FieldType").Value,
                                ExtendType = m.Attribute("Type").Value
                            };

            try
            {
                for (int i = 0; i < fixedObjs.Count(); i++)
                {
                    var extendType = fixedObjs.ElementAt(i).ExtendType;
                    if (extendType == FIXED || extendType == VAR)
                    {

                        Field field = new Field();
                        field.FieldDescription = fixedObjs.ElementAt(i).FieldDescription;
                        field.FieldName = fixedObjs.ElementAt(i).FieldName;
                        var fieldValueAndPosition = GetFieldValueAndPosition(br, fixedObjs.ElementAt(i).FieldLength,
                                                                             fixedObjs.ElementAt(i).FieldType);
                        field.FieldValue = fieldValueAndPosition.Item3;
                        field.StartPositionInMsg = (int)fieldValueAndPosition.Item1;
                        field.EndPositionInMsg = (int) fieldValueAndPosition.Item2-1;
                        StringBuilder builder = new StringBuilder();
                        for (int j = field.StartPositionInMsg; j < field.EndPositionInMsg + 1; j++)
                        {
                            builder.AppendFormat("{0:X2} ", msg[j]);
                        }
                        field.HexValue = builder.ToString();
                        list.Add(field);
                    }
                    else
                    {
                        Table table = new Table();
                        table.StartPositionInMsg = (int)br.BaseStream.Position;
                        table.TableName = GetFieldValue(br, -1, "string");
                        
                        table.TableRow = br.ReadByte();
                        table.TableCol = br.ReadByte();
                        table.EndPositionInMsg = (int)br.BaseStream.Position-1;
                        List<Field> tableFields = new List<Field>();
                        list.Add(table);

                        for (int row = 0; row < table.TableRow; row++)
                        {
                            for (int col = 0; col < table.TableCol; col++)
                            {

                                int elementIndex = i - table.TableCol * row;
                                Field field = new Field();
                                field.FieldDescription = fixedObjs.ElementAt(elementIndex).FieldDescription;
                                field.FieldName = fixedObjs.ElementAt(elementIndex).FieldName;
                                var fieldValueAndPosition = GetFieldValueAndPosition(br, fixedObjs.ElementAt(i).FieldLength,
                                                     fixedObjs.ElementAt(i).FieldType);
                                field.FieldValue = fieldValueAndPosition.Item3;
                                field.StartPositionInMsg = (int)fieldValueAndPosition.Item1;
                                field.EndPositionInMsg = (int)fieldValueAndPosition.Item2-1;
                                StringBuilder builder = new StringBuilder();
                                for (int j = field.StartPositionInMsg; j < field.EndPositionInMsg + 1; j++)
                                {
                                    builder.AppendFormat("{0:X2} ", msg[j]);
                                }
                                field.HexValue = builder.ToString();
                                tableFields.Add(field);
                                i++;
                            }
                        }
                        i--;
                        table.Fields = tableFields;
                    }

                }
            }
            finally
            {
                br.Close();
                stream.Close();
            }

            return list;
        }

        private static Tuple<long,long,string> GetFieldValueAndPosition(BinaryReader br,int length,string fieldType)
        {
            long currentIndex = br.BaseStream.Position;
            string fieldValue = GetFieldValue(br, length, fieldType);
            long endIndex = br.BaseStream.Position;
            return new Tuple<long, long, string>(currentIndex,endIndex,fieldValue);
        }


        private static string GetFieldValue(BinaryReader br, int length,string fieldType)
        {
            string result = string.Empty;
            if (length==-1)
            {
                length = br.ReadByte();
            }

            switch (fieldType)
            {
                case "nshort":
                    result = IPAddress.NetworkToHostOrder(br.ReadInt16()).ToString();

                    break;
                case "string":
                    result = SopEncoding.GetString(br.ReadBytes(length));

                    break;
                case "ipv4":
                    var data = br.ReadBytes(4);
                    result = String.Format("{0}.{1}.{2}.{3}", data[0], data[1], data[2],
                                           data[3]);

                    break;
                case "byte":
                    result = string.Format("{0:X2}", br.ReadByte());

                    break;
                case "nint":
                    result = IPAddress.NetworkToHostOrder(br.ReadInt32()).ToString();

                    break;
                case "datetime":
                    result = SopEncoding.GetString(br.ReadBytes(length));

                    break;
                default:
                    result = SopEncoding.GetString(br.ReadBytes(length));
                    break;     
            }
            return result.Replace("\0","\\0");

        }

        
         public static List<Tuple<string,string,string>> ParseMessage(IEnumerable<XElement> elements,byte[] msg )
         {
             List<Tuple<string,string,string>> list=new List<Tuple<string, string,string>>();
             
             MemoryStream stream=new MemoryStream(msg);
             BinaryReader br=new BinaryReader(stream,Encoding.Default);
             var fixedObjs = from m in elements
                             select new
                             {
                                 FieldName = m.Attribute("FieldName").Value,
                                 FieldLength = GetLength(m.Attribute("FieldLength").Value),
                                 FieldDescription = m.Attribute("FieldDescription").Value,
                                 FieldType = m.Attribute("FieldType").Value,
                                 ExtendType=m.Attribute("Type").Value
                             };
             List<string> tableList=new List<string>();
             bool hasLoop = false;
             int row = -1;
             int col = -1;
             foreach (var fixedObj in fixedObjs)
             {


                 if (fixedObj.ExtendType!=FIXED&&fixedObj.ExtendType!="var")
                 {
                     hasLoop = true;

                     if (!tableList.Contains(fixedObj.ExtendType))
                     {
                         tableList.Add(fixedObj.ExtendType);
                         int length = br.ReadByte();
                         br.ReadBytes(length);
                         row = br.ReadByte();
                         col = br.ReadByte();
                         Tuple<string,string,string> table=new Tuple<string, string, string>(fixedObj.ExtendType,"表格",string.Format("共有{0}行，{1}列",row,col));
                         list.Add(table);
                     }

                 }
                 else
                 {
                     if (hasLoop)
                     {
                         Tuple<string, string, string> table = new Tuple<string, string, string>(fixedObj.ExtendType, "表格", "结束");
                         list.Add(table);
                     }
                     hasLoop = false;

                 }
                 AddFixedRequestInfo(br, fixedObj, list);
             }
             return list;
         }

        private static void AddFixedRequestInfo(BinaryReader br, dynamic fixedObj, List<Tuple<string, string, string>> list)
        {
            int length = fixedObj.FieldLength;
            if (fixedObj.ExtendType != FIXED)
            {
                length = br.ReadByte();

            }
            switch (fixedObj.FieldType as string)
            {
                case "nshort":

                    list.Add(new Tuple<string, string, string>(fixedObj.FieldName, fixedObj.FieldDescription,
                                                               IPAddress.NetworkToHostOrder(br.ReadInt16()).ToString()));
                    break;
                case "string":

                    list.Add(new Tuple<string, string, string>(fixedObj.FieldName, fixedObj.FieldDescription,
                                                               SopEncoding.GetString(br.ReadBytes(length))));
                    break;
                case "ipv4":
                    var data = br.ReadBytes(4);
                    list.Add(new Tuple<string, string, string>(fixedObj.FieldName, fixedObj.FieldDescription,
                                                               String.Format("{0}.{1}.{2}.{3}", data[0], data[1], data[2],
                                                                             data[3])));
                    break;
                case "byte":
                    list.Add(new Tuple<string, string, string>(fixedObj.FieldName, fixedObj.FieldDescription,
                                                               string.Format("{0:X2}", br.ReadByte())));
                    break;
                case "nint":
                    list.Add(new Tuple<string, string, string>(fixedObj.FieldName, fixedObj.FieldDescription,
                                                               IPAddress.NetworkToHostOrder(br.ReadInt32()).ToString()));
                    break;
                case "datetime":

                    list.Add(new Tuple<string, string, string>(fixedObj.FieldName, fixedObj.FieldDescription,
                                                               SopEncoding.GetString(br.ReadBytes(length))));
                    break;
                default:
                    throw new InvalidDataException();
                    break;
            }
        }

        private static int GetLength(string s)
        {
            int length;

            bool isFixed = int.TryParse(s, out length);
            if (!isFixed)
            {
                length = -1;
            }
            return length;
        }
    }
}