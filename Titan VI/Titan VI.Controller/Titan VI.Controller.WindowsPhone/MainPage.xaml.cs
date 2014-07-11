using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Titan_VI.Controller.WP8;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Titan_VI.Controller
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        BluetoothManager btManager;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            btManager = new BluetoothManager();
        }



        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }


        async private void Button_RequestBluetooth_Click(object sender, RoutedEventArgs e)
        {
            if (btManager.IsBluetoothEnabled())
            {
                MessageDialog dialog = new MessageDialog("Bluetooth is already enabled.");
                await dialog.ShowAsync();
            }
            else
                btManager.RequestBluetooth();
        }

        async private void Button_ConnectBT_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog dialog;

            if (await btManager.AttemptEstablishConnection())
            {
                dialog = new MessageDialog("Connection successfully established.");
            }
            else
            {
                dialog = new MessageDialog("Connection could not be established");
            }

            dialog.ShowAsync();
        }

        private void Button_MotorGo_Click(object sender, RoutedEventArgs e)
        {
            btManager.WriteToDevice("go");
        }

        private void Button_MotorStop_Click(object sender, RoutedEventArgs e)
        {
            btManager.WriteToDevice("stop");
        }

        private void Button_MotorReverse_Click(object sender, RoutedEventArgs e)
        {
            btManager.WriteToDevice("reverse");
        }
    }
}
