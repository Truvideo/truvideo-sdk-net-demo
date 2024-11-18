using Foundation;
using UIKit;
using System;

namespace TruVideoiOSTestApp
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register ("AppDelegate")]
    public class AppDelegate : UIResponder, IUIApplicationDelegate {
    
        [Export("window")]
        public UIWindow Window { get; set; }

        [Export ("application:didFinishLaunchingWithOptions:")]
        public bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
        {
            Window = new UIWindow(UIScreen.MainScreen.Bounds);

            var vc = new MainViewController();
            var navigationVC = new UINavigationController(vc);
            Window.RootViewController = navigationVC;

            Window.MakeKeyAndVisible();

            return true;
        }

    }
}


