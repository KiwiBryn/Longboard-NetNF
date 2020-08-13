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
namespace devMobile.Longboard.AdcTest
{
   using System;
   using System.Diagnostics;
   using System.Threading;
   using Windows.Devices.Adc;

   public class Program
   {
      public static void Main()
      {
         Debug.WriteLine("devMobile.Longboard.AdcTest starting");
         Debug.WriteLine(AdcController.GetDeviceSelector());

         try
         {
            AdcController adc = AdcController.GetDefault();
            AdcChannel adcChannel = adc.OpenChannel(0);

            while (true)
            {
               double value = adcChannel.ReadRatio();

               Debug.WriteLine($"Value: {value:F2}");

               Thread.Sleep(100);
            }
         }
         catch (Exception ex)
         {
            Debug.WriteLine(ex.Message);
         }
      }
   }
}
