using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace Titan_VI
{
    public class ZumoMotors
    {
        private readonly Cpu.Pin PWM_L_PIN = Pins.GPIO_PIN_D10;
        private readonly Cpu.Pin PWM_R_PIN = Pins.GPIO_PIN_D9;
        private readonly Cpu.Pin DIR_L_PIN = Pins.GPIO_PIN_D8;
        private readonly Cpu.Pin DIR_R_PIN = Pins.GPIO_PIN_D7;


        private SecretLabs.NETMF.Hardware.PWM PWM_L;
        private SecretLabs.NETMF.Hardware.PWM PWM_R;
        private OutputPort DIR_L;
        private OutputPort DIR_R;

        private static bool flipLeft = false;
        private static bool flipRight = false;

        public ZumoMotors()
        {
            Init();
        }

        /// <summary>
        /// Initalize the Timer1 to generate the proper PWM outputs to the motor drivers
        /// </summary>
        private void Init()
        {
            PWM_L = new SecretLabs.NETMF.Hardware.PWM(PWM_L_PIN);
            PWM_R = new SecretLabs.NETMF.Hardware.PWM(PWM_R_PIN);
            DIR_L = new OutputPort(DIR_L_PIN, false);
            DIR_R = new OutputPort(DIR_R_PIN, false);
        }

        /// <summary>
        /// Enable or Disable the Flipping of Left Motor
        /// </summary>
        /// <param name="flip"></param>
        public void FlipLeftMotor(bool flip)
        {
            flipLeft = flip;
        }


        /// <summary>
        /// Enable or Disable the Flipping of Right Motor
        /// </summary>
        /// <param name="flip"></param>
        public void FlipRightMotor(bool flip)
        {
            flipRight = flip;
        }


        public void SetLeftSpeed(int speed)
        {
            //init(); // initialize if necessary

            bool reverse = false;

            if (speed < 0)
            {
                speed = -speed; // make speed a positive quantity
                reverse = true;    // preserve the direction
            }
            if (speed > 100)  // Max 
                speed = 100;

            PWM_L.SetDutyCycle((uint)speed);

            if (reverse ^ flipLeft) // flip if speed was negative or flipLeft setting is active, but not both
                DIR_L.Write(true);
            else
                DIR_L.Write(false);
        }


        public void SetRightSpeed(int speed)
        {
            bool reverse = false;

            if (speed < 0)
            {
                speed = -speed;  // Make speed a positive quantity
                reverse = true;  // Preserve the direction
            }
            if (speed > 100)  // Max PWM dutycycle
                speed = 100;
            
            PWM_R.SetDutyCycle((uint)speed);

            if (reverse ^ flipRight) // flip if speed was negative or flipRight setting is active, but not both
                DIR_R.Write(true);
            else
                DIR_R.Write(false);
        }


        public void SetSpeeds(int leftSpeed, int rightSpeed)
        {
            SetLeftSpeed(leftSpeed);
            SetRightSpeed(rightSpeed);
        }

    }
}



