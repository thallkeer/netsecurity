﻿using System;
using System.Numerics;

namespace ClientForLab.CipherUtils
{
    [Serializable]
    public class PublicKey
    {
        public BigInteger E { get; }
        public BigInteger N { get; }

        public PublicKey(BigInteger e, BigInteger n)
        {
            this.E = e;
            this.N = n;
        }

    }
}
