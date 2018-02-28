using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Novell.Directory.Ldap;

namespace AT.RKSV.Kassenbeleg
{
	public enum Vda
	{
		ATrust = 1,
		Globaltrust = 2,
		Primesign = 3
	}

	public static class CertificateLookup
	{
		public static Dictionary<Vda,LdapConfig> LdapConfigs = new Dictionary<Vda, LdapConfig>()
		{
			// http://www.a-trust.at/Company-Profile/ scroll to bottom
			{ Vda.ATrust, new LdapConfig("ldap.a-trust.at", 389, "C=AT", "(eidCertificateSerialNumber={0})") },
			// http://www.globaltrust.eu/php/cms_monitor.php?q=PUB&s=81391ttp
			{ Vda.Globaltrust, new LdapConfig("ldap.globaltrust.eu)", 389, "C=AT", "(serialNumber={0})") },
			// https://tc.prime-sign.com/policies/PrimeSign_RKSV_CPS_v1.0.0.pdf Sektion 2.1.1 (p17)
			{ Vda.Primesign, new LdapConfig("ldap.tc.prime-sign.com", 389, "cn=PrimeSign RKSV Signing CA,o=PrimeSign GmbH,dc=tc,dc=prime-sign,dc=com", "(uniqueIdentifier={0})") },
		};

		private const string VdaATrustCipherSuite = "R1-AT1";
		private const string VdaGlobalsignTrustCipherSuite = "R1-AT2";
		private const string VdaPrimesignCipherSuite = "R1-AT3";

		public static bool IsSupportedCipherSuite(string cipherSuite)
		{
			return (0 == String.Compare(VdaATrustCipherSuite, cipherSuite, StringComparison.InvariantCultureIgnoreCase)
			        || 0 == String.Compare(VdaGlobalsignTrustCipherSuite, cipherSuite, StringComparison.InvariantCultureIgnoreCase)
			        || 0 == String.Compare(VdaPrimesignCipherSuite, cipherSuite, StringComparison.InvariantCultureIgnoreCase));
		}

		public static CertificateLookupResult Lookup(ReceiptQrCode qrCode)
		{
			if (!IsSupportedCipherSuite(qrCode.CipherSuite))
				return new CertificateLookupResult("cipher suite not implemented");

			CertificateLookupResult certificateLookupResult = null;
			switch (qrCode.CipherSuite)
			{
				case VdaATrustCipherSuite:
					certificateLookupResult = CertificateLookup.ATrust(qrCode.CertificateSerialAsDecimal);
					break;
				case VdaGlobalsignTrustCipherSuite:
					certificateLookupResult = CertificateLookup.Globaltrust(qrCode.CertificateSerialAsDecimal);
					break;
				case VdaPrimesignCipherSuite:
					certificateLookupResult = CertificateLookup.Primesign(qrCode.CertificateSerialAsDecimal);
					break;
			}

			return certificateLookupResult;
		}

		public static CertificateLookupResult ATrust(long certificateSerialDecimal)
		{
			return Lookup(certificateSerialDecimal, LdapConfigs[Vda.ATrust]);
		}

		public static CertificateLookupResult Globaltrust(long certificateSerialDecimal)
		{
			return Lookup(certificateSerialDecimal, LdapConfigs[Vda.Globaltrust]);
		}

		public static CertificateLookupResult Primesign(long certificateSerialDecimal)
		{
			return Lookup(certificateSerialDecimal, LdapConfigs[Vda.Primesign]);
		}

		private static CertificateLookupResult Lookup(long certificateSerialDecimal, LdapConfig config)
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
}
