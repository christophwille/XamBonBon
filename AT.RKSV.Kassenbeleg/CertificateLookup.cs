using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Novell.Directory.Ldap;

namespace AT.RKSV.Kassenbeleg
{
	public enum Vda
	{
		ATrust,
		Globaltrust,
		Primesign
	}

	public class LdapConfig
	{
		public LdapConfig(string server, int port, string searchDN, string filterFormat)
		{
			Server = server;
			Port = port;
			SearchDN = searchDN;
			FilterFormat = filterFormat;
		}

		public string Server { get; set; }
		public int Port { get; set; }
		public string SearchDN { get; set; }
		public string FilterFormat { get; set; }
	}

	public static class CertificateLookup
	{
		public static Dictionary<Vda,LdapConfig> LdapConfigs = new Dictionary<Vda, LdapConfig>()
		{
			{ Vda.ATrust, new LdapConfig("ldap.a-trust.at", 389, "C=AT", "(eidCertificateSerialNumber={0})") },
			{ Vda.Primesign, new LdapConfig("ldap.tc.prime-sign.com", 389, "cn=PrimeSign RKSV Signing CA,o=PrimeSign GmbH,dc=tc,dc=prime-sign,dc=com", "(uniqueIdentifier={0})") },
		};

		public static CertificateLookupResult ATrust(long certificateSerialDecimal)
		{
			return Lookup(certificateSerialDecimal, LdapConfigs[Vda.ATrust]);
		}

		public static CertificateLookupResult Primesign(long certificateSerialDecimal)
		{
			return Lookup(certificateSerialDecimal, LdapConfigs[Vda.Primesign]);
		}

		public static CertificateLookupResult Lookup(long certificateSerialDecimal, LdapConfig config)
		{
			try
			{
				using (var conn = new LdapConnection())
				{
					conn.Connect(config.Server, config.Port);
					conn.Bind(null, null);

					var searchBase = config.SearchDN;
					var filter = String.Format(config.FilterFormat, certificateSerialDecimal);
					var search = conn.Search(searchBase, LdapConnection.SCOPE_SUB, filter, null, false);

					// We only look at the first result
					if (search.hasMore())
					{
						var nextEntry = search.next();
						nextEntry.getAttributeSet();

						var cn = nextEntry.getAttribute("cn").StringValue;
						var cert = nextEntry.getAttribute("usercertificate;binary").ByteValue;

						return new CertificateLookupResult(cn, (byte[])(object)cert);
					}
					else
					{
						return new CertificateLookupResult("Not found");
					}
				}
			}
			catch (Exception e)
			{
				Trace.WriteLine(e.ToString());
				return new CertificateLookupResult(e.Message);
			}
		}
	}

	public class CertificateLookupResult
	{
		public CertificateLookupResult(string errorMessage)
		{
			ErrorMessage = errorMessage;
			Found = false;
		}

		public CertificateLookupResult(string cn, byte[] certBin)
		{
			CN = cn;
			CertificateBinary = certBin;
			Found = true;
		}

		public bool Found { get; private set; }
		public string ErrorMessage { get; private set; }

		public string CN { get; private set; }
		public byte[] CertificateBinary { get; private set; }
	}
}
