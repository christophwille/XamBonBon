using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AT.RKSV.Kassenbeleg;
using Xamarin.Forms;
using XamBonBon.Services;

namespace XamBonBon
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		private async void ScanBon_Clicked(object sender, EventArgs e)
		{
			try
			{
				var scanner = DependencyService.Get<IQrScanningService>();
				var result = await scanner.ScanAsync();
				if (result != null)
				{
					System.Diagnostics.Debug.WriteLine("QR: " + result);

					StringBuilder stb = new StringBuilder();
					stb.AppendLine($"QR: {result}");

					var qrCode = new ReceiptQrCode(result);
					if (qrCode.IsValid)
					{
						stb.AppendLine($"Cipher Suite: {qrCode.CipherSuite}");
						stb.AppendLine($"Cert Id: {qrCode.CertificateSerialAsDecimal}");
						stb.AppendLine($"Datum: {qrCode.Date}");
						stb.AppendLine($"Beträge: {qrCode.BetragSatzNormal} / {qrCode.BetragSatzErmaessigt1} / {qrCode.BetragSatzErmaessigt2} / {qrCode.BetragSatzNull} / {qrCode.BetragSatzBesonders}");

						var certificateLookupResult = CertificateLookup.Lookup(qrCode);

						if (certificateLookupResult.Found)
						{
							bool verified = qrCode.ValidateSignatureBouncyCastle(certificateLookupResult.CertificateBinary);
							stb.AppendLine($"Ergebnis Validierung Signatur: {verified}");
						}
						else
						{
							stb.AppendLine($"Fehler: Zertifikat nicht gefunden, {certificateLookupResult.ErrorMessage}");
						}
					}
					else
					{
						stb.AppendLine("Fehler: QR Code ungültig");
					}

					VerificationResult.Text = stb.ToString();
				}
			}
			catch (Exception ex)
			{
				await DisplayAlert("Scan Error", ex.ToString(), "OK");
			}
		}
	}
}
