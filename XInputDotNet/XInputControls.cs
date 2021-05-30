using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XInputDotNet
{
    public class XInputControls
    {
        public enum Button
        {
            Guide, Back, Start,
            DpadLeft, DpadUp, DpadRight, DpadDown,
            A, B, X, Y,
            LB, LT, LS, RB, RT, RS
        }

        public enum Analog
        {
            LeftTrigger, RightTrigger,
            LeftThumbStick, RightThumbStick
        }
    }
}
