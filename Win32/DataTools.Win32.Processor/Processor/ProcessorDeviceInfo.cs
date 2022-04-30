using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTools.Win32;
using DataTools.Hardware;
using DataTools.SystemInformation;
using DataTools.Text;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections.ObjectModel;
using static DataTools.Win32.DeviceEnum;

namespace DataTools.Hardware.Processor
{
    public class ProcessorDeviceInfo : DeviceInfo
    {
        private IReadOnlyCollection<CacheInfo> cacheInfo;
        private SystemLogicalProcessorInformation source;




        public static ProcessorDeviceInfo[] EnumProcessors()
        {
            var p = InternalEnumerateDevices<ProcessorDeviceInfo>(DevProp.GUID_DEVCLASS_PROCESSOR, ClassDevFlags.Present);

            if (p != null && p.Length > 0)
            {
                if (p is null) return null;

                var procs = SysInfo.LogicalProcessors;

                Array.Sort(procs, new Comparison<SystemLogicalProcessorInformation>((x, y) =>
                {
                    if (x.Relationship == y.Relationship)
                    {
                        return (int)(x.ProcessorMask - y.ProcessorMask);
                    }
                    else
                    {
                        return ((int)x.Relationship - (int)y.Relationship);
                    }

                }));

                Array.Sort(p, new Comparison<ProcessorDeviceInfo>((x, y) =>
                {
                    return string.Compare(x.InstanceId, y.InstanceId);
                }));

                int c = p.Length;
                int d = procs.Length;

                List<CacheInfo>[] pci = new List<CacheInfo>[c];

                for (int i = 0; i < c; i++)
                {
                    ProcessorDeviceInfo pInfo = p[i];

                    pInfo.LogicalProcessor = (i + 1);

                    int ccore = 1;
                    foreach (var proc in procs)
                    {
                        if ((proc.ProcessorMask & (1 << i)) == (1 << i))
                        {
                            if ((proc.Relationship | LogicalProcessorRelationship.RelationProcessorCore) == LogicalProcessorRelationship.RelationProcessorCore)
                            {
                                pInfo.Core = ccore;
                                pInfo.Source = proc;
                            }
                            else if ((proc.Relationship | LogicalProcessorRelationship.RelationCache) == LogicalProcessorRelationship.RelationCache)
                            {
                                var cd = new CacheInfo(proc.CacheDescriptor);

                                if (pci[i] == null)
                                {
                                    pci[i] = new List<CacheInfo>();
                                }

                                switch (cd.Level)
                                {
                                    case 1:
                                        pInfo.HasL1Cache = true;
                                        break;

                                    case 2:
                                        pInfo.HasL2Cache = true;
                                        break;

                                    case 3:
                                        pInfo.HasL3Cache = true;
                                        break;
                                }

                                pInfo.TotalCacheSize += cd.Size;
                                pInfo.TotalLineSize += cd.LineSize;

                                pci[i].Add(cd);
                            }
                        }

                        if ((proc.Relationship | LogicalProcessorRelationship.RelationProcessorCore) == LogicalProcessorRelationship.RelationProcessorCore)
                        {
                            ccore++;
                        }
                    }

                    pInfo.Caches = new ReadOnlyCollection<CacheInfo>(pci[i]);
                }

            }

            return p;

        }


        internal SystemLogicalProcessorInformation Source
        {
            get => source;
            set => source = value;
        }

        private long totalLineSize;

        public long TotalLineSize
        {
            get => totalLineSize;
            internal set
            {
                totalLineSize = value;
            }
        }
        
        private long totalCacheSize;

        public long TotalCacheSize
        {
            get => totalCacheSize;
            internal set
            {
                totalCacheSize = value;
            }
        }

        public IReadOnlyCollection<CacheInfo> Caches
        {
            get => cacheInfo;
            internal set
            {
                cacheInfo = value;
            }
        }

        private bool hasL1cache;

        public bool HasL1Cache
        {
            get => hasL1cache;
            internal set
            {
                hasL1cache = value;
            }
        }

        public CacheInfo L1DataCache
        {
            get
            {
                foreach (var c in Caches)
                {
                    if (c.Level == 1 && c.Type == ProcessorCacheType.CacheData)
                    {
                        return c;
                    } 
                }

                return null;
            }
        }

        public CacheInfo L1InstructionCache
        {
            get
            {
                foreach (var c in Caches)
                {
                    if (c.Level == 1 && c.Type == ProcessorCacheType.CacheInstruction)
                    {
                        return c;
                    }
                }

                return null;
            }
        }

        private bool hasL2cache;

        public bool HasL2Cache
        {
            get => hasL2cache;
            internal set
            {
                hasL2cache = value;
            }
        }


        public CacheInfo L2Cache
        {
            get
            {
                foreach (var c in Caches)
                {
                    if (c.Level == 2) return c;
                }

                return null;
            }
        }

        private bool hasL3cache;

        public bool HasL3Cache
        {
            get => hasL3cache;
            internal set
            {
                hasL3cache = value;
            }
        }

        public CacheInfo L3Cache
        {
            get
            {
                foreach (var c in Caches)
                {
                    if (c.Level == 3) return c;
                }

                return null;
            }
        }

        private int core;

        public int Core
        {
            get => core;
            internal set
            {
                core = value;
            }
        }

        private int logProc;

        public int LogicalProcessor
        {
            get => logProc;
            internal set
            {
                logProc = value;
            }
        }

        public override string Description
        {
            get => ToString();
            internal set
            {
                base.Description = value;
            }
        }

        public override string ToString()
        {
            try
            {
                string s = FriendlyName + $", Logical Processor: {LogicalProcessor}, Core: {Core}";
                if (HasL1Cache)
                {
                    s += $", L1 ({TextTools.PrintFriendlySize(L1InstructionCache.Size)})";
                }
                if (HasL2Cache)
                {
                    s += $", L2 ({TextTools.PrintFriendlySize(L2Cache.Size)})";
                }
                if (HasL3Cache)
                {
                    s += $", L3 ({TextTools.PrintFriendlySize(L3Cache.Size)})";
                }
                return s;
            }
            catch
            {

            }
            
            return base.ToString();
        }
    }
}
