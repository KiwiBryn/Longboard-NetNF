//---------------------------------------------------------------------------------
// Copyright (c) August 2020, devMobile Software
//
// Inspired by sample code provided by GHI Electronics. Thanks, your sample code got me 
// thinking abouthow to solve this problem when the NF PWM did support setting the 
// duration.
//
// https://docs.ghielectronics.com/software/tinyclr/tutorials/pwm.html
//
// With nanoFramework modifications by Bryn Lewis (https://blog.devMobile.co.nz)
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
//---------------------------------------------------------------------------------
namespace devMobile.Hardware
{
   using System;
   using Windows.Devices.Pwm;

   public class ServoMotor
   {
      private readonly PwmPin servo;
      private readonly ServoType type;

      private double minPulseLength;
      private double maxPulseLength;
      public double Position { get; set; }

      public enum ServoType
      {
         Positional,
         Continuous
      }

      public ServoMotor(string pwmId, ServoType type, int pwmPin)
      {
         PwmController pwmController = PwmController.FromId(pwmId);
         pwmController.SetDesiredFrequency(50);

         this.ConfigurePulseParameters(1.0, 2.0);
         this.servo = pwmController.OpenPin(pwmPin);

         this.type = type;
         this.Position = 0;
      }

      public void ConfigurePulseParameters(double minimumPulseWidth, double maximumPulseWidth)
      {

         if (minimumPulseWidth > 1.5 || minimumPulseWidth < 0.1)
            throw new ArgumentOutOfRangeException("Must be between 0.1 and 1.5 ms");

         if (maximumPulseWidth > 3 || maximumPulseWidth < 1.6)
            throw new ArgumentOutOfRangeException("Must be between 1.6 and 3 ms");

         this.minPulseLength = minimumPulseWidth;
         this.maxPulseLength = maximumPulseWidth;
      }

      public void Set(double value)
      {
         if (this.type == ServoType.Positional)
         {
            this.PositionalSetPosition(value);
            this.Position = value;
         }
         else
         {
            this.ContinousSetSpeed(value);
         }
      }

      private void PositionalSetPosition(double angle)
      {
         if (angle < 0 || angle > 180)
            throw new ArgumentOutOfRangeException("degrees",
                "degrees must be between 0 and 180.");

         var duty = ((angle / 180.0 * (this.maxPulseLength - this.minPulseLength))
             + this.minPulseLength) / 20;

         this.servo.SetActiveDutyCyclePercentage(duty);
         this.servo.Start();
      }

      private void ContinousSetSpeed(double speed)
      {
         if (speed < -100 || speed > 100)
            throw new ArgumentOutOfRangeException("speed",
                "speed must be between -100 and 100.");

         PositionalSetPosition(speed / 100 * 90 + 90);
      }

      public void Stop() => this.servo.Stop();
   }
}

