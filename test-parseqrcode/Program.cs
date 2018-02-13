using System;
using System.Threading.Tasks;
using AT.RKSV.Kassenbeleg;

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

			// TODO: lookup cert by serial to be able to verify ECDSA sig
		}
	}
}
