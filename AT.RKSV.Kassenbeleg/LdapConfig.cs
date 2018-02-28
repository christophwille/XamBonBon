using System;
using System.Collections.Generic;
using System.Text;

namespace AT.RKSV.Kassenbeleg
{
	public class LdapConfig
	{
		public LdapConfig(string server, int port, string searchDN, string filterFormat)
		{
			Server = server;
			Port = port;
			SearchDN = searchDN;
			FilterFormat = filterFormat;
		}

		public string Server { get; private set; }
		public int Port { get; private set; }
		public string SearchDN { get; private set; }
		public string FilterFormat { get; private set; }
	}
}
