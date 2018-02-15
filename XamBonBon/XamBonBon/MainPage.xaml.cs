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
					BarCodeScanResult.Text = result;
					System.Diagnostics.Debug.WriteLine("Barcode: " + result);

					// Minimal test code
					var qrCode = new ReceiptQrCode(result);
					if (qrCode.IsValid)
					{
						var certificateLookupResult = CertificateLookup.ATrust(qrCode.CertificateSerialAsDecimal);
						if (certificateLookupResult.Found)
						{
							bool verified = qrCode.ValidateSignature(certificateLookupResult.CertificateBinary);
						}
					}
				}
			}
			catch (Exception ex)
			{
				await DisplayAlert("Scan Error", ex.ToString(), "OK");
			}
		}
	}
}
