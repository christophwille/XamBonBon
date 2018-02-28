using System;
using System.Collections.Generic;
using System.Text;
using AT.RKSV.Kassenbeleg;
using Xunit;

namespace test_parseqrcode
{
	public class GeschlossenesSystemTests
	{
		private const string QRCODE1 =
			"_R1-AT0_4820_48200280271447_2018-02-21T11:42:16_0,00_0,00_0,00_1,70_0,00_QEgP4Uo=_U:ATU46674503-01_N4Ks3W2TdlI=_bCgv+uMgv7V4RIy/BhwJWfcLllxXAi42rSdLOXvI4krjsCPN5QMok0xS7zWKgfM/tDKZpkhCSkwyWWw/YUXI1Q==";

		[Fact]
		public void ParseQrCodeGeschlossenesSystemTest()
		{
			var test = new ReceiptQrCode(QRCODE1);
			string Ordnungsbegriff_des_Unternehmers = test.CertificateSerial;

			Assert.True(test.IstGeschlossenesSystem());
			Assert.Equal("U:ATU46674503-01", Ordnungsbegriff_des_Unternehmers);
		}
	}
}
