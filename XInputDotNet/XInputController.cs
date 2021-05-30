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
        private const int POLL_TIMEOUT = 1000 / 25;

        private const int THUMBSTICK_AXIS_RANGE = 255 / 2;
        private const int THUMBSTICK_DEAD_ZONE_X = 10;
        private const int THUMBSTICK_DEAD_ZONE_Y = 10;

        private const int TRIGGER_FULL_PULL_VALUE = 255;
        private const int TRIGGER_CLICK_THRESHOLD = TRIGGER_FULL_PULL_VALUE / 2;

        private readonly PlayerIndex playerIndex;

        private GamePadState PreviousState { get; set; }

        public bool IsConnected { get; set; }

        private int leftStickX = 0;
        private int leftStickY = 0;
        private int rightStickX = 0;
        private int rightStickY = 0;
        private bool isLeftTriggerClicked = false;
        private bool isRightTriggerClicked = false;
        private byte prevBatteryState = 0;

        public event EventHandler<DeviceButtonStateChangedEventArgs> DeviceButtonStateChanged;
        public event EventHandler<DeviceAnalogStateChangedEventArgs> DeviceAnalogStateChanged;
        public event EventHandler<DeviceGeneralStateChangedEventArgs> DeviceGeneralStateChanged;

        internal XInputController(PlayerIndex playerIndex)
        {
            this.playerIndex = playerIndex;
            this.IsConnected = true;
        }

        public void PollState()
        {
            // Spawning a thread to handle continuous publishing of the analog
            // stick axis values when above 0
            new Thread(delegate ()
            {
                while (IsConnected)
                {
                    if (leftStickX != 0 || leftStickY != 0)
                    {
                        OnDeviceAnalogStateChanged(XInputControls.Analog.LeftThumbStick, new int[] { leftStickX, leftStickY });
                    }

                    if (rightStickX != 0 || rightStickY != 0)
                    {
                        OnDeviceAnalogStateChanged(XInputControls.Analog.RightThumbStick, new int[] { rightStickX, rightStickY });
                    }

                    Thread.Sleep(POLL_TIMEOUT);
                }
            }).Start();

            // Polling and parsing the controller state
            while (true)
            {
                GamePadState currentState = GamePad.GetState(playerIndex);

                if (!currentState.IsConnected)
                {
                    IsConnected = false;
                    break;
                }

                if (currentState.PacketNumber == PreviousState.PacketNumber)
                {
                    continue;
                }

                ParseButtonStates(currentState, PreviousState);
                ParseAnalogStates(currentState);
                //ParseDeviceState() // TODO: get battery state

                PreviousState = currentState;

                Thread.Sleep(POLL_TIMEOUT);
            }
        }

        private void ParseButtonStates(GamePadState currentState, GamePadState previousState)
        {
            if (currentState.Buttons.Guide != previousState.Buttons.Guide) OnDeviceButtonStateChanged(XInputControls.Button.Guide, currentState.Buttons.Guide);

            if (currentState.Buttons.Back != previousState.Buttons.Back) OnDeviceButtonStateChanged(XInputControls.Button.Back, currentState.Buttons.Back);

            if (currentState.Buttons.Start != previousState.Buttons.Start) OnDeviceButtonStateChanged(XInputControls.Button.Start, currentState.Buttons.Start);

            if (currentState.DPad.Left != previousState.DPad.Left) OnDeviceButtonStateChanged(XInputControls.Button.DpadLeft, currentState.DPad.Left);

            if (currentState.DPad.Up != previousState.DPad.Up) OnDeviceButtonStateChanged(XInputControls.Button.DpadUp, currentState.DPad.Up);

            if (currentState.DPad.Right != previousState.DPad.Right) OnDeviceButtonStateChanged(XInputControls.Button.DpadRight, currentState.DPad.Right);

            if (currentState.DPad.Down != previousState.DPad.Down) OnDeviceButtonStateChanged(XInputControls.Button.DpadDown, currentState.DPad.Down);

            if (currentState.Buttons.A != previousState.Buttons.A) OnDeviceButtonStateChanged(XInputControls.Button.A, currentState.Buttons.A);

            if (currentState.Buttons.B != previousState.Buttons.B) OnDeviceButtonStateChanged(XInputControls.Button.B, currentState.Buttons.B);

            if (currentState.Buttons.X != previousState.Buttons.X) OnDeviceButtonStateChanged(XInputControls.Button.X, currentState.Buttons.X);

            if (currentState.Buttons.Y != previousState.Buttons.Y) OnDeviceButtonStateChanged(XInputControls.Button.Y, currentState.Buttons.Y);

            if (currentState.Buttons.LeftShoulder != previousState.Buttons.LeftShoulder) OnDeviceButtonStateChanged(XInputControls.Button.LB, currentState.Buttons.LeftShoulder);

            if (currentState.Buttons.LeftStick != previousState.Buttons.LeftStick) OnDeviceButtonStateChanged(XInputControls.Button.LS, currentState.Buttons.LeftStick);

            if (currentState.Buttons.RightShoulder != previousState.Buttons.RightShoulder) OnDeviceButtonStateChanged(XInputControls.Button.RB, currentState.Buttons.RightShoulder);

            if (currentState.Buttons.RightStick != previousState.Buttons.RightStick) OnDeviceButtonStateChanged(XInputControls.Button.RS, currentState.Buttons.RightStick);
        }

        private void ParseAnalogStates(GamePadState currentState)
        {
            int x;
            int y;
            int trigger;

            // Parsing left thumbstick state
            x = (int)(THUMBSTICK_AXIS_RANGE * currentState.ThumbSticks.Left.X);
            y = (int)(THUMBSTICK_AXIS_RANGE * currentState.ThumbSticks.Left.Y);

            if ((-THUMBSTICK_DEAD_ZONE_X <= x) && (x <= THUMBSTICK_DEAD_ZONE_X)) x = 0;

            if ((-THUMBSTICK_DEAD_ZONE_Y <= y) && (y <= THUMBSTICK_DEAD_ZONE_Y)) y = 0;

            leftStickX = x;
            leftStickY = -y;

            // Parsing right thumbstick state
            x = (int)(THUMBSTICK_AXIS_RANGE * currentState.ThumbSticks.Right.X);
            y = (int)(THUMBSTICK_AXIS_RANGE * currentState.ThumbSticks.Right.Y);

            if ((-THUMBSTICK_DEAD_ZONE_X <= x) && (x <= THUMBSTICK_DEAD_ZONE_X)) x = 0;

            if ((-THUMBSTICK_DEAD_ZONE_Y <= y) && (y <= THUMBSTICK_DEAD_ZONE_Y)) y = 0;

            rightStickX = x;
            rightStickY = -y;

            // Parsing left trigger state
            trigger = (int)(TRIGGER_FULL_PULL_VALUE * currentState.Triggers.Left);

            OnDeviceAnalogStateChanged(XInputControls.Analog.LeftTrigger, new int[] { trigger });

            if (trigger > TRIGGER_CLICK_THRESHOLD)
            {
                if (!isLeftTriggerClicked)
                {
                    isLeftTriggerClicked = true;
                    OnDeviceButtonStateChanged(XInputControls.Button.LT, ButtonState.Pressed);
                }
            }
            else
            {
                if (isLeftTriggerClicked)
                {
                    isLeftTriggerClicked = false;
                    OnDeviceButtonStateChanged(XInputControls.Button.LT, ButtonState.Released);
                }
            }

            // Parsing right trigger state
            trigger = (int)(TRIGGER_FULL_PULL_VALUE * currentState.Triggers.Right);

            OnDeviceAnalogStateChanged(XInputControls.Analog.RightTrigger, new int[] { trigger });

            if (trigger > TRIGGER_CLICK_THRESHOLD)
            {
                if (!isRightTriggerClicked)
                {
                    isRightTriggerClicked = true;
                    OnDeviceButtonStateChanged(XInputControls.Button.RT, ButtonState.Pressed);
                }
            }
            else
            {
                if (isRightTriggerClicked)
                {
                    isRightTriggerClicked = false;
                    OnDeviceButtonStateChanged(XInputControls.Button.RT, ButtonState.Released);
                }
            }
        }

        private void ParseDeviceState()
        {

        }

        private void OnDeviceButtonStateChanged(XInputControls.Button button, ButtonState buttonState)
        {
            DeviceButtonStateChangedEventArgs args = new DeviceButtonStateChangedEventArgs();
            args.Button = button;
            args.IsPressed = buttonState == ButtonState.Pressed;

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
