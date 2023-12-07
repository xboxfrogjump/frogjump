using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Gaming.Input;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.WebUI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
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
        
        private async void Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            string urlInput = urlTextBox.Text;
            Uri url;
            if (Uri.TryCreate(urlInput, UriKind.Absolute, out url) && (url.Scheme == Uri.UriSchemeHttp || url.Scheme == Uri.UriSchemeHttps))
            {
                WebView2.Source = url;
            }
            else
            {
                var dialog = new MessageDialog("Invalid URL. Please enter a valid URL with https:// or http://");
                await dialog.ShowAsync();
            }
        }
        private void WebView2_NavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            urlTextBox.Text = sender.Source.AbsoluteUri;
        }

        private DispatcherTimer gamepadTimer;
        private void StartGamepadPolling()
        {
            gamepadTimer = new DispatcherTimer();
            gamepadTimer.Interval = TimeSpan.FromMilliseconds(100); // Adjust the interval as needed
            gamepadTimer.Tick += GamepadTimer_Tick;
            gamepadTimer.Start();
        }
        private bool rightStickPressedRecently = false;
        private void GamepadTimer_Tick(object sender, object e)
        {
            foreach (var gamepad in Gamepad.Gamepads)
            {
                var reading = gamepad.GetCurrentReading();

                if (reading.Buttons.HasFlag(GamepadButtons.RightThumbstick) && !rightStickPressedRecently)
                {
                    TogglePointerMode();
                    rightStickPressedRecently = true;
                    Task.Delay(3000).ContinueWith(_ => rightStickPressedRecently = false); // Adjust delay as needed
                    break; // Exit the loop once the condition is met and action is taken
                }
                if (reading.Buttons.HasFlag(GamepadButtons.LeftThumbstick))
                {
                    urlTextBox.Visibility = Visibility.Visible;
                    GoButton.Visibility = Visibility.Visible;
                }
            }
        }
        private void TogglePointerMode()
        {
            var app = (Application.Current as App);
            if (app.RequiresPointerMode == ApplicationRequiresPointerMode.WhenRequested)
            {
                app.RequiresPointerMode = ApplicationRequiresPointerMode.Auto;
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
            }
            else
            {
                app.RequiresPointerMode = ApplicationRequiresPointerMode.WhenRequested;
                Window.Current.CoreWindow.PointerCursor = null;
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            StartGamepadPolling();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (gamepadTimer != null)
            {
                gamepadTimer.Stop();
            }
        }
    }
}
