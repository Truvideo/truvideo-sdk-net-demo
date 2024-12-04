using System;
using UIKit;
using Foundation;
using CoreGraphics;
using System.Collections.Generic;

using TruvideoCameraiOS;
using TruvideoMediaiOS;
using TruvideoVideoiOS;

namespace TruVideoiOSTestApp
{
    public class MainViewController : UIViewController, IUITableViewDelegate, IUITableViewDataSource
    {
        private UITableView tableView;
        private UITextView textView;
        private List<string> items;
        private Dictionary<int, UIImage> itemIcons;

        private NSArray<NSString> selectedMedia = new NSArray<NSString>();

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.White;

            items = new List<string> { "Authenticate", "Show Camera", "Generate Thumbnail", "Upload" };

            itemIcons = new Dictionary<int, UIImage>
            {
                { 0, UIImage.GetSystemImage("lock") }, // Replace with your actual image names
                { 1, UIImage.GetSystemImage("camera") },
                { 2, UIImage.GetSystemImage("photo.circle") },
                { 3, UIImage.GetSystemImage("arrow.up.circle") }
            };

            tableView = new UITableView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Delegate = this,
                DataSource = this,
                RowHeight = 60,
                BackgroundColor = UIColor.Clear,
                Bounces = false,
                AllowsSelection = true
            };

            tableView.RegisterClassForCellReuse(typeof(UITableViewCell), "cell");

            textView = new UITextView
            {
                Text = "Logs...\n",
                TextColor = UIColor.Black,
                Font = UIFont.SystemFontOfSize(16),
                Editable = false,
                ScrollEnabled = true,
                BackgroundColor = UIColor.White,
                DataDetectorTypes = UIDataDetectorType.Link,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            View.AddSubviews(tableView, textView);

            NSLayoutConstraint.ActivateConstraints(new[]
            {
                tableView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor),
                tableView.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor),
                tableView.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor),
                tableView.HeightAnchor.ConstraintEqualTo(180)
            });

            NSLayoutConstraint.ActivateConstraints(new[]
            {
                textView.TopAnchor.ConstraintEqualTo(tableView.BottomAnchor, 10),
                textView.LeadingAnchor.ConstraintEqualTo(View.LeadingAnchor, 20),
                textView.TrailingAnchor.ConstraintEqualTo(View.TrailingAnchor, -20),
                textView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor, -20)
            });
        }

        #region UITableViewDataSource

        public nint RowsInSection(UITableView tableView, nint section)
        {
            return items.Count;
        }

        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("cell", indexPath);

            cell.TextLabel.Text = items[indexPath.Row];
            cell.TextLabel.TextAlignment = UITextAlignment.Left;
            cell.TextLabel.TextColor = UIColor.Black;
            cell.TextLabel.Font = UIFont.SystemFontOfSize(18);
            cell.BackgroundColor = UIColor.White;

            cell.AccessoryView = new UIImageView(itemIcons[indexPath.Row])
            {
                ContentMode = UIViewContentMode.ScaleAspectFit,
                Frame = new CGRect(0, 0, 24, 24)
            };

            return cell;
        }

        #endregion

        #region UITableViewDelegate

        [Export("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            switch (indexPath.Row)
            {
                case 0:
                    NavigationController?.PushViewController(new AuthenticateViewController(), true);
                    break;
                case 1:
                    Action<NSArray<NSString>> ShowCameraHandler = (paths) =>
                    {
                        if (paths.Count > 0)
                        {
                            textView.Text += "Selected media paths:\n";
                            textView.Text += paths + "\n";
                            selectedMedia = paths;
                        }
                        else
                        {
                            textView.Text += "No selected Media\n";
                        }
                    };

                    TruvideoCameraSdk.Shared.ShowCameraIn(this, ShowCameraHandler);
                    break;
                case 2:
                    if (selectedMedia.Count > 0)
                    {
                        NSUrl url = NSUrl.FromFilename(selectedMedia[0]);
                        ThumbnailRequest request = new ThumbnailRequest(url, 0.5, null, null);
                        Action<NSUrl, NSError> ThumbnailHandler = (response, error) =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                if (error != null)
                                {
                                    textView.Text += error.LocalizedDescription;
                                }
                                else
                                {
                                    textView.Text += "Generated thumbnail url:\n";
                                    textView.Text += response + "\n";
                                }
                            });
                        };
                        TruvideoVideoSdk.Shared.GenerateThumbnailWithRequest(request, ThumbnailHandler);
                    }
                    break;
                case 3:
                    if (selectedMedia.Count > 0)
                    {
                        textView.Text += "Uploading ....\n";
                        Action<MediaResponse, NSError> UploadMediaHandler = (response, error) =>
                        {
                            if (error != null)
                            {
                                textView.Text += error.LocalizedDescription;
                            }
                            else
                            {
                                textView.Text += "Uploaded media url:\n";
                                textView.Text += response.UploadedFileURL.AbsoluteString + "\n";
                            }
                        };
                        TruvideoMediaSdk.Shared.UploadWithPath(selectedMedia[0], UploadMediaHandler);
                    }
                    else
                    {
                        textView.Text += "No selected media from camera";
                    }
                    break;
            }
            tableView.DeselectRow(indexPath, true);
        }

        #endregion
    }

}
