using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Novell.Directory.Ldap;

namespace AT.RKSV.Kassenbeleg
{
	public static class CertificateLookup
	{
		public static CertificateLookupResult ATrust(int certificateSerialDecimal)
		{
			try
			{
				using (var conn = new LdapConnection())
				{
					conn.Connect("ldap.a-trust.at", 389);
					conn.Bind(null, null);

					var searchBase = "C=AT";
					var filter = $"(eidCertificateSerialNumber={certificateSerialDecimal})";
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
