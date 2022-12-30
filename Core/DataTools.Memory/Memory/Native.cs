using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DataTools.Memory, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c174bbb29449348a38b8385890700e1f1f83cd83dd6a93ee4db80993d85f6e46f49b6a3f31392dd63b033d03fc321190f3d7034876d13ed9ea8952fb5d32f03e958fc9062ed0a12b75cf85a9cf65aeef91404bfb09ca43489ec69e15dc763d459162aacb84d21ea39b6992d747b871af709a313621ec6bebcdf7c5396abc33bb")]
[assembly: InternalsVisibleTo("DataTools.Win32, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c174bbb29449348a38b8385890700e1f1f83cd83dd6a93ee4db80993d85f6e46f49b6a3f31392dd63b033d03fc321190f3d7034876d13ed9ea8952fb5d32f03e958fc9062ed0a12b75cf85a9cf65aeef91404bfb09ca43489ec69e15dc763d459162aacb84d21ea39b6992d747b871af709a313621ec6bebcdf7c5396abc33bb")]
[assembly: InternalsVisibleTo("DataTools.Hardware, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c174bbb29449348a38b8385890700e1f1f83cd83dd6a93ee4db80993d85f6e46f49b6a3f31392dd63b033d03fc321190f3d7034876d13ed9ea8952fb5d32f03e958fc9062ed0a12b75cf85a9cf65aeef91404bfb09ca43489ec69e15dc763d459162aacb84d21ea39b6992d747b871af709a313621ec6bebcdf7c5396abc33bb")]

namespace DataTools.Memory
{
    internal static class Native
    {
        internal static unsafe void ZeroMemory(void* handle, long len)
        {
            unsafe
            {
                byte* bp1 = (byte*)handle;
                byte* bep = (byte*)handle + len;

                if (len >= IntPtr.Size)
                {
                    if (IntPtr.Size == 8)
                    {
                        long* lp1 = (long*)bp1;
                        long* lep = (long*)bep;

                        do
                        {
                            *lp1++ = 0L;
                        } while (lp1 < lep);

                        if (lp1 == lep) return;

                        lp1--;
                        bp1 = (byte*)lp1;
                    }
                    else
                    {
                        int* ip1 = (int*)bp1;
                        int* lep = (int*)bep;

                        do
                        {
                            *ip1++ = 0;
                        } while (ip1 < lep);

                        if (ip1 == lep) return;

                        ip1--;
                        bp1 = (byte*)ip1;
                    }
                }

                do
                {
                    *bp1++ = 0;
                } while (bp1 < bep);
            }
        }

        public static void MemCpy(IntPtr src, IntPtr dest, long len)
        {
            unsafe
            {
                Buffer.MemoryCopy((void*)src, (void*)dest, len, len);
            }
        }

        //        internal static unsafe void MemCpy(void* src, void* dest, long len)
        //        {
        //            unsafe
        //            {
        //                //if (len >= 2048)
        //                //{
        //                Buffer.MemoryCopy(src, dest, len, len);
        //                return;
        //                //}

        //                byte* bp1 = (byte*)src;
        //                byte* bp2 = (byte*)dest;
        //                byte* bep = (byte*)src + len;

        //#if X64
        //                if (len >= 16)
        //                {
        //                    do
        //                    {
        //                        *(long*)bp2 = *(long*)bp1;
        //                        if (bep - bp1 < 8) break;

        //                        bp1 += 8;
        //                        bp2 += 8;

        //                    } while (bp1 < bep);

        //                    if (bp1 == bep) return;

        //                    len = bep - bp1;
        //                }

        //#else
        //                if (len >= 16)
        //                {
        //                    do
        //                    {
        //                        *(int*)bp2 = *(int*)bp1;
        //                        if (bep - bp1 < 4) break;

        //                        bp1 += 4;
        //                        bp2 += 4;

        //                    } while (bp1 < bep);

        //                    if (bp1 == bep) return;
        //                }

        //#endif
        //                do
        //                {
        //                    *bp2++ = *bp1++;
        //                } while (bp1 < bep);
        //            }
        //        }
    }

    //[StructLayout(LayoutKind.Sequential, Size = 16, Pack = 1)]
    //internal struct Block16
    //{
    //    public ulong val1;
    //    public ulong val2;
    //}

    //[StructLayout(LayoutKind.Sequential, Size = 64, Pack = 1)]
    //internal struct Block64
    //{
    //    public ulong val1;
    //    public ulong val2;
    //    public ulong val3;
    //    public ulong val4;
    //    public ulong val5;
    //    public ulong val6;
    //    public ulong val7;
    //    public ulong val8;
    //}
}