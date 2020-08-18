//---------------------------------------------------------------------------------
// Copyright (c) August 2020, devMobile Software
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// nanoff --target NETDUINO3_WIFI --update
//---------------------------------------------------------------------------------
namespace devMobile.Longboard
{
   using System;
   using System.Diagnostics;
   using System.Threading;
   using Windows.Devices.Gpio;
   using Windows.Devices.Pwm;
   using Windows.Devices.I2c;

   using Toolbox.NETMF.Hardware;

   public class Program
   {
      private const double PulseFrequency = 50.0;
      private const double PulseDurationMinimum = 0.05; // 1000uSec
      private const double PulseDurationMaximum = 0.1; // 2000uSec
      private const double WiiNunchukYMinimum = 0.0;
      private const double WiiNunchukYMaximum = 255.0;
      private const int ThrottleUpdatePeriod = 100;

      public static void Main()
      {
         Debug.WriteLine("devMobile.Longboard starting");
         Debug.WriteLine($"I2C:{I2cDevice.GetDeviceSelector()}");
         Debug.WriteLine($"PWM:{PwmController.GetDeviceSelector()}");

         try
         {
            Debug.WriteLine("LED Starting");
            GpioPin led = GpioController.GetDefault().OpenPin(PinNumber('A', 10));
            led.SetDriveMode(GpioPinDriveMode.Output);
            led.Write(GpioPinValue.Low);

            Debug.WriteLine("LED Starting");
            WiiNunchuk nunchuk = new WiiNunchuk("I2C1");

            Debug.WriteLine("ESC Starting");
            PwmController pwm = PwmController.FromId("TIM5");
            PwmPin pwmPin = pwm.OpenPin(PinNumber('A', 1));
            pwmPin.Controller.SetDesiredFrequency(PulseFrequency);
            pwmPin.Start();

            Debug.WriteLine("Thread.Sleep Starting");
            Thread.Sleep(2000);

            Debug.WriteLine("Mainloop Starting");
            while (true)
            {
               nunchuk.Read();

               double duration = Map(nunchuk.AnalogStickY, WiiNunchukYMinimum, WiiNunchukYMaximum, PulseDurationMinimum, PulseDurationMaximum);
               Debug.WriteLine($"Value:{nunchuk.AnalogStickY} Duration:{duration:F3}");

               pwmPin.SetActiveDutyCyclePercentage(duration);

               led.Toggle();
               Thread.Sleep(ThrottleUpdatePeriod);
            }
         }
         catch (Exception ex)
         {
            Debug.WriteLine(ex.Message);
         }
      }

      private static int PinNumber(char port, byte pin)
      {
         if (port < 'A' || port > 'J')
            throw new ArgumentException();

         return ((port - 'A') * 16) + pin;
      }

      private static double Map(double x, double inputMinimum, double inputMaximum, double outputMinimum, double outputMaximum)
      {
         return (x - inputMinimum) * (outputMaximum - outputMinimum) / (inputMaximum - inputMinimum) + outputMinimum;
      }
   }
}
