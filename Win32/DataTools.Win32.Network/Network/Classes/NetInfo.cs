// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: NetInfoApi
//         Windows Networking Api
//
//         Enums are documented in part from the API documentation at MSDN.
//
// Copyright (C) 2011-2023 Nathaniel Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License
// *************************************************

using DataTools.Win32.Memory;

using System;
using System.Runtime.InteropServices;

namespace DataTools.Win32.Network
{
    /// <summary>
    /// Network Information static class
    /// </summary>
    public static class NetInfo
    {
        [DllImport("Secur32.dll", EntryPoint = "GetUserNameExW", CharSet = CharSet.Unicode)]
        private static extern bool GetUserNameEx(ExtendedNameFormat NameFormat, [MarshalAs(UnmanagedType.LPWStr)] string lpNameBuffer, ref int lpnSize);

        [DllImport("Secur32.dll", EntryPoint = "GetUserNameExW", CharSet = CharSet.Unicode)]
        private static extern bool GetUserNameEx(ExtendedNameFormat NameFormat, nint lpNameBuffer, ref int lpnSize);

        [DllImport("advapi32.dll", EntryPoint = "GetUserNameW", CharSet = CharSet.Unicode)]
        private static extern bool GetUserName(nint lpNameBuffer, ref int lpnSize);

        [DllImport("netapi32.dll")]
        private static extern NET_API_STATUS NetUserGetInfo([MarshalAs(UnmanagedType.LPWStr)] string servername, [MarshalAs(UnmanagedType.LPWStr)] string username, int level, ref MemPtr bufptr);

        [DllImport("netapi32.dll", EntryPoint = "NetServerEnum", CharSet = CharSet.Unicode)]
        private static extern NET_API_STATUS NetServerEnum([MarshalAs(UnmanagedType.LPWStr)] string servername, int level, ref MemPtr bufptr, int prefmaxlen, ref int entriesread, ref int totalentries, ServerTypes serverType, [MarshalAs(UnmanagedType.LPWStr)] string domain, ref nint resume_handle);

        [DllImport("netapi32.dll", EntryPoint = "NetServerGetInfo", CharSet = CharSet.Unicode)]
        private static extern NET_API_STATUS NetServerGetInfo([MarshalAs(UnmanagedType.LPWStr)] string servername, int level, ref MemPtr bufptr);

        [DllImport("netapi32.dll", EntryPoint = "NetLocalGroupEnum", CharSet = CharSet.Unicode)]
        private static extern NET_API_STATUS NetLocalGroupEnum([MarshalAs(UnmanagedType.LPWStr)] string servername, int level, ref MemPtr bufptr, int prefmaxlen, ref int entriesread, ref int totalentries, ref nint resume_handle);

        [DllImport("netapi32.dll", EntryPoint = "NetGroupEnum", CharSet = CharSet.Unicode)]
        private static extern NET_API_STATUS NetGroupEnum([MarshalAs(UnmanagedType.LPWStr)] string servername, int level, ref MemPtr bufptr, int prefmaxlen, ref int entriesread, ref int totalentries, ref nint resume_handle);

        [DllImport("netapi32.dll", EntryPoint = "NetUserEnum", CharSet = CharSet.Unicode)]
        private static extern NET_API_STATUS NetUserEnum([MarshalAs(UnmanagedType.LPWStr)] string servername, int level, int filter, ref MemPtr bufptr, int prefmaxlen, ref int entriesread, ref int totalentries, ref nint resume_handle);

        [DllImport("netapi32.dll", EntryPoint = "NetUserEnum", CharSet = CharSet.Unicode)]
        private static extern NET_API_STATUS NetUserEnum([MarshalAs(UnmanagedType.LPWStr)] string servername, int level, int filter, out NetworkMemPtr bufptr, int prefmaxlen, out int entriesread, out int totalentries, ref nint resume_handle);

        [DllImport("netapi32.dll")]
        private static extern NET_API_STATUS NetApiBufferFree(nint buffer);

        [DllImport("netapi32.dll")]
        private static extern NET_API_STATUS NetApiBufferAllocate(int bytecount, ref nint buffer);

        [DllImport("Netapi32.dll", CharSet = CharSet.Unicode)]
        private static extern NET_API_STATUS NetGetJoinInformation(string lpServer, ref nint lpNameBuffer, ref NetworkJoinStatus bufferType);

        [DllImport("netapi32.dll")]
        private static extern NET_API_STATUS NetGroupGetUsers([MarshalAs(UnmanagedType.LPWStr)] string servername, [MarshalAs(UnmanagedType.LPWStr)] string groupname, int level, ref MemPtr bufptr, int prefmaxlen, ref int entriesread, ref int totalentries, ref nint ResumeHandle);

        [DllImport("netapi32.dll")]
        private static extern NET_API_STATUS NetLocalGroupGetMembers([MarshalAs(UnmanagedType.LPWStr)] string servername, [MarshalAs(UnmanagedType.LPWStr)] string groupname, int level, ref MemPtr bufptr, int prefmaxlen, ref int entriesread, ref int totalentries, ref nint ResumeHandle);

        /// <summary>
        /// Return the current user's full display name
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string CurrentUserFullName(ExtendedNameFormat type = ExtendedNameFormat.NameDisplay)
        {
            string ret;
            int cb = 10240;

            using (var lps = new SafePtr(cb))
            {
                if (GetUserNameEx(type, lps, ref cb))
                {
                    ret = lps.ToString();
                }
                else
                {
                    ret = null;
                }

                return ret;
            }
        }

        /// <summary>
        /// Return the username for the current user
        /// </summary>
        /// <returns></returns>
        public static string CurrentUserName()
        {
            int cb = 10240;

            using (SafePtr lps = new SafePtr(cb))
            {
                if (GetUserName(lps, ref cb))
                {
                    return lps.ToString();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Enumerates all computers visible to the specified computer on the specified domain.
        /// </summary>
        /// <param name="computer">Optional computer name.  The local computer is assumed if this parameter is null.</param>
        /// <param name="domain">Optional domain name.  The primary domain of the specified computer is assumed if this parameter is null.</param>
        /// <returns>An array of ServerInfo1 objects.</returns>
        /// <remarks></remarks>
        public static ServerInfo101[] EnumServers()
        {
            MemPtr adv;

            var mm = new MemPtr();
            int en = 0;
            int ten = 0;

            ServerInfo101[] servers = null;

            int i;
            int c;

            var inul = new nint();

            try
            {
                var result = NetServerEnum(null, 101, ref mm, -1, ref en, ref ten, ServerTypes.WindowsNT, null, ref inul);

                if (result == NET_API_STATUS.NERR_Success)
                {
                    adv = mm;
                    c = ten;

                    servers = new ServerInfo101[c + 1];
                    var cbstr = Marshal.SizeOf<ServerInfo101>();

                    for (i = 0; i < c; i++)
                    {
                        servers[i] = adv.ToStruct<ServerInfo101>();
                        adv += cbstr;
                    }
                }
            }
            catch
            {
            }
            finally
            {
                mm.NetFree();
            }

            return servers;
        }

        /// <summary>
        /// Gets network information for the specified computer.
        /// </summary>
        /// <param name="computer">Computer for which to retrieve the information.</param>
        /// <param name="info">A ServerInfo101 structure that receives the information.</param>
        /// <remarks></remarks>
        public static bool GetServerInfo(string computer, ref ServerInfo101 info)
        {
            var mm = new MemPtr();

            try
            {
                var result = NetServerGetInfo(computer, 101, ref mm);
                if (result == NET_API_STATUS.NERR_Success)
                {
                    info = mm.ToStruct<ServerInfo101>();
                    return true;
                }
            }
            catch
            {
            }
            finally
            {
                mm.NetFree();
            }

            return false;
        }

        /// <summary>
        /// Enumerates the groups on a system.
        /// </summary>
        /// <param name="computer">The computer to enumerate.</param>
        /// <returns>An array of GroupInfo2 structures.</returns>
        /// <remarks></remarks>
        public static GroupInfo2[] EnumGroups(string computer)
        {
            var mm = new MemPtr();
            int en = 0;
            int ten = 0;

            MemPtr adv;
            GroupInfo2[] grp;

            var inul = new nint();

            try
            {
                var result = NetGroupEnum(computer, 2, ref mm, -1, ref en, ref ten, ref inul);
                if (result == NET_API_STATUS.NERR_Success)
                {
                    adv = mm;

                    int i;
                    int c = ten;

                    grp = new GroupInfo2[c + 1];
                    var cbstr = Marshal.SizeOf<GroupInfo2>();
                    for (i = 0; i < c; i++)
                    {
                        grp[i] = adv.ToStruct<GroupInfo2>();
                        adv += cbstr;
                    }

                    return grp;
                }
            }
            catch
            {
            }
            finally
            {
                mm.NetFree();
            }

            return null;
        }

        /// <summary>
        /// Enumerates local groups for the specified computer.
        /// </summary>
        /// <param name="computer">The computer to enumerate.</param>
        /// <returns>An array of LocalGroupInfo1 structures.</returns>
        /// <remarks></remarks>
        public static LocalGroupInfo1[] EnumLocalGroups(string computer)
        {
            var mm = new MemPtr();
            int en = 0;
            int ten = 0;

            MemPtr adv;
            LocalGroupInfo1[] grp;

            var inul = new nint();

            try
            {
                var result = NetLocalGroupEnum(computer, 1, ref mm, -1, ref en, ref ten, ref inul);
                if (result == NET_API_STATUS.NERR_Success)
                {
                    adv = mm;

                    int i;
                    int c = ten;

                    grp = new LocalGroupInfo1[c + 1];
                    var cbstr = Marshal.SizeOf<LocalGroupInfo1>();
                    for (i = 0; i < c; i++)
                    {
                        grp[i] = adv.ToStruct<LocalGroupInfo1>();
                        adv += cbstr;
                    }

                    return grp;
                }
            }
            catch
            {
            }
            finally
            {
                mm.NetFree();
            }

            return null;
        }

        /// <summary>
        /// Enumerates users of a given group.
        /// </summary>
        /// <param name="computer">Computer for which to retrieve the information.</param>
        /// <param name="group">Group to enumerate.</param>
        /// <returns>An array of user names.</returns>
        /// <remarks></remarks>
        public static string[] GroupUsers(string computer, string group)
        {
            var mm = new MemPtr();

            MemPtr origPtr = new MemPtr();

            int cbt = 0;
            int cb = 0;

            string[] s = null;

            try
            {
                var inul = new nint();

                if (NetGroupGetUsers(computer, group, 0, ref mm, -1, ref cb, ref cbt, ref inul) == NET_API_STATUS.NERR_Success)
                {
                    origPtr = mm;
                    UserGroup0 z;
                    int i;

                    s = new string[cb];
                    var cbstr = Marshal.SizeOf<UserGroup0>();

                    for (i = 0; i < cb; i++)
                    {
                        z = mm.ToStruct<UserGroup0>();
                        s[i] = z.Name;

                        mm += cbstr;
                    }

                    return s;
                }
            }
            catch
            {
                throw new NativeException();
            }
            finally
            {
                origPtr.NetFree();
            }

            return null;
        }

        /// <summary>
        /// Gets the members of the specified local group on the specified machine.
        /// </summary>
        /// <param name="computer">The computer for which to retrieve the information.</param>
        /// <param name="group">The name of the group to enumerate.</param>
        /// <param name="SidType">The type of group members to return.</param>
        /// <returns>A list of group member names.</returns>
        /// <remarks></remarks>
        public static string[] LocalGroupUsers(string computer, string group, SidUsage SidType = SidUsage.SidTypeUser)
        {
            var mm = new MemPtr();

            MemPtr origPtr = new MemPtr();

            int x = 0;
            int cbt = 0;
            int cb = 0;

            string[] s = null;

            try
            {
                var inul = new nint();
                if (NetLocalGroupGetMembers(computer, group, 1, ref mm, -1, ref cb, ref cbt, ref inul) == NET_API_STATUS.NERR_Success)
                {
                    if (cb == 0)
                    {
                        mm.NetFree();
                        return null;
                    }

                    origPtr = mm;

                    UserLocalGroup1 z;
                    int i;

                    s = new string[cb];

                    for (i = 0; i < cb; i++)
                    {
                        z = mm.ToStruct<UserLocalGroup1>();
                        if (z.SidUsage == SidType)
                        {
                            s[x] = z.Name;
                            mm = mm + Marshal.SizeOf(z);
                            x += 1;
                        }
                    }

                    Array.Resize(ref s, x);
                    return s;
                }
            }
            catch
            {
                throw new NativeException();
            }
            finally
            {
                origPtr.NetFree();
            }

            return null;
        }

        /// <summary>
        /// Grab the join status for the specified computer.
        /// </summary>
        /// <param name="joinStatus">Receives the NetworkJoinStatus value.</param>
        /// <param name="Computer">Optional name of a computer for which to retrieve the NetworkJoinStatus information.  If this parameter is null, the local computer is assumed.</param>
        /// <returns>The name of the current domain or workgroup for the specified computer.</returns>
        /// <remarks></remarks>
        public static string GrabJoin(ref NetworkJoinStatus joinStatus, string Computer = null)
        {
            using (var mm = new NetworkMemPtr())
            {
                try
                {
                    mm.Alloc(1024);
                    var nn = mm.DangerousGetHandle();
                    var result = NetGetJoinInformation(Computer, ref nn, ref joinStatus);
                    if (result == NET_API_STATUS.NERR_Success)
                    {
                        string s = (string)mm;
                        return s;
                    }
                }
                catch
                {
                    throw new NativeException();
                }

                return null;
            }
        }

        /// <summary>
        /// Enumerate users into a USER_INFO_11 structure.
        /// </summary>
        /// <param name="machine">Computer on which to perform the enumeration.  If this parameter is null, the local machine is assumed.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static UserInfo11[] EnumUsers11(string machine = null)
        {
            try
            {
                int cb = 0;
                int er = 0;

                int te = 0;

                //MemPtr buff = new MemPtr();
                UserInfo11[] usas;

                try
                {
                    cb = Marshal.SizeOf<UserInfo11>();
                }
                catch
                {
                    //Interaction.MsgBox(ex.Message + "\r\n" + "\r\n" + "Stack Trace: " + ex.StackTrace, MsgBoxStyle.Exclamation);
                }

                var inul = new nint();
                var err = NetUserEnum(machine, 11, 0, out NetworkMemPtr rh, -1, out er, out te, ref inul);

                if (err == NET_API_STATUS.NERR_Success)
                {
                    MemPtr buff = (MemPtr)rh;

                    usas = new UserInfo11[te];

                    for (int i = 0; i < te; i++)
                    {
                        usas[i] = buff.ToStruct<UserInfo11>();
                        usas[i].LogonHours = nint.Zero;

                        buff += cb;
                    }

                    return usas;
                }
            }
            catch
            {
                throw new NativeException();
            }

            return null;
        }

        /// <summary>
        /// For Windows 8, retrieves the user's Microsoft login account information.
        /// </summary>
        /// <param name="machine">Computer on which to perform the enumeration.  If this parameter is null, the local machine is assumed.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static UserInfo24[] EnumUsers24(string machine = null)
        {
            MemPtr rh = nint.Zero;

            try
            {
                int i = 0;
                var uorig = EnumUsers11();
                if (uorig == null) return null;

                UserInfo24[] usas;

                int c = uorig.Length;

                usas = new UserInfo24[c];

                for (i = 0; i < c; i++)
                {
                    var result = NetUserGetInfo(machine, uorig[i].Name, 24, ref rh);

                    if (result == NET_API_STATUS.NERR_Success)
                    {
                        usas[i] = rh.ToStruct<UserInfo24>();
                    }
                }

                return usas;
            }
            catch
            {
                throw new NativeException();
            }
            finally
            {
                rh.NetFree();
            }

            return null;
        }
    }
}