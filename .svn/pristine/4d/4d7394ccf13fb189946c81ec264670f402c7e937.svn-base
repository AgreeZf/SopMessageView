namespace SopMessageView.Libs
{
    public class Field:IMessageNode
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public string FieldDescription { get; set; }
        public int StartPositionInMsg { get; set; }
        public int EndPositionInMsg { get; set; }
        public string HexValue { get; set; }
        public override string ToString()
        {
            return string.Format("FieldName:{0},FieldDescription:{1},FieldValue:{2}", FieldName, FieldDescription,
                                 FieldValue);
        }


        public string Text
        {
            get
            {
                return string.Format("FieldName:{0},FieldDescription:{1},FieldValue:{2},HexValue:{5},StartPosition:0x{3:X4},EndPosition:0x{4:X4}", FieldName, FieldDescription,
                               FieldValue,StartPositionInMsg,EndPositionInMsg,HexValue);
            }
        }

    }
}