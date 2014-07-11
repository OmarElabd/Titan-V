using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;

namespace Titan_VI
{
    /// <summary>
    /// Library for HC-SR04 Ultrasonic Range Detector
    /// Original Author: John E. Wilson
    /// 
    /// @Author John E. Wilson (04/03/2012)
    /// @Author Matt Cavanagh (02/10/2013)
    /// @Author Omar Elabd (7/2/2014)
    /// </summary>
    public class HC_SR04
    {
        private OutputPort triggerOutPort;
        private InterruptPort echoInPort;
        private long startTick;
        private long stopTick;

        /// <summary>
        /// Conversion Factor for Timer ticks to Centimeters
        /// </summary>
        public double CentimeterConversionFactor { get; set; }

        /// <summary>
        /// Conversion Factor for Timer ticks to Inches
        /// </summary>
        public double InchConversionFactor { get; set; }

        /// <summary>
        /// The system latency (minimum number of ticks)
        /// This number will be subtracted off to find actual sound travel time
        /// </summary>
        public long LatencyTicks { get; set; }


        /// <summary>
        /// Initalize Constructor for HC_SR04 
        /// </summary>
        /// <param name="triggerPin">Netduino output pin that is connected to the HC-SR04 Trigger Pin</param>
        /// <param name="echoPin">Netduino input pin that is connected to the HC-SR04 Echo Pin</param>
        /// <param name="latency">System latency ticks (may vary for different Netduino devices (optional)</param>
        /// <param name="inchCF">Inches Conversion Factor (optional)</param>
        /// <param name="cmCF">Centimeter Conversion Factor (optional)</param>
        public HC_SR04(Cpu.Pin triggerPin, Cpu.Pin echoPin, long latency = 6200L, double inchCF = 1440.0, double cmCF = 500.0)
        {
            triggerOutPort = new OutputPort(triggerPin, false);
            echoInPort = new InterruptPort(echoPin, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeLow);
            echoInPort.OnInterrupt += new NativeEventHandler(interIn_OnInterrupt); //Register the echo event (when we receive a pulse back)

            LatencyTicks = latency;
            InchConversionFactor = inchCF;
            CentimeterConversionFactor = cmCF;
        }


        /// <summary>
        /// Trigger a sensor reading
        /// Convert ticks to distance using TicksToInches below
        /// </summary>
        /// <returns>Number of ticks it takes to get back sonic pulse</returns>
        public long Ping()
        {
            // Reset Sensor
            triggerOutPort.Write(true);
            Thread.Sleep(1);

            // Start Clock
            stopTick = 0L;
            startTick = System.DateTime.Now.Ticks;
            // Trigger Sonic Pulse
            triggerOutPort.Write(false);

            // Wait 1/20 second (this could be set as a variable instead of constant)
            Thread.Sleep(40);

            if (stopTick > 0L)
            {
                // Calculate Difference
                long elapsed = stopTick - startTick;

                // Subtract out fixed overhead (interrupt lag, etc.)
                elapsed -= LatencyTicks;
                if (elapsed < 0L)
                {
                    elapsed = 0L;
                }

                // Return elapsed ticks
                return elapsed;
            }

            // Sonic pulse wasn't detected within 1/20 second
            return -1L;
        }

        /// <summary>
        /// This interrupt will trigger when detector receives back reflected sonic pulse       
        /// </summary>
        /// <param name="data1">Not used</param>
        /// <param name="data2">Not used</param>
        /// <param name="time">Transfer to endTick to calculated sound pulse travel time</param>
        void interIn_OnInterrupt(uint data1, uint data2, DateTime echoTime)
        {
            // Save the ticks when pulse was received back
            stopTick = echoTime.Ticks;
        }

        /// <summary>
        /// Convert timer ticks to Meters
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public double ConvertTicksToInches(long ticks)
        {
            return (double)ticks / InchConversionFactor;
        }

        /// <summary>
        /// Convert timer ticks to Centimeters
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public double ConvertTicksToCm(long ticks)
        {
            return (double)ticks / CentimeterConversionFactor;
        }
    }
}
