using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Popups;


namespace Titan_VI.Controller.Win8
{
    class BluetoothManager
    {
        private IAsyncOperation<RfcommDeviceService> connectService;
        private IAsyncAction connectAction;
        private RfcommDeviceService rfcommService;



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
                if ((uint) ex.HResult == 0x8007048F)
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
            //Feature Not Available on WIN8
        }


        // Note: You can only browse and connect to paired devices!
        public async Task<bool> AttemptEstablishConnection()
        {
            IReadOnlyList<DeviceInformation> deviceInfoCollection = null;

            deviceInfoCollection = await DeviceInformation.FindAllAsync(
                RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort)
            );

            DeviceInformation deviceInfo = deviceInfoCollection.FirstOrDefault(c => c.Name.Contains("HC-07"));

            RfcommDeviceService rfcommService;
            rfcommService = await RfcommDeviceService.FromIdAsync(deviceInfo.Id);

            if (rfcommService == null)
            {
                // Message and open control settings for pairing bluetooth device here.
                return false;
            }

            // Create a socket and connect to the target. 
            socket = new StreamSocket();

            await socket.ConnectAsync(
                        rfcommService.ConnectionHostName,
                        rfcommService.ConnectionServiceName,
                        SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);

            // Create data writer and reader to communicate with the device.
            DataWriter dataWriter = new DataWriter(socket.OutputStream);
            DataReader dataReader = new DataReader(socket.InputStream);
            ListenForMessagesAsync(dataReader);
            return true;
        }

        async public void WriteToDevice(string message)
        {
            var dataBuffer = GetBufferFromByteArray(Encoding.UTF8.GetBytes(message + "|"));
            await socket.OutputStream.WriteAsync(dataBuffer);
        }

        async public void WriteToDevice(string message, StreamSocket streamSocket)
        {
            var dataBuffer = GetBufferFromByteArray(Encoding.UTF8.GetBytes(message + "|"));
            await streamSocket.OutputStream.WriteAsync(dataBuffer);
        }

        private IBuffer GetBufferFromByteArray(byte[] package)
        {
            using (var dw = new DataWriter())
            {
                dw.WriteBytes(package);
                return dw.DetachBuffer();
            }
        }

        private async Task ListenForMessagesAsync(DataReader reader)
        {
            while (reader != null)
            {
                // Read first byte (length of the subsequent message, 255 or less). 
                uint sizeFieldCount = await reader.LoadAsync(1);
                if (sizeFieldCount != 1)
                {
                    // The underlying socket was closed before we were able to read the whole data. 
                    return;
                }

                // Read the message. 
                uint messageLength = reader.ReadByte();
                uint actualMessageLength = await reader.LoadAsync(messageLength);
                if (messageLength != actualMessageLength)
                {
                    // The underlying socket was closed before we were able to read the whole data. 
                    return;
                }
                // Read the message and process it.
                string message = reader.ReadString(actualMessageLength);
            }
        }
    }
}
