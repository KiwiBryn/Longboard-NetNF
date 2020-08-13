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
namespace devMobile.LongBoard.WiiNunchuckTest
{
   using System;
   using System.Diagnostics;
   using System.Threading;
   using Windows.Devices.I2c;

   using Toolbox.NETMF.Hardware;

   public class Program
   {
      public static void Main()
      {
         Debug.WriteLine("devMobile.LongBoard.WiiNunchuckTest starting");
         Debug.WriteLine(I2cDevice.GetDeviceSelector());

         try
         {
            WiiNunchuk nunchuk = new WiiNunchuk("I2C1");

            while (true)
            {
               nunchuk.Read();

               Debug.WriteLine($"JoyX: {nunchuk.AnalogStickX} JoyY:{nunchuk.AnalogStickY} AX:{nunchuk.AcceleroMeterX} AY:{nunchuk.AcceleroMeterY} AZ:{nunchuk.AcceleroMeterZ} BtnC:{nunchuk.ButtonC} BtnZ:{nunchuk.ButtonZ}");

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
