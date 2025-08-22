using System;
using System.Linq;
using System.Text;

namespace DataTools.Text
{
    internal struct BASE64STRUCT
    {
        public byte[] Data;
        public int Length;
    }

    /// <summary>
    /// Base 64 Tools
    /// </summary>
    public static class Base64
    {
        internal static readonly bool B64TableCreated;
        private const int BASE64PAD = 61;
        private const int BASE64PADRETURN = 254;
        private const string BASE64TABLE = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
        private static readonly byte[] InputTable = new byte[256];
        private static readonly byte[] OutputTable = new byte[64];
        static Base64()
        {
            int i;
            int d;

            for (i = 0; i < 256; i++)
            {
                InputTable[i] = 0x7F;
            }

            for (i = 0; i < 64; i++)
            {
                d = BASE64TABLE[i];

                OutputTable[i] = (byte)(d & 255);
                InputTable[d] = (byte)(i & 0x3F);
            }

            InputTable[BASE64PAD] = BASE64PADRETURN;
            B64TableCreated = true;
        }

        /// <summary>
        /// Convert from base 64 UTF-8 text data to binary data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] FromBase64(byte[] data)
        {
            Decode64(data, data.Length, out var bOut);
            return bOut;
        }

        /// <summary>
        /// Convert from a base 64 text string to binary data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] FromBase64String(string data)
        {
            return FromBase64(Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        /// Convert to base 64 UTF-8 text data from binary data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ToBase64(byte[] data)
        {
            Encode64(data, data.Length, out var bOut);
            return bOut.Data;
        }

        /// <summary>
        /// Convert to a base 64 text string from binary data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToBase64String(byte[] data)
        {
            return Encoding.UTF8.GetString(ToBase64(data));
        }
        internal static int Decode64(byte[] dataIn, int length, out byte[] dataOut)
        {
            int triplen, j, v;

            var quartet = new byte[4];

            triplen = length / 4 * 3;
            dataOut = new byte[triplen + 1];

            v = 0;

            for (triplen = 0; triplen < length;)
            {
                j = 0;
                while (j < 4)
                {
                    if (InputTable[dataIn[triplen]] != 0x7F)
                    {
                        quartet[j] = InputTable[dataIn[triplen]];
                        j++;
                    }

                    triplen++;
                    if (triplen >= length)
                    {
                        while (j < 4)
                        {
                            quartet[j] = BASE64PADRETURN;
                            j++;
                        }

                        break;
                    }
                }

                if (quartet[0] == BASE64PADRETURN || quartet[1] == BASE64PADRETURN)
                {
                    break;
                }

                dataOut[v] = (byte)((quartet[0] << 2) | (quartet[1] >> 4));
                v++;

                if (quartet[2] == BASE64PADRETURN)
                {
                    break;
                }

                dataOut[v] = (byte)((quartet[1] << 4) | (quartet[2] >> 2));
                v++;

                if (quartet[3] == BASE64PADRETURN)
                {
                    //l = (int)-1L;
                    break;
                }

                dataOut[v] = (byte)((quartet[2] << 6) | quartet[3]);
                v++;
            }

            Array.Resize(ref dataOut, v);
            return v;
        }

        internal static bool Encode64(byte[] dataIn, int length, out BASE64STRUCT b64Out, int maxColumns = 76)
        {
            double dln = (double)maxColumns;
            if (dln <= 0) dln = 0;

            int l;
            int x;
            int outPos;
            var column = 0;
            int lProcess;
            int lReturn;
            int lActual;

            if (length > dataIn.Length)
            {
                length = dataIn.Length;
            }

            lProcess = lActual = length;
            b64Out = new BASE64STRUCT();

            var triplets = dataIn.ToArray();

            if (0 != (lProcess % 3))
            {
                lProcess += (3 - lProcess % 3);
            }

            lReturn = lProcess / 3 * 4;

            Array.Resize(ref triplets, lProcess);

            if (dln != 0)
            {
                if (Math.Floor(lReturn / dln) != lReturn / dln)
                {
                    x = (int)(Math.Round(lReturn / dln) + 1);
                }
                else
                {
                    x = (int)(lReturn / dln);
                }
            }
            else
            {
                x = 0;
            }

            // carriage return and line feed
            x *= 2;
            l = lReturn + x;

            b64Out.Data = new byte[l];
            b64Out.Length = l;

            outPos = 0;

            for (x = 0; x < lProcess; x += 3)
            {
                b64Out.Data[outPos] = OutputTable[(triplets[x] >> 2)];
                b64Out.Data[outPos + 1] = OutputTable[(triplets[x] << 4) & 0x30 | (triplets[x + 1] >> 4)];
                b64Out.Data[outPos + 2] = OutputTable[(triplets[x + 1] << 2) & 0x3C | (triplets[x + 2] >> 6)];
                b64Out.Data[outPos + 3] = OutputTable[triplets[x + 2] & 0x3F];

                outPos += 4;
                column += 4;

                if (dln != 0 && column >= dln)
                {
                    column = 0;
                    if (l - x > 3)
                    {
                        b64Out.Data[outPos] = 13;
                        b64Out.Data[outPos + 1] = 10;

                        outPos += 2;
                    }
                }
            }

            switch (lProcess - lActual)
            {
                case 1:
                    b64Out.Data[outPos - 1] = BASE64PAD;
                    break;

                case 2:
                    b64Out.Data[outPos - 1] = BASE64PAD;
                    b64Out.Data[outPos - 2] = BASE64PAD;
                    break;
            }

            if (dln != 0 && column != 0)
            {
                b64Out.Data[outPos] = 13;
                b64Out.Data[outPos + 1] = 10;

                Array.Resize(ref b64Out.Data, outPos + 2);
            }
            else
            {
                Array.Resize(ref b64Out.Data, outPos);
            }

            return 0 != (b64Out.Length);
        }
    }
}