using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace XamBonBon.Droid
{
	[Activity(Label = "XamBonBon", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

			global::Xamarin.Forms.Forms.Init(this, bundle);
			ZXing.Net.Mobile.Forms.Android.Platform.Init();

			var app = new XamBonBon.App();

			// https://github.com/Redth/ZXing.Net.Mobile/issues/637#issuecomment-348032927
			ZXing.Mobile.MobileBarcodeScanner.Initialize(Application);

			LoadApplication(app);
		}

		// https://github.com/Redth/ZXing.Net.Mobile
		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			global::ZXing.Net.Mobile.Forms.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}

		// Newer versions (eg 2.3.2)
		//public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		//{
		//	ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		//}
	}
}

