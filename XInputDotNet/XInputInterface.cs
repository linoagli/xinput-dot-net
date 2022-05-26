using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XInputDotNet
{
    internal class XInputInterface
    {        
        internal const uint RESULT_SUCCESS = 0x000;

        [DllImport("XInputInterface")]
        internal static extern uint XInputGamePadGetState(uint playerIndex, out RawState state);
        [DllImport("XInputInterface")]
        internal static extern void XInputGamePadSetState(uint playerIndex, float leftMotor, float rightMotor);

        [StructLayout(LayoutKind.Sequential)]
        internal struct RawState
        {
            public uint dwPacketNumber;
            public GamePad Gamepad;

            [StructLayout(LayoutKind.Sequential)]
            public struct GamePad
            {
                public ushort wButtons;
                public byte bLeftTrigger;
                public byte bRightTrigger;
                public short sThumbLX;
                public short sThumbLY;
                public short sThumbRX;
                public short sThumbRY;
            }
        }

        internal enum ButtonsConstants
        {
            DPadUp = 0x00000001,
            DPadDown = 0x00000002,
            DPadLeft = 0x00000004,
            DPadRight = 0x00000008,
            Start = 0x00000010,
            Back = 0x00000020,
            LeftThumb = 0x00000040,
            RightThumb = 0x00000080,
            LeftShoulder = 0x0100,
            RightShoulder = 0x0200,
            Guide = 0x0400,
            A = 0x1000,
            B = 0x2000,
            X = 0x4000,
            Y = 0x8000
        }

        public static bool IsGamePadConnected(uint playerIndex)
        {
            return XInputGamePadGetState(playerIndex, out _) == RESULT_SUCCESS;
        }
    }
}
