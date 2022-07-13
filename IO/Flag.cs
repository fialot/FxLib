using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.IO
{
    public class Flag
    {
        uint Value;

        public void SetFlag(uint flag)
        {
            Value |= flag;
        }

        public void SetFlag(Enum flag)
        {
            //Value |= (int)flag;
        }

        public void ClearFlag(uint flag)
        {
            Value &= ~flag;
        }

        public bool HasFlag(uint flag)
        {
            if ((Value & flag) > 0)
                return true;
            else
                return false;
        }
    }
}
