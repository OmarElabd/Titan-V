using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using System.IO.Ports;
using System.Text;

namespace Titan_VI
{
    public class Program
    {
        private static SerialPort btSerialPort;
        private static OutputPort ledPort;
        private const string delim = "|";
        private static string buffer = "";
        private static ZumoMotors motors;

        public static void Main()
        {
            //Ininitalize Components
            motors = new ZumoMotors();
            //HC_SR04 pingSensor = new HC_SR04();
            ledPort = new OutputPort(Pins.ONBOARD_LED, false);
            btSerialPort = new SerialPort(SerialPorts.COM1, 9600, Parity.None, 8, StopBits.One);
            btSerialPort.Open();

            btSerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceived);

            //TestEcho();

            //Continue program
            Thread.Sleep(Timeout.Infinite);
        }

        private static void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            ledPort.Write(true);

            byte[] bytes = new byte[btSerialPort.BytesToRead];
            btSerialPort.Read(bytes, 0, bytes.Length);

            string receivedData = new String(Bytes2Chars(bytes));
            if (receivedData != null && receivedData.Length > 0)
            {
                if (receivedData.IndexOf(delim) > -1)
                {
                    buffer += receivedData.Substring(0, receivedData.IndexOf(delim));
                    ExecuteCommand(buffer);
                    buffer = receivedData.Substring(receivedData.LastIndexOf(delim) + 1);
                }
                else
                {
                    buffer += receivedData;
                }
            }

            ledPort.Write(false);
        }
        
        private static void ExecuteCommand(string command)
        {
            switch (command)
            {
                case "ping": //ping
                    break;
                case "go":
                    motors.SetSpeeds(70, 70);
                    break;
                case "stop":
                    motors.SetSpeeds(0, 0);
                    break;
                case "reverse":
                    motors.SetSpeeds(-70, -70);
                    break;
                default:
                    motors.SetSpeeds(0, 0);
                    break;
            }

            EchoCommand(command);
        }

        private static void WriteMessage(string msg)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(msg + delim);
            btSerialPort.Write(bytes, 0, bytes.Length);
        }

        private static void EchoCommand(string cmd)
        {
            WriteMessage("ackn " + cmd);
        }

        private static char[] Bytes2Chars(byte[] input)
        {
            var output = new char[input.Length];
            for (int counter = 0; counter < input.Length; ++counter)
                output[counter] = (char)input[counter];
            return output;
        }


        //Test Code
        private static void TestEcho()
        {
            while (true)
            {
                ledPort.Write(true);
                WriteMessage("PING");
                Thread.Sleep(250);
                ledPort.Write(false);
                Thread.Sleep(250);
            }
        }
    }
}
