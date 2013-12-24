using System.Collections.Generic;
using System.Text;

namespace SopMessageView.Libs
{
    public class Table:IMessageNode
    {
        public string TableName { get; set; }
        public int TableRow { get; set; }
        public int TableCol { get; set; }
        public int StartPositionInMsg { get; set; }
        public int EndPositionInMsg { get; set; }
        public IEnumerable<Field> Fields { get; set; }
        public override string ToString()
        {
            StringBuilder builder=new StringBuilder();
            builder.AppendFormat("TableName:{0},TableRow:{1},TableCol:{2}", TableName, TableRow, TableCol);
            builder.AppendLine();
            foreach (var field in Fields)
            {
                builder.AppendLine(field.ToString());
            }
            return builder.ToString();
        }

        public string Text
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("TableName:{0},TableRow:{1},TableCol:{2},StartPosition:0x{3:X4},EndPosition:0x{4:X4}", TableName, TableRow, TableCol,StartPositionInMsg,EndPositionInMsg);
                builder.AppendLine("表格开始");

                for (int i = 0; i < TableRow; i++)
                {
                    builder.AppendFormat("第{0}行", i);
                    builder.AppendLine();
                }
                foreach (var field in Fields)
                {
                    builder.AppendLine(field.ToString());
                    
                }
                builder.AppendFormat("表格{0}结束", TableName);
                builder.AppendLine();
                return builder.ToString();
            }
        }
    }
}