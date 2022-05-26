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

            int playerIndex = -1;

            if (XInputInterface.IsGamePadConnected(GamePad.PLAYER_INDEX_ONE))
            {
                playerIndex = (int)GamePad.PLAYER_INDEX_ONE;
            }
            else if (XInputInterface.IsGamePadConnected(GamePad.PLAYER_INDEX_TWO))
            {
                playerIndex = (int)GamePad.PLAYER_INDEX_TWO;
            }
            else if (XInputInterface.IsGamePadConnected(GamePad.PLAYER_INDEX_THREE))
            {
                playerIndex = (int)GamePad.PLAYER_INDEX_THREE;
            }
            else if (XInputInterface.IsGamePadConnected(GamePad.PLAYER_INDEX_FOUR))
            {
                playerIndex = (int)GamePad.PLAYER_INDEX_FOUR;
            }

            if (playerIndex != -1)
            {
                Console.WriteLine($"XInput controller found: {playerIndex}");

                controller = new XInputController((uint)playerIndex);
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
