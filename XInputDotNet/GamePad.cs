using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XInputDotNet
{
    internal class GamePad
    {
        public const uint PLAYER_INDEX_ONE = 0;
        public const uint PLAYER_INDEX_TWO = 1;
        public const uint PLAYER_INDEX_THREE = 2;
        public const uint PLAYER_INDEX_FOUR = 3;

        private readonly uint playerIndex;
        
        private XInputInterface.RawState rawState;

        public bool IsConnected { get; private set; }
        public bool HasChanges { get; private set; }
        
        public uint PacketNumber { get; private set; }
        public GamePadState CurrentState { get; private set; }
        public GamePadState PreviousState { get; private set; }

        public GamePad(uint playerIndex)
        {
            this.playerIndex = playerIndex;

            IsConnected = false;
            HasChanges = false;

            PacketNumber = 0;
            CurrentState = new GamePadState();
            PreviousState = new GamePadState();
        }

        public void Update()
        {
            // Saving previous state
            PreviousState.Copy(CurrentState);

            // Getting raw gamepad state
            uint result = XInputInterface.XInputGamePadGetState(playerIndex, out rawState);

            // Zeroing raw state if the controller is not connected
            if (result != XInputInterface.RESULT_SUCCESS)
            {
                rawState.dwPacketNumber = 0;
                rawState.Gamepad.wButtons = 0;
                rawState.Gamepad.bLeftTrigger = 0;
                rawState.Gamepad.bRightTrigger = 0;
                rawState.Gamepad.sThumbLX = 0;
                rawState.Gamepad.sThumbLY = 0;
                rawState.Gamepad.sThumbRX = 0;
                rawState.Gamepad.sThumbRY = 0;
            }

            // Updating cycle related flags
            IsConnected = result == XInputInterface.RESULT_SUCCESS;
            HasChanges = PacketNumber != rawState.dwPacketNumber;
            
            // Quitting early if we have no changes
            if (!HasChanges)
            {
                return;
            }

            // Updating current state
            PacketNumber = rawState.dwPacketNumber;
            CurrentState.Apply(rawState);
        }

        public void SetVibration(float leftMotor, float rightMotor)
        {
            XInputInterface.XInputGamePadSetState(playerIndex, leftMotor, rightMotor);
        }
    }
}
