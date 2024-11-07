using System;
using UIKit;
using Foundation;
using CoreGraphics;
using TruvideoCoreiOS;

namespace TruVideoiOSTestApp
{
    public class AuthenticateViewController : UIViewController
    {
        private UITextField apiKeyTextField;
        private UITextField secretTextField;
        private UITextField externalTextField;
        private UIButton authenticateButton;
        private UIActivityIndicatorView activityIndicator;

        Action<bool, NSError> IsAuthenicatedHandler;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.White;

            apiKeyTextField = CreateTextField("ApiKey");
            secretTextField = CreateTextField("Secret");
            externalTextField = CreateTextField("External Id");

            authenticateButton = new UIButton(UIButtonType.System);

            authenticateButton.TitleLabel.Font = UIFont.BoldSystemFontOfSize(18);
            authenticateButton.TranslatesAutoresizingMaskIntoConstraints = false;

            activityIndicator = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Large)
            {
                HidesWhenStopped = true,
                Color = UIColor.Blue,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            var stackView = new UIStackView(new UIView[] { activityIndicator, apiKeyTextField, secretTextField, externalTextField, authenticateButton })
            {
                Axis = UILayoutConstraintAxis.Vertical,
                Distribution = UIStackViewDistribution.FillProportionally,
                Spacing = 20,
                Alignment = UIStackViewAlignment.Fill,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            View.AddSubview(stackView);

            NSLayoutConstraint.ActivateConstraints(new[]
            {
                stackView.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor),
                stackView.CenterYAnchor.ConstraintEqualTo(View.CenterYAnchor),
                stackView.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, 0.8f)
            });

            IsAuthenicatedHandler = (isAuthenticated, Error) =>
                {
                    InvokeOnMainThread(() =>
                        {
                            if (Error != null)
                            {
                                authenticateButton.SetTitle(Error.LocalizedDescription, UIControlState.Normal);
                            }
                            else if (isAuthenticated)
                            {
                                authenticateButton.SetTitle("Authenticated", UIControlState.Normal);
                                authenticateButton.Enabled = false;
                                apiKeyTextField.Hidden = true;
                                secretTextField.Hidden = true;
                                externalTextField.Hidden = true;
                            }
                            else
                            {
                                authenticateButton.SetTitle("Authenticate", UIControlState.Normal);
                                authenticateButton.TouchUpInside += AuthenticateButtonTapped;
                            }
                        });
                };
            TruvideoCore.Shared.IsAuthenticatedWithCompletionHandler(IsAuthenicatedHandler);
        }

        private UITextField CreateTextField(string placeholder)
        {
            return new UITextField
            {
                Placeholder = placeholder,
                BorderStyle = UITextBorderStyle.RoundedRect,
                Font = UIFont.SystemFontOfSize(16),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
        }

        private void AuthenticateButtonTapped(object sender, EventArgs e)
        {
            Action<NSError> AuthenticateHandler = (Error) =>
            {
                InvokeOnMainThread(() =>
                    {
                        if (Error != null)
                        {
                            activityIndicator.StopAnimating();
                            authenticateButton.Enabled = true;
                        }
                        else
                        {
                            Action<NSError> InitAuthHandler = (Error) =>
                                {
                                    InvokeOnMainThread(() =>
                                            {
                                                activityIndicator.StopAnimating();
                                                authenticateButton.Enabled = true;
                                                TruvideoCore.Shared.IsAuthenticatedWithCompletionHandler(IsAuthenicatedHandler);
                                            });
                                };
                            TruvideoCore.Shared.InitAuthenticationWithCompletionHandler(InitAuthHandler);
                        }
                    });
            };

            activityIndicator.StartAnimating();
            authenticateButton.Enabled = false;
            TruvideoCore.Shared.AuthenticateWithApiKey(apiKeyTextField.Text, secretTextField.Text, externalTextField.Text, AuthenticateHandler);
        }
    }
}
