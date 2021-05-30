using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XInputDotNet
{
    public class XInputControllerManager
    {
        private XInputController controller;

        public event EventHandler<DeviceConnectionStateChangedEventArgs> DeviceConnectionStateChanged;

        public void Init()
        {
            Console.WriteLine("Initializing XInput controller manager...");

            new Thread(delegate ()
            {
                while (true)
                {
                    PurgeInactiveControllers();
                    FindControllers();
                    PollControllers(); // This is a blocking function. This will run until the controller is no logger connected.
                    Thread.Sleep(1000 * 3);
                }
            }).Start();
        }

        public void CleanUp()
        {
            // TODO: figure out a way to kill the init thread while controllers are polling...
        }

        private void FindControllers()
        {
            if (controller != null)
            {
                // If we already have a controller, skip the search
                return;
            }

            GamePadState statePlayer1 = GamePad.GetState(PlayerIndex.One);
            GamePadState statePlayer2 = GamePad.GetState(PlayerIndex.Two);
            GamePadState statePlayer3 = GamePad.GetState(PlayerIndex.Three);
            GamePadState statePlayer4 = GamePad.GetState(PlayerIndex.Four);

            int playerIndex = -1;

            if (statePlayer1.IsConnected)
            {
                playerIndex = (int)PlayerIndex.One;
            }
            else if (statePlayer2.IsConnected)
            {
                playerIndex = (int)PlayerIndex.Two;
            }
            else if (statePlayer3.IsConnected)
            {
                playerIndex = (int)PlayerIndex.Three;
            }
            else if (statePlayer4.IsConnected)
            {
                playerIndex = (int)PlayerIndex.Four;
            }

            if (playerIndex != -1)
            {
                Console.WriteLine($"XInput controller found: {playerIndex}");

                controller = new XInputController((PlayerIndex)playerIndex);
                OnDeviceConnectionStateChanged(controller, true);
            }
        }

        private void PollControllers()
        {
            if (controller != null)
            {
                controller.PollState();
            }
        }

        private void PurgeInactiveControllers()
        {
            if (controller == null)
            {
                return;
            }

            if (!controller.IsConnected)
            {
                OnDeviceConnectionStateChanged(controller, false);

                controller = null;
            }
        }

        private void OnDeviceConnectionStateChanged(XInputController controller, bool isConnected)
        {
            DeviceConnectionStateChangedEventArgs args = new DeviceConnectionStateChangedEventArgs();
            args.Controller = controller;
            args.IsConnected = isConnected;

            DeviceConnectionStateChanged?.Invoke(this, args);
        }

        public class DeviceConnectionStateChangedEventArgs : EventArgs
        {
            public XInputController Controller { get; set; }
            public bool IsConnected { get; set; }
        }
    }
}
