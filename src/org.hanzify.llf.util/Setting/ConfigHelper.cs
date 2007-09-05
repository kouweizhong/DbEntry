
#region usings

using System;
using System.Configuration;
using System.Collections.Specialized;

#endregion

namespace org.hanzify.llf.util.Setting
{
	public class ConfigHelper : ConfigHelperBase
	{
        public static readonly ConfigHelper AppSettings = new ConfigHelper("appSettings");
        public static readonly ConfigHelper DefaultSettings = new ConfigHelper("org.hanzify.llf.Settings");

        internal ConfigHelper()
        {
            AppSettings.nvc = new ConfigHelper("appSettings").nvc;
            DefaultSettings.nvc = new ConfigHelper("org.hanzify.llf.Settings").nvc;
        }

        private NameValueCollection nvc = null;

		public ConfigHelper(string SectionName)
		{
            nvc = ConfigReaderProxy.Instance.GetSection(SectionName);
        }

		protected override string GetString(string key)
		{
			return nvc[key];
		}
	}
}