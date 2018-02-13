using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using XamBonBon.Services;
using ZXing;
using ZXing.Mobile;

// https://developer.xamarin.com/guides/xamarin-forms/application-fundamentals/dependency-service/introduction/
[assembly: Xamarin.Forms.Dependency(typeof(XamBonBon.Droid.Services.QrScanningService))]

namespace XamBonBon.Droid.Services
{
	public class QrScanningService : IQrScanningService
	{
		public QrScanningService()
		{
		}

		public async Task<string> ScanAsync()
		{
			// https://forums.xamarin.com/discussion/comment/284749/#Comment_284749
			var optionsCustom = new MobileBarcodeScanningOptions()
			{
				AutoRotate = false,
				TryInverted = true,
				TryHarder = true,
				PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE },
				// CharacterSet = "ISO-8859-1",
				CameraResolutionSelector = HandleCameraResolutionSelectorDelegate
				// DisableAutofocus = false
			};

			var scanner = new ZXing.Mobile.MobileBarcodeScanner()
			{
				TopText = "QR Code scannen",
				BottomText = "Bitte warten",
				CancelButtonText = "Abbrechen"
			};

			ZXing.Result scanResult = null;

			// https://forums.xamarin.com/discussion/72077/zxing-barcode-reader-autofocus
			Thread autofocusThread = new Thread(new ThreadStart(delegate
			{
				while (scanResult == null)
				{
					scanner.AutoFocus();
					Thread.Sleep(2000);
				}
			}));

			autofocusThread.Start();

			// scanner.AutoFocus();
			try
			{
				scanResult = await scanner.Scan(optionsCustom);
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				if (autofocusThread.IsAlive) { autofocusThread.Abort(); }
			}

			return scanResult.Text;
		}

		// https://stackoverflow.com/questions/38994901/xamarin-ios-zxing-net-mobile-barcode-scanner
		CameraResolution HandleCameraResolutionSelectorDelegate(List<CameraResolution> availableResolutions)
		{
			//Don't know if this will ever be null or empty
			if (availableResolutions == null || availableResolutions.Count < 1)
				return new CameraResolution() { Width = 800, Height = 600 };

			//Debugging revealed that the last element in the list
			//expresses the highest resolution. This could probably be more thorough.
			return availableResolutions[availableResolutions.Count - 1];
		}
	}
}