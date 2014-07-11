using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;


namespace Titan_VI.Controller.WP8
{
    class BluetoothManager
    {
        #region Events
        //When an Exception Occurs
        public delegate void AddOnExceptionOccuredDelegate(object sender, Exception ex);
        public event AddOnExceptionOccuredDelegate ExceptionOccured;
        private void OnExceptionOccuredEvent(object sender, Exception ex)
        {
            if (ExceptionOccured != null)
                ExceptionOccured(sender, ex);
        }

        //When a message is received from the Device
        public delegate void AddOnMessageReceivedDelegate(object sender, string message);
        public event AddOnMessageReceivedDelegate MessageReceived;
        private void OnMessageReceivedEvent(object sender, string message)
        {
            if (MessageReceived != null)
                MessageReceived(sender, message);
        }
        #endregion

        private StreamSocket socket;

        public bool IsBluetoothEnabled()
        {
            try
            {
                //var peers = await PeerFinder.FindAllPeersAsync();
                PeerFinder.Start();
                var peers = PeerFinder.FindAllPeersAsync();

                return true;
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x8007048F)
                {
                    return false;
                }
            }
            finally
            {
                PeerFinder.Stop();
            }

            return false;

        }

        async public void RequestBluetooth()
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings-bluetooth:///"));
        }

        // Note: You can only browse and connect to paired devices!
        public async Task<bool> AttemptEstablishConnection()
        {
            // Configure PeerFinder to search for all paired devices.

            PeerFinder.AlternateIdentities["Bluetooth:Paired"] = "";
            var pairedDevices = await PeerFinder.FindAllPeersAsync();

            if (pairedDevices.Count == 0)
            {
                Debug.WriteLine("No paired devices were found.");
                return false;
            }
            else
            {
                // Select a paired device. TODO: Set to "linov"
                PeerInformation selectedDevice = pairedDevices[0];

                // Attempt a connection
                socket = new StreamSocket();
                await socket.ConnectAsync(selectedDevice.HostName, "1");
                return true;
            }

           //PeerFinder.Stop();
        }

        /// <summary>
        /// Receive messages from the Arduino through bluetooth.
        /// </summary>
        //private async void ReceiveMessages(object sender, DoWorkEventArgs e)
        //{
        //    try
        //    {
        //        while (true)
        //        {
        //            // Read first byte (length of the subsequent message, 255 or less). 
        //            uint sizeFieldCount = await dataReader.LoadAsync(1);
        //            if (sizeFieldCount != 1)
        //            {
        //                // The underlying socket was closed before we were able to read the whole data. 
        //                return;
        //            }

        //            // Read the message. 
        //            uint messageLength = dataReader.ReadByte();
        //            uint actualMessageLength = await dataReader.LoadAsync(messageLength);
        //            if (messageLength != actualMessageLength)
        //            {
        //                // The underlying socket was closed before we were able to read the whole data. 
        //                return;
        //            }
        //            // Read the message and process it.
        //            string message = dataReader.ReadString(actualMessageLength);
        //            MessageReceived(message);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex.Message);
        //    }

        async public void WriteToDevice(string message)
        {
            var dataBuffer = GetBufferFromByteArray(Encoding.UTF8.GetBytes(message + "|"));
            await socket.OutputStream.WriteAsync(dataBuffer);
        }

        private IBuffer GetBufferFromByteArray(byte[] package)
        {
            using (DataWriter dw = new DataWriter())
            {
                dw.WriteBytes(package);
                return dw.DetachBuffer();
            }
        }
    }
}
