using System;
using System.Runtime.InteropServices;
using System.Threading;
using XInputDotNetPure;

namespace XInputDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                /*
                GamePadState state = GamePad.GetState(PlayerIndex.One);
                Console.WriteLine("IsConnected {0} Packet #{1}", state.IsConnected, state.PacketNumber);
                Console.WriteLine("\tTriggers {0} {1}", state.Triggers.Left, state.Triggers.Right);
                Console.WriteLine("\tD-Pad {0} {1} {2} {3}", state.DPad.Up, state.DPad.Right, state.DPad.Down, state.DPad.Left);
                Console.WriteLine("\tButtons Start {0} Back {1} LeftStick {2} RightStick {3} LeftShoulder {4} RightShoulder {5} Guide {6} A {7} B {8} X {9} Y {10}",
                    state.Buttons.Start, state.Buttons.Back, state.Buttons.LeftStick, state.Buttons.RightStick, state.Buttons.LeftShoulder, state.Buttons.RightShoulder,
                    state.Buttons.Guide, state.Buttons.A, state.Buttons.B, state.Buttons.X, state.Buttons.Y);
                Console.WriteLine("\tSticks Left {0} {1} Right {2} {3}", state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y, state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y);
                GamePad.SetVibration(PlayerIndex.One, state.Triggers.Left, state.Triggers.Right);
                */

                //testHomeButton();

                GamePadState state = GamePad.GetState(PlayerIndex.One);
                Console.WriteLine("IsConnected {0} Packet #{1} Guide Pressed={2}", state.IsConnected, state.PacketNumber, state.Buttons.Guide);

                Thread.Sleep(1000);
            }
        }



        [DllImport("xinput1_4.dll", EntryPoint = "#100")]
        static extern int secret_get_gamepad(int playerIndex, out XINPUT_GAMEPAD_SECRET struc);

        public struct XINPUT_GAMEPAD_SECRET
        {
            public UInt32 eventCount;
            public ushort wButtons;
            public Byte bLeftTrigger;
            public Byte bRightTrigger;
            public short sThumbLX;
            public short sThumbLY;
            public short sThumbRX;
            public short sThumbRY;
        }

        public static XINPUT_GAMEPAD_SECRET xgs;

        static bool testHomeButton()
        {
            int stat;
            bool value;


            stat = secret_get_gamepad(0, out xgs);

            Console.WriteLine($"stat = {stat}");

            if (stat != 0)
                return false;

            Console.WriteLine($"eventCount {xgs.eventCount}");

            value = ((xgs.wButtons & 0x0400) != 0);

            if (value)
            {
                Console.WriteLine($"guide pressed: {value}");
                return true;
            }
               


            return false;
        }


    }
}
