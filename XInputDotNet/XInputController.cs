using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XInputDotNet
{
    public class XInputController
    {
        private const int POLL_TIMEOUT = 30; // ~33hz

        private const int THUMBSTICK_AXIS_RANGE = 255 / 2;
        private const int THUMBSTICK_DEAD_ZONE_X = 10;
        private const int THUMBSTICK_DEAD_ZONE_Y = 10;

        private const int TRIGGER_FULL_PULL_VALUE = 255;
        private const int TRIGGER_CLICK_THRESHOLD = TRIGGER_FULL_PULL_VALUE / 2;

        private readonly GamePad gamePad;

        public bool IsConnected { get; private set; }

        private bool isLeftTriggerClicked = false;
        private bool isRightTriggerClicked = false;

        public event EventHandler<DeviceButtonStateChangedEventArgs> DeviceButtonStateChanged;
        public event EventHandler<DeviceAnalogStateChangedEventArgs> DeviceAnalogStateChanged;
        public event EventHandler<DeviceGeneralStateChangedEventArgs> DeviceGeneralStateChanged;

        internal XInputController(uint playerIndex)
        {
            this.gamePad = new GamePad(playerIndex);
            this.IsConnected = true;
        }

        public void PollState()
        {
            // Polling and parsing the controller state
            while (true)
            {
                gamePad.Update();

                if (!gamePad.IsConnected)
                {
                    IsConnected = false;
                    break;
                }

                if (gamePad.HasChanges)
                {
                    ParseButtonStates(gamePad.CurrentState, gamePad.PreviousState);
                    ParseAnalogStates(gamePad.CurrentState);
                    // TODO: add logic for tracking and publishing controller battery level
                }

                Thread.Sleep(POLL_TIMEOUT);
            }
        }

        private void ParseButtonStates(GamePadState currentState, GamePadState previousState)
        {
            if (currentState.Guide != previousState.Guide) OnDeviceButtonStateChanged(XInputControls.Button.Guide, currentState.Guide);

            if (currentState.Start != previousState.Start) OnDeviceButtonStateChanged(XInputControls.Button.Start, currentState.Start);

            if (currentState.Options != previousState.Options) OnDeviceButtonStateChanged(XInputControls.Button.Back, currentState.Options);

            if (currentState.Left != previousState.Left) OnDeviceButtonStateChanged(XInputControls.Button.DpadLeft, currentState.Left);

            if (currentState.Up != previousState.Up) OnDeviceButtonStateChanged(XInputControls.Button.DpadUp, currentState.Up);

            if (currentState.Right != previousState.Right) OnDeviceButtonStateChanged(XInputControls.Button.DpadRight, currentState.Right);

            if (currentState.Down != previousState.Down) OnDeviceButtonStateChanged(XInputControls.Button.DpadDown, currentState.Down);

            if (currentState.A != previousState.A) OnDeviceButtonStateChanged(XInputControls.Button.A, currentState.A);

            if (currentState.B != previousState.B) OnDeviceButtonStateChanged(XInputControls.Button.B, currentState.B);

            if (currentState.X != previousState.X) OnDeviceButtonStateChanged(XInputControls.Button.X, currentState.X);

            if (currentState.Y != previousState.Y) OnDeviceButtonStateChanged(XInputControls.Button.Y, currentState.Y);

            if (currentState.LeftBumper != previousState.LeftBumper) OnDeviceButtonStateChanged(XInputControls.Button.LB, currentState.LeftBumper);

            if (currentState.LeftStick != previousState.LeftStick) OnDeviceButtonStateChanged(XInputControls.Button.LS, currentState.LeftStick);

            if (currentState.RightBumper != previousState.RightBumper) OnDeviceButtonStateChanged(XInputControls.Button.RB, currentState.RightBumper);

            if (currentState.RightStick != previousState.RightStick) OnDeviceButtonStateChanged(XInputControls.Button.RS, currentState.RightStick);
        }

        private void ParseAnalogStates(GamePadState currentState)
        {
            int x;
            int y;

            // Parsing left thumbstick state
            x = (int)((float)currentState.LeftStickX / short.MaxValue * THUMBSTICK_AXIS_RANGE);
            y = (int)((float)currentState.LeftStickY / short.MaxValue * THUMBSTICK_AXIS_RANGE);

            if ((-THUMBSTICK_DEAD_ZONE_X <= x) && (x <= THUMBSTICK_DEAD_ZONE_X)) x = 0;

            if ((-THUMBSTICK_DEAD_ZONE_Y <= y) && (y <= THUMBSTICK_DEAD_ZONE_Y)) y = 0;

            if (x != 0 || y != 0) OnDeviceAnalogStateChanged(XInputControls.Analog.LeftThumbStick, new int[] { x, -y });

            // Parsing right thumbstick state
            x = (int)((float)currentState.RightStickX / short.MaxValue * THUMBSTICK_AXIS_RANGE);
            y = (int)((float)currentState.RightStickY / short.MaxValue * THUMBSTICK_AXIS_RANGE);

            if ((-THUMBSTICK_DEAD_ZONE_X <= x) && (x <= THUMBSTICK_DEAD_ZONE_X)) x = 0;

            if ((-THUMBSTICK_DEAD_ZONE_Y <= y) && (y <= THUMBSTICK_DEAD_ZONE_Y)) y = 0;

            if (x != 0 || y != 0) OnDeviceAnalogStateChanged(XInputControls.Analog.RightThumbStick, new int[] { x, -y });

            // Parsing left trigger state
            OnDeviceAnalogStateChanged(XInputControls.Analog.LeftTrigger, new int[] { currentState.LeftTrigger });

            if (currentState.LeftTrigger > TRIGGER_CLICK_THRESHOLD)
            {
                if (!isLeftTriggerClicked)
                {
                    isLeftTriggerClicked = true;
                    OnDeviceButtonStateChanged(XInputControls.Button.LT, true);
                }
            }
            else
            {
                if (isLeftTriggerClicked)
                {
                    isLeftTriggerClicked = false;
                    OnDeviceButtonStateChanged(XInputControls.Button.LT, false);
                }
            }

            // Parsing right trigger state
            OnDeviceAnalogStateChanged(XInputControls.Analog.RightTrigger, new int[] { currentState.RightTrigger });

            if (currentState.RightTrigger > TRIGGER_CLICK_THRESHOLD)
            {
                if (!isRightTriggerClicked)
                {
                    isRightTriggerClicked = true;
                    OnDeviceButtonStateChanged(XInputControls.Button.RT, true);
                }
            }
            else
            {
                if (isRightTriggerClicked)
                {
                    isRightTriggerClicked = false;
                    OnDeviceButtonStateChanged(XInputControls.Button.RT, false);
                }
            }
        }

        private void OnDeviceButtonStateChanged(XInputControls.Button button, bool isPressed)
        {
            DeviceButtonStateChangedEventArgs args = new DeviceButtonStateChangedEventArgs();
            args.Button = button;
            args.IsPressed = isPressed;

            DeviceButtonStateChanged?.Invoke(this, args);
        }

        private void OnDeviceAnalogStateChanged(XInputControls.Analog analog, int[] value)
        {
            DeviceAnalogStateChangedEventArgs args = new DeviceAnalogStateChangedEventArgs();
            args.Analog = analog;
            args.Value = value;

            DeviceAnalogStateChanged?.Invoke(this, args);
        }

        public class DeviceButtonStateChangedEventArgs : EventArgs
        {
            public XInputControls.Button Button { get; set; }
            public bool IsPressed { get; set; }
        }

        public class DeviceAnalogStateChangedEventArgs : EventArgs
        {
            public XInputControls.Analog Analog { get; set; }
            public int[] Value { get; set; }
        }

        public class DeviceGeneralStateChangedEventArgs : EventArgs
        {
            public int Battery { get; set; }
        }
    }
}
