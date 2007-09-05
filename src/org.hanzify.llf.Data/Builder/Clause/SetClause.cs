
#region usings

using System;
using System.Text;
using org.hanzify.llf.util.Text;
using org.hanzify.llf.Data.Dialect;
using org.hanzify.llf.Data.SqlEntry;

#endregion

namespace org.hanzify.llf.Data.Builder.Clause
{
	[Serializable]
	public class SetClause : KeyValueCollection, IClause
	{
		public SetClause() {}

		public string ToSqlText(ref DataParamterCollection dpc, DbDialect dd)
		{
			StringBuilder sb = new StringBuilder("Set ");
			foreach ( KeyValue kv in this )
			{
				string dpStr;
                if (DataSetting.UsingParamter)
                {
                    dpStr = string.Format(dd.ParamterPrefix + "{0}_{1}", kv.Key, dpc.Count);
                    DataParamter dp = new DataParamter(dpStr, kv.Value, kv.ValueType);
                    dpc.Add(dp);
                }
                else
                {
                    dpStr = DataTypeParser.ParseToString(kv.Value, dd);
                }

				sb.Append( dd.QuoteForColumnName(kv.Key) );
				sb.Append( "=" );
				sb.Append( dpStr );
				sb.Append(",");
			}
			try
			{
				return StringHelper.GetStringLeft(sb.ToString());
			}
			catch ( ArgumentOutOfRangeException )
			{
				return "";
			}
		}
	}
}