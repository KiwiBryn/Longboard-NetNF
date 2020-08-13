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
namespace devMobile.LongBoard.PwmTest
{
   using System;
   using System.Threading;
   using System.Diagnostics;
   using Windows.Devices.Adc;
   using Windows.Devices.Pwm;

   public class Program
   {
      public static void Main()
      {
         Debug.WriteLine("devMobile.LongBoard.PwmTest starting");
         Debug.WriteLine(PwmController.GetDeviceSelector());

         try
         {
            PwmController pwm = PwmController.FromId("TIM5");
            AdcController adc = AdcController.GetDefault();
            AdcChannel adcChannel = adc.OpenChannel(0);

            PwmPin pwmPin = pwm.OpenPin(PinNumber('A', 0));
            pwmPin.Controller.SetDesiredFrequency(1000);
            pwmPin.Start();

            while (true)
            {
               double value = adcChannel.ReadRatio();

               Debug.WriteLine(value.ToString("F2"));

               pwmPin.SetActiveDutyCyclePercentage(value);

               Thread.Sleep(100);
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
   }
}