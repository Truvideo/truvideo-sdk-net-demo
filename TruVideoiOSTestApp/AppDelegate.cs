namespace TruVideoiOSTestApp;

using System;
using System.Security.Cryptography;
using System.Text;

[Register ("AppDelegate")]
public class AppDelegate : UIApplicationDelegate {
	public override UIWindow? Window {
		get;
		set;
	}

	public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
	{
		Window = new UIWindow (UIScreen.MainScreen.Bounds);

		var vc = new MainViewController ();
		var navigationVC  = new UINavigationController(vc);
		Window.RootViewController = navigationVC;

		Window.MakeKeyAndVisible ();

		return true;
	}
}