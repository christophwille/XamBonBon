using System;
using System.Collections.Generic;
using System.Text;
using NeoSmart.Utils;
using Xunit;

namespace test_parseqrcode
{
	public class SpecDocVerificationTests
	{
		[Fact]
		public void EncodingFromBase64UrlToBase64()
		{
			// 4.1 Aufbereitung des maschinenlesbaren Codes (Basis-Code), Seite 47
			string base64UrlEncoded = "J7YC28zquHfHzMpx02TqElbXOTSgXQu5JAA9Xu1Xzzu5p8eUYT-sgmyhzRps5nYyEp5Yh8ATIa9130zmuiACHw";
			var decoded = UrlBase64.Decode(base64UrlEncoded);
			string base64Encoded = Convert.ToBase64String(decoded);

			Assert.Equal("J7YC28zquHfHzMpx02TqElbXOTSgXQu5JAA9Xu1Xzzu5p8eUYT+sgmyhzRps5nYyEp5Yh8ATIa9130zmuiACHw==",
				base64Encoded);
		}
	}
}
