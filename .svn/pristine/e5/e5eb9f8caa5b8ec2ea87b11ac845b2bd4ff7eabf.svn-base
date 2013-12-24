using System.Collections.Generic;

namespace SopMessageView
{
    public class ContextAccessor : Ast.Exchange.Core.IDataAccessor
    {
        #region Data Members
        private Dictionary<string, object> ContextData = new Dictionary<string, object>();
        #endregion

        public void SetValue(string nodePath, object value)
        {
            ContextData.Add(nodePath, value);
        }

        public object GetValue(string nodePath)
        {
            return ContextData[nodePath];
        }
    }
}