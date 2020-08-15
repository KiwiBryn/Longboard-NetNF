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
#define POSITIONAL
//#define CONTINUOUS

#if !POSITIONAL && !CONTINUOUS
   #error One of positional or continuous must be defined
#endif
#if POSITIONAL && CONTINUOUS
   #error Only one of positional or continuous must be defined
#endif
using System;
using System.Threading;
using System.Diagnostics;
using Windows.Devices.Adc;

using devMobile.Hardware;


namespace devMobile.Longboard.ServoTest
{
   public class Program
   {
      public static void Main()
      {
         Debug.WriteLine("devMobile.Longboard.ServoTest starting");

         try
         {
            AdcController adc = AdcController.GetDefault();
            AdcChannel adcChannel = adc.OpenChannel(0);

#if POSITIONAL
            ServoMotor servo = new ServoMotor("TIM5", ServoMotor.ServoType.Positional, PinNumber('A', 0));
            servo.ConfigurePulseParameters(0.1, 2.3);
#endif
#if CONTINUOUS
            ServoMotor servo = new ServoMotor("TIM5", ServoMotor.ServoType.Continuous, PinNumber('A', 0));
            servo.ConfigurePulseParameters(0.1, 2.3);
#endif

            while (true)
            {
               double value = adcChannel.ReadRatio();

#if POSITIONAL
               double position = Map(value, 0.0, 1.0, 0.0, 180);
#endif
#if CONTINUOUS
               double position = Map(value, 0.0, 1.0, -100.0, 100.0);
#endif
               Debug.WriteLine($"Value: {value:F2} Position: {position:F1}");

               servo.Set(position);

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
      private static double Map(double x, double inputMinimum, double inputMaximum, double outputMinimum, double outputMaximum)
      {
         return (x - inputMinimum) * (outputMaximum - outputMinimum) / (inputMaximum - inputMinimum) + outputMinimum;
      }
   }
}