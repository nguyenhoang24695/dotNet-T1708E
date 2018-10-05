using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MUXC = Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SlowVMusic
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private readonly IList<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("home", typeof(Views.HomePage)),
            ("bxh", typeof(MainPage)),
            ("Nhaccuatui", typeof(MainPage)),
            ("Nhacaumy", typeof(MainPage)),
            ("Nhacviet", typeof(MainPage)),
        };

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            // You can also add items in code behind
            NavView.MenuItems.Add(new NavigationViewItemSeparator());
            NavView.MenuItems.Add(new NavigationViewItem
            {
                Content = "My content",
                Icon = new SymbolIcon(Symbol.Folder),
                Tag = "content"
            });
            _pages.Add(("content", typeof(Views.HomePage)));

            ContentFrame.Navigated += On_Navigated;

            // NavView doesn't load any page by default: you need to specify it
            NavView_Navigate("home");

            // Add keyboard accelerators for backwards navigation
            var goBack = new KeyboardAccelerator { Key = VirtualKey.GoBack };
            goBack.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(goBack);

            // ALT routes here
            var altLeft = new KeyboardAccelerator
            {
                Key = VirtualKey.Left,
                Modifiers = VirtualKeyModifiers.Menu
            };
            altLeft.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(altLeft);
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {

            if (args.IsSettingsInvoked)
                ContentFrame.Navigate(typeof(MainPage));
            else
            {
                // Getting the Tag from Content (args.InvokedItem is the content of NavigationViewItem)
                var navItemTag = NavView.MenuItems
                    .OfType<NavigationViewItem>()
                    .First(i => args.InvokedItem.Equals(i.Content))
                    .Tag.ToString();

                NavView_Navigate(navItemTag);
            }
        }

        private void NavView_Navigate(string navItemTag)
        {
            var item = _pages.First(p => p.Tag.Equals(navItemTag));
            ContentFrame.Navigate(item.Page);
        }

        private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            On_BackRequested();
        }

        private void BackInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            On_BackRequested();
            args.Handled = true;
        }

        private bool On_BackRequested()
        {
            if (!ContentFrame.CanGoBack)
                return false;

            // Don't go back if the nav pane is overlayed
            if (NavView.IsPaneOpen &&
                (NavView.DisplayMode == NavigationViewDisplayMode.Compact ||
                NavView.DisplayMode == NavigationViewDisplayMode.Minimal))
                return false;

            ContentFrame.GoBack();
            return true;
        }

        private void On_Navigated(object sender, NavigationEventArgs e)
        {
            NavView.IsBackEnabled = ContentFrame.CanGoBack;

            if (ContentFrame.SourcePageType == typeof(Views.PageLogin))
            {
                // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag
                NavView.SelectedItem = (NavigationViewItem)NavView.SettingsItem;
            }
            else
            {
                var item = _pages.First(p => p.Page == e.SourcePageType);

                NavView.SelectedItem = NavView.MenuItems
                    .OfType<NavigationViewItem>()
                    .First(n => n.Tag.Equals(item.Tag));
            }
        }


        ContentDialog dialogLogin = new ContentDialog();
        ContentDialog dialogRegister = new ContentDialog();

        //Dialog login
        private  async void PageLogin(object sender, RoutedEventArgs e)
        {
            var stackPanel = new StackPanel();

            // Tạo input tài khoản 
            var textBoxName = new TextBox
            {
                Header = "Email",
                Name = "Email",
                Margin =  new Thickness(0,5,0,10)
            };

            // Tạo input pass 
            var textBoxPass = new PasswordBox
            {
                Header = "Password",
                Name = "Password",
            };

            // tạo mới link sang page đăng ký
            var linkSignUp = new HyperlinkButton
            {
                Content = "Đăng ký",
                FontSize = 13,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            // add sự kiện Click vào hyperlink
            linkSignUp.Click += LinkSignUp_Click;


            stackPanel.Children.Add(textBoxName);
            stackPanel.Children.Add(textBoxPass);
            stackPanel.Children.Add(linkSignUp);

            Style btnPrimaryStyle = (Style)App.Current.Resources["myButtonPrimary"];
            Style btnCloseStyle = (Style)App.Current.Resources["myButtonClose"];
            dialogLogin.Title = "Đăng nhập";
            dialogLogin.Content = stackPanel;
            dialogLogin.PrimaryButtonText = "Đăng nhập";
            dialogLogin.CloseButtonText = "Đóng";
            dialogLogin.PrimaryButtonStyle = btnPrimaryStyle;
            dialogLogin.CloseButtonStyle = btnCloseStyle;

            dialogLogin.PrimaryButtonClick += async (s, args) =>
            {
                ContentDialogButtonClickDeferral deferral = args.GetDeferral();
                //await Task.Delay(3000);  //Here I just wait 3 seconds

                // Xu ly nut dang nhap ben trong nay.
                Debug.WriteLine("SlowV");
                deferral.Complete();
            };
            await dialogLogin.ShowAsync();
        }


        //Dialog Register
        private async void LinkSignUp_Click(object sender, RoutedEventArgs e)
        {
            var stackPanel = new StackPanel();
            var sv = new ScrollViewer()
            {
                Content = stackPanel
            };


            // Tạo input tài khoản. 
            var textBoxName = new TextBox
            {
                Header = "Email",
                Name = "Email",
                Margin = new Thickness(50, 5, 50, 10),
                PlaceholderText = "Email của ban.",
            };

            // Tạo input pass. 
            var textBoxPass = new PasswordBox
            {
                Header = "Mật khẩu",
                Name = "Password",
                Margin = new Thickness(50, 5, 50, 10),
                PlaceholderText = "Mật khẩu của bạn.",
            };

            // Tạo mới link sang page đăng nhập
            var linkSignIn = new HyperlinkButton
            {
                Content = "Quay lại đăng nhập",
                Margin = new Thickness(50, 5, 50, 10),
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 13,
            };
            // Sự kiện click của hyperLink.
            linkSignIn.Click += LinkSignIn_Click;


            // Tạo input firstName. 
            var firstName = new TextBox
            {
                Header = "Họ",
                Name = "FirstName",
                PlaceholderText = "Họ của bạn.",
                Margin = new Thickness(50, 5, 50, 10),
            };

            // Tạo input lastName. 
            var lastName = new TextBox
            {
                Header = "Tên",
                Name = "LastName",
                PlaceholderText = "Tên của bạn.",
                Margin = new Thickness(50, 5, 50, 10),
            };

            // Tạo input image Url.
            var imageUrl = new TextBox
            {
                Header = "Url Ảnh",
                Name = "ImageUrl",
                PlaceholderText = "Url ảnh.",
                Margin = new Thickness(50, 5, 50, 10),
            };


            // Tạo thẻ Image để nhét Avatar khi chụp ảnh.
            var Image = new Image
            {
                Name = "MyAvatar",
                Width = 100,
                Height = 100
            };

            // nút Capture_photo chụp ảnh
            var btn_CapturePhoto = new Button
            {
                Content = "Capture Your Photo",
                Margin = new Thickness(50, 5, 50, 10),
            };
            btn_CapturePhoto.Click += Capture_Photo;

            var phone = new TextBox
            {
                Header = "Số điện thoại",
                Margin = new Thickness(50, 5, 50, 10),
                Name = "Phone",
                AcceptsReturn = true,
                PlaceholderText = "Số điện thoại của bạn."
            };

            var address = new TextBox
            {
                Header = "Địa chỉ",
                Margin = new Thickness(50, 5, 50, 10),
                Name = "Address",
                AcceptsReturn = true,
                PlaceholderText = "Địa chỉ của bạn."
            };

            var introduction = new TextBox
            {
                Header = "Giới thiệu",
                Margin = new Thickness(50, 5, 50, 10),
                Name = "Introduction",
                Height = 100,
                AcceptsReturn = true,
                PlaceholderText = "Giới thiệu chút ít về bản thân."
            };

            // UI Giới tính
            var textBlockGender = new TextBlock
            {
                Text = "Giới tính",
                Margin = new Thickness(50, 5, 50, 10),
            };

            var stackPanelGender = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(50, 5, 50, 10),
            };

            var Male = new RadioButton
            {
                Content = "Nam",
                Tag = 1,
            };

            var Female = new RadioButton
            {
                Content = "Nữ",
                Tag = 2,
            };

            var Other = new RadioButton
            {
                Content = "Khác",
                Tag = 3,
                IsChecked = true
            };

            Female.Checked += Select_Gender;
            Male.Checked += Select_Gender;
            Other.Checked += Select_Gender;

            stackPanelGender.Children.Add(Male);
            stackPanelGender.Children.Add(Female);
            stackPanelGender.Children.Add(Other);
            //End Ui giới tính.

            var birthDay = new CalendarDatePicker
            {
                Name = "BirthDay",
                Header = "Ngày sinh",
                Margin = new Thickness(50, 5, 50, 10),
            };
            birthDay.DateChanged += BirthDay_DateChanged;


            // add giao diện vào layout.
            stackPanel.Children.Add(textBoxName);
            stackPanel.Children.Add(textBoxPass);
            stackPanel.Children.Add(firstName);
            stackPanel.Children.Add(imageUrl);
            stackPanel.Children.Add(Image);
            stackPanel.Children.Add(btn_CapturePhoto);
            stackPanel.Children.Add(phone);
            stackPanel.Children.Add(address);
            stackPanel.Children.Add(introduction);
            stackPanel.Children.Add(textBlockGender);
            stackPanel.Children.Add(stackPanelGender);
            stackPanel.Children.Add(birthDay);
            stackPanel.Children.Add(linkSignIn);

            stackPanel.MinWidth = Window.Current.Bounds.Width - 600;
            stackPanel.MaxWidth = Window.Current.Bounds.Height;
            dialogRegister.MinWidth = Window.Current.Bounds.Width;
            dialogRegister.MaxWidth = Window.Current.Bounds.Height;

            Style btnPrimaryStyle = (Style)App.Current.Resources["myButtonPrimary"];
            Style btnCloseStyle = (Style)App.Current.Resources["myButtonClose"];
            dialogRegister.Title = "Đăng ký";
            dialogRegister.Content = sv;
            dialogRegister.CloseButtonText = "Đóng";
            dialogRegister.PrimaryButtonText = "Đăng ký";
            dialogRegister.PrimaryButtonStyle = btnPrimaryStyle;
            dialogRegister.CloseButtonStyle = btnCloseStyle;
            // Ẩn dialog SignIn.
            dialogLogin.Hide();
            await dialogRegister.ShowAsync();
        }

        private void BirthDay_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void Select_Gender(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Capture_Photo(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void LinkSignIn_Click(object sender, RoutedEventArgs e)
        {
            dialogRegister.Hide();
            await dialogLogin.ShowAsync();
        }
    }
}
