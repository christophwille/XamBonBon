using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using AT.RKSV.Kassenbeleg;
using Xunit;

namespace test_parseqrcode
{
	public class QrCodeTests
	{
		private const string QRCODE1 =
			"_R1-AT1_fiskaltrust1_ft1C905#105218_2018-02-08T12:37:34_0,00_16,30_0,00_0,00_0,00_xfWUwBw=_7b164a88_6w3fQw5bEog=_irdxIo1TAowB1OzpU+dgeAS887k8AuT09jrcMjZx95xHzbKp5pLQcupkbpZK5UxtDaxj08+8bRO30Y4wxiwonw==";

		[Fact]
		public void TestLookup()
		{
			var test = new ReceiptQrCode(QRCODE1);
			string certSerial = test.CertificateSerial;

			// Lookup needs serial in decimal
			int certificateSerialDecimal = Convert.ToInt32(certSerial, 16);
			Assert.Equal(2065058440, certificateSerialDecimal);

			// Sample A-Trust lookup for above serial
			var result = CertificateLookup.ATrust(certificateSerialDecimal);
			string cert64Encoded = Convert.ToBase64String(result.CertificateBinary);
			Assert.Equal(CERT64ENCODED, cert64Encoded);
		}

		private const string CERT64ENCODED =
			"MIIFUDCCAzigAwIBAgIEexZKiDANBgkqhkiG9w0BAQsFADCBoTELMAkGA1UEBhMCQVQxSDBGBgNVBAoMP0EtVHJ1c3QgR2VzLiBmLiBTaWNoZXJoZWl0c3N5c3RlbWUgaW0gZWxla3RyLiBEYXRlbnZlcmtlaHIgR21iSDEjMCEGA1UECwwaQS1UcnVzdCBSZWdpc3RyaWVya2Fzc2UuQ0ExIzAhBgNVBAMMGkEtVHJ1c3QgUmVnaXN0cmllcmthc3NlLkNBMB4XDTE3MDQwNjExNTI1OVoXDTIyMDQwNjA5NTI1OVowRjELMAkGA1UEBhMCQVQxIDAeBgNVBAMMF1N0ZXVlcm51bW1lciA2NTEyNC85MjQ1MRUwEwYDVQQFEwwzMzE0NDU0MjA4NjEwWTATBgcqhkjOPQIBBggqhkjOPQMBBwNCAAQK9hC2R6EDgUbs8Zrv6m76GbI4eGGmkR5XU78mxsKILA5jBl7ITiDp48hYIBqcRtutVBRNTHKK5/TcTG4te77Jo4IBszCCAa8wfwYIKwYBBQUHAQEEczBxMEYGCCsGAQUFBzAChjpodHRwOi8vd3d3LmEtdHJ1c3QuYXQvY2VydHMvQS1UcnVzdC1SZWdpc3RyaWVya2Fzc2UtQ0EuY2VyMCcGCCsGAQUFBzABhhtodHRwOi8vb2NzcC5hLXRydXN0LmF0L29jc3AwDgYDVR0PAQH/BAQDAgbAMBEGA1UdDgQKBAhNzmNJ7Y8J/zBFBgNVHR8EPjA8MDqgOKA2hjRodHRwOi8vY3JsLmEtdHJ1c3QuYXQvY3JsL0EtVHJ1c3QtUmVnaXN0cmllcmthc3NlLkNBMAkGA1UdEwQCMAAwWAYDVR0gBFEwTzBNBgYqKAARARgwQzBBBggrBgEFBQcCARY1aHR0cDovL3d3dy5hLXRydXN0LmF0L2RvY3MvY3AvQS1UcnVzdC1SZWdpc3RyaWVya2Fzc2UwEwYDVR0jBAwwCoAIQEeeruOQ37YwIgYDVR0RBBswGYEXam9zZWZlZS5hcG90aGVrZUBhb24uYXQwJAYHKigACgELAQQZDBdTdGV1ZXJudW1tZXIgNjUxMjQvOTI0NTANBgkqhkiG9w0BAQsFAAOCAgEAV5qSu2dQDMODf4FWF7y3YLnIAvPvBH7fX/07JSsyoXd3rySS0vZyt0nXn92GkHkObyzPI2KVZMv8FV/XkLXP0L5Alirz3EUewDtW9jlKwTE7F81vkrPwXnEQdN/qVW/zuNjTANMidNTNg1VNOFv4GQKoCFNUSBLK/qfj0gF2HKlx8a1BR6vIwh9IyHX5sLvI/nJGEYocDuGgNATenyZqj+SlJ1XHdBlxBQdn4k9nXegdrgxFmHFCgw9L0X6Zs1sakILII6gNyBMEWvZ8bJi2LSlX8YuurXE1qAyTgUdZtZ+EDsh7tzv0/sGFLSJNOV1tTHOtFinzfdn1q7FUl1MU3ttopr2sW2agtYn3fnJQbtJ/pdxnltfEknPiViCzvi88FSJFWu1Dtm2FljKcBsb4sGWetc4rmbkJeKdxIW+Rb0nzTVcYYH3E9IAKlXDbV+2M95lCwlhwqt1Acl4J8CDXMfc5Z+Z2xHKglkSsU71uMJAk+FePgWuwzFgzCJHPxjs/zbC2d0yIGMBtKrtn9jQHZZ4ZZHOEH9hVTS/rwtkcEyUBMsnw2lcwKry1lb0jLeLkalFsE41mhVh+6nV21zktbVrLxywku5J5iZOEdcw2SSd6kY9ymQMCndFCOqKPyBB/FE2hrqtujUVeEmKd6o9J3SiRbTpCBgeR4tKPlVta0dg=";


		[Fact]
		public void CertPlayGround()
		{
			byte[] certBits = Convert.FromBase64String(CERT64ENCODED);
			var cert = new X509Certificate2(certBits);

			// https://stackoverflow.com/a/38235996/141927
			using (ECDsa ecdsa = cert.GetECDsaPublicKey())
			{
				if (ecdsa != null)
				{
					var test = new ReceiptQrCode(QRCODE1);

					byte[] hash = null;
					using (var algorithm = SHA256.Create())
					{
						string dataAsString = test.GetDataForHashing();
						byte[] data = System.Text.Encoding.UTF8.GetBytes(dataAsString);
						hash = algorithm.ComputeHash(data);
					}

					byte[] signature = Convert.FromBase64String(test.SignatureValue); // MUST be 64 byte array
					
					bool verified = ecdsa.VerifyHash(hash, signature);
				}
			}
			// 3.1 Erstellung der JWS-Signatur (Sicherheitseinrichtung funktionsfähig)
			/* Anmerkungen: Signaturwert:
			 Wird die Signatur manuell erstellt (ohne Verwendung einer JWS-Bibliothek) muss darauf geachtet werden, dass der Signaturwert korrekt 
			 formatiert ist. So verwendet z.B. Java ASN.1 für die Kodierung und die DER Darstellung für die Repräsentation des Signaturwerts. Der 
			 JWS-Standard3 verlangt aber die einfache Konkatenierung der beiden Teilelemente des Signaturwertes R und S: R|S. Im Muster-Code ist 
			 dies in den unten angegebenen Beispielen ersichtlich. */

			// https://openid.net/specs/draft-jones-json-web-signature-04.html#DefiningECDSA
		}
	}
}
