using System;
using System.Collections.Generic;
using System.Text;
using AT.RKSV.Kassenbeleg;
using Xunit;

namespace test_parseqrcode
{
	public class PrimesignTests
	{
		private const string QRCODE1 =
			"_R1-AT3_NovaTouch3_ft9959#38164_2018-02-26T10:02:23_0,00_0,00_0,00_21,20_0,00_mrcVvQE=_1bed73b390d58_Pf0Q7tXusVg=_wq2L/wZ/ueq1R1IOv6KUjtklGQKSdHlLt8rYA52lAvFq4SIylOGz42q9P16GVUYdLrA5lxEI5ROnvwCOmL0ZjA==";

		[Fact]
		public void LookupPrimesignCertificateTest()
		{
			var test = new ReceiptQrCode(QRCODE1);
			string certSerial = test.CertificateSerial;

			// Lookup needs serial in decimal
			long certificateSerialDecimal = Convert.ToInt64(certSerial, 16);
			Assert.Equal(491306597551448, certificateSerialDecimal);

			// Sample Primesign lookup for above serial
			var result = CertificateLookup.Primesign(certificateSerialDecimal);
			string cert64Encoded = Convert.ToBase64String(result.CertificateBinary);
			Assert.Equal(CERT64ENCODED, cert64Encoded);
		}

		[Fact]
		public void ValidatePrimesignSignatureTest()
		{
			byte[] certBits = Convert.FromBase64String(CERT64ENCODED);

			var test = new ReceiptQrCode(QRCODE1);
			bool ok = test.ValidateSignatureBouncyCastle(certBits);
			Assert.True(ok);
		}

		private const string CERT64ENCODED =
			"MIIEkjCCAnqgAwIBAgIHAb7XOzkNWDANBgkqhkiG9w0BAQsFADBKMQswCQYDVQQGEwJBVDEXMBUGA1UEChMOUHJpbWVTaWduIEdtYkgxIjAgBgNVBAMTGVByaW1lU2lnbiBSS1NWIFNpZ25pbmcgQ0EwHhcNMTcwMjI2MTAyNzUyWhcNMjMwMjI2MTAyNzUyWjBYMQswCQYDVQQGEwJBVDEvMC0GA1UECgwmS29uZGl0b3JlaS1LYWZmZWUgWmF1bmVyIEdtYkggJiBDby4gS0cxGDAWBgNVBAMTD1VJRCBBVFU1MjYwMDMwNTBZMBMGByqGSM49AgEGCCqGSM49AwEHA0IABAuzunV/boVk2Rl30sZBQl9qVyo4kzJhISVitGZoAktE6lzU82SULpV6wk5ZZu+8SndcxtErb1ZW6MTZwfpgH9CjggE4MIIBNDAOBgNVHQ8BAf8EBAMCBsAwRgYIKwYBBQUHAQEEOjA4MDYGCCsGAQUFBzAChipodHRwOi8vdGMucHJpbWUtc2lnbi5jb20vY2VydHMvcmtzdi1jYS5jZXIwHwYDVR0jBBgwFoAUmqNK9QdVVZSTZi5ZN3gtCnEE/DAwQAYDVR0gBDkwNzA1BgYqKAAnAQIwKzApBggrBgEFBQcCARYdaHR0cDovL3RjLnByaW1lLXNpZ24uY29tL2Nwcy8wHAYHKigACgELAQQRDA9VSUQgQVRVNTI2MDAzMDUwOgYDVR0fBDMwMTAvoC2gK4YpaHR0cDovL3RjLnByaW1lLXNpZ24uY29tL2NybHMvcmtzdi1jYS5jcmwwHQYDVR0OBBYEFL8z12FkECg8+VS/Xd+Ei6XsAiDZMA0GCSqGSIb3DQEBCwUAA4ICAQC/7GzHCIimySYPdSjS4wbU32DEEDiD6SKVbU8gixoqbDypobJZngc2vZ1NQp3qTT+kFJLPPplDEnkW20ZAGT9/JMzDkl48lmfxP3SIbRWi+oVzKJhGRqBJ+mQ9Nc4rNfEeuQqxW3IIedvwUjCD+ntHfEd/mvZD58sijYxULvaMjGw2tyej+GQdrVohrwoAtCf3cX6MhOGRcN4Q5i6YC1kimAvqJo9+zo2jJ0MXipn6A+Y5YJkHpyQogUS8FQW5dPkU/0bCo3ulCuPjcA/oelBFeMkntf277YcMihPQWFXOBBTU/trePja7efz4yMaucI73H61X52yI1N6s8N9eed/DG0Pscuq/tTl+28fqjE65XBRcsjP2Kci6rz4y1IH6KllLgdcs+28tXERdDVxiMDYx7Azfxs3/DZv5ks17sOdHYnGb57qZ1EMWhv283LWrCH7GRCi6PlcJ7pSLU6XtZSVDC28pxDfWPdjCc68Q9ARxdJMoINEDWtSxk3QVWijFGhNBHi6vgjf1dwL/dYJ05H/MmnkuBW7WCFFhi4FFBDDKrU6H5Dr0coUIXzEmuD0HqrNjfCKKHR88Ym6g71N7+JioiJ7PNmBu+6nyobEPemfPTSHpt0TghRPvRwh0eO9sUuMDq8eFDfPOiQNmEsO+I6DneU0HgB22ENmiuX9kuAleUw==";

	}
}
