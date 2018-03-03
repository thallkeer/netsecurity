using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerForLab
{
    public enum StateMachine
    {
        WaitingForClient=0,
        GeneratingKeys,
        KeySending,
        WaitingForReceive,
        DecryptingFile

    }
}
