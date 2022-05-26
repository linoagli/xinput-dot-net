using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XInputDotNet
{
    internal class GamePadState
    {
        public bool Guide { get; private set; } = false;
        public bool Start { get; private set; } = false;
        public bool Options { get; private set; } = false;

        public bool Left { get; private set; } = false;
        public bool Up { get; private set; } = false;
        public bool Right { get; private set; } = false;
        public bool Down { get; private set; } = false;

        public bool A { get; private set; } = false;
        public bool B { get; private set; } = false;
        public bool X { get; private set; } = false;
        public bool Y { get; private set; } = false;

        public bool LeftBumper { get; private set; } = false;
        public bool RightBumper { get; private set; } = false;

        public bool LeftStick { get; private set; } = false;
        public bool RightStick { get; private set; } = false;

        public byte LeftTrigger { get; private set; } = 0;
        public byte RightTrigger { get; private set; } = 0;

        public short LeftStickX { get; private set; } = 0;
        public short LeftStickY { get; private set; } = 0;
        public short RightStickX { get; private set; } = 0;
        public short RightStickY { get; private set; } = 0;

        internal void Apply(XInputInterface.RawState rawState)
        {
            Guide = (rawState.Gamepad.wButtons & (uint)XInputInterface.ButtonsConstants.Guide) != 0;
            Start = (rawState.Gamepad.wButtons & (uint)XInputInterface.ButtonsConstants.Start) != 0;
            Options = (rawState.Gamepad.wButtons & (uint)XInputInterface.ButtonsConstants.Back) != 0;

            Left = (rawState.Gamepad.wButtons & (uint)XInputInterface.ButtonsConstants.DPadLeft) != 0;
            Up = (rawState.Gamepad.wButtons & (uint)XInputInterface.ButtonsConstants.DPadUp) != 0;
            Right = (rawState.Gamepad.wButtons & (uint)XInputInterface.ButtonsConstants.DPadRight) != 0;
            Down = (rawState.Gamepad.wButtons & (uint)XInputInterface.ButtonsConstants.DPadDown) != 0;

            A = (rawState.Gamepad.wButtons & (uint)XInputInterface.ButtonsConstants.A) != 0;
            B = (rawState.Gamepad.wButtons & (uint)XInputInterface.ButtonsConstants.B) != 0;
            X = (rawState.Gamepad.wButtons & (uint)XInputInterface.ButtonsConstants.X) != 0;
            Y = (rawState.Gamepad.wButtons & (uint)XInputInterface.ButtonsConstants.Y) != 0;

            LeftBumper = (rawState.Gamepad.wButtons & (uint)XInputInterface.ButtonsConstants.LeftShoulder) != 0;
            RightBumper = (rawState.Gamepad.wButtons & (uint)XInputInterface.ButtonsConstants.RightShoulder) != 0;

            LeftStick = (rawState.Gamepad.wButtons & (uint)XInputInterface.ButtonsConstants.LeftThumb) != 0;
            RightStick = (rawState.Gamepad.wButtons & (uint)XInputInterface.ButtonsConstants.RightThumb) != 0;

            LeftTrigger = rawState.Gamepad.bLeftTrigger;
            RightTrigger = rawState.Gamepad.bRightTrigger;

            LeftStickX = rawState.Gamepad.sThumbLX;
            LeftStickY = rawState.Gamepad.sThumbLY;
            RightStickX = rawState.Gamepad.sThumbRX;
            RightStickY = rawState.Gamepad.sThumbRY;
        }

        internal void Copy(GamePadState state)
        {
            Guide = state.Guide;
            Start = state.Start;
            Options = state.Options;

            Left = state.Left;
            Up = state.Up;
            Right = state.Right;
            Down = state.Down;

            A = state.A;
            B = state.B;
            X = state.X;
            Y = state.Y;

            LeftBumper = state.LeftBumper;
            RightBumper = state.RightBumper;

            LeftStick = state.LeftStick;
            RightStick = state.RightStick;

            LeftTrigger = state.LeftTrigger;
            RightTrigger = state.RightTrigger;

            LeftStickX = state.LeftStickX;
            LeftStickY = state.LeftStickY;
            RightStickX = state.RightStickX;
            RightStickY = state.RightStickY;
        }
    }
}
