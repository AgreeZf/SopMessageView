using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SopMessageView.Libs
{
    public class MainFrameTradeLogHelper
    {
        private static string GetHexStringFromLog(string log)
        {
            var list = log.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder builder = new StringBuilder();
            foreach (var s in list)//12,47
            {
                builder.AppendLine(s.Substring(12, 47));
            }
            return builder.ToString();

        }
        private static byte[] ProcessHexFromLog(string receivedMsg)
        {
            return receivedMsg.Split(new string[] { " ", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList().
                Select(o => byte.Parse(o, NumberStyles.HexNumber)).ToArray();
        }

        public static byte[] GetHexMessageFromLog(string log)
        {
            return ProcessHexFromLog(GetHexStringFromLog(log));
        }
    }
}