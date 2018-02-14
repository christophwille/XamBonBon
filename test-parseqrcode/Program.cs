using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AT.RKSV.Kassenbeleg;
using Novell.Directory.Ldap;

namespace test_parseqrcode
{
	class Program
	{
		static async Task Main(string[] args)
		{
			string qrCode1 =
				"_R1-AT1_fiskaltrust1_ft1C905#105218_2018-02-08T12:37:34_0,00_16,30_0,00_0,00_0,00_xfWUwBw=_7b164a88_6w3fQw5bEog=_irdxIo1TAowB1OzpU+dgeAS887k8AuT09jrcMjZx95xHzbKp5pLQcupkbpZK5UxtDaxj08+8bRO30Y4wxiwonw==";

			var test = new ReceiptQrCode(qrCode1);
			string certSerial = test.CertificateSerial;

			// Lookup needs serial in decimal (sample: 2065058440)
			int certificateSerialDecimal = Convert.ToInt32(certSerial, 16);

			// Sample A-Trust lookup for above serial
			// TODO: get actual cert data (public key)
			try
			{
				using (var conn = new LdapConnection())
				{
					conn.Connect("ldap.a-trust.at", 389);
					conn.Bind(null, null);

					var searchBase = "C=AT";
					var filter = $"(eidCertificateSerialNumber={certificateSerialDecimal})";
					var search = conn.Search(searchBase, LdapConnection.SCOPE_SUB, filter, null, false);

					while (search.hasMore())
					{
						var nextEntry = search.next();
						nextEntry.getAttributeSet();

						var cn = nextEntry.getAttribute("cn").StringValue;
						Console.WriteLine($"cn = {cn}");
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Demystify().ToString());
			}
			
			// TODO: verify signature (ECDSA JWS)
		}
	}
}
