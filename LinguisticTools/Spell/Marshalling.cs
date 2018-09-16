namespace LinguisticTools.Spell
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    internal static class MarshalHunspellDll
    {
        private static readonly object nativeDllReferenceCountLock = new object();
        private static IntPtr dllHandle = IntPtr.Zero;
        internal static HunspellAddDelegate HunspellAdd;
        internal static HunspellAddWithAffixDelegate HunspellAddWithAffix;
        internal static HunspellAnalyzeDelegate HunspellAnalyze;
        internal static HunspellFreeDelegate HunspellFree;
        internal static HunspellGenerateDelegate HunspellGenerate;
        internal static HunspellInitDelegate HunspellInit;
        internal static HunspellRemoveDelegate HunspellRemove;
        internal static HunspellSpellDelegate HunspellSpell;
        internal static HunspellStemDelegate HunspellStem;
        internal static HunspellSuggestDelegate HunspellSuggest;
        internal static HyphenFreeDelegate HyphenFree;
        internal static HyphenHyphenateDelegate HyphenHyphenate;
        internal static HyphenInitDelegate HyphenInit;
        private static string nativeDLLPath;
        private static int nativeDllReferenceCount;

        internal static string NativeDLLPath
        {
            get
            {
                if (nativeDLLPath == null)
                {
                    nativeDLLPath = Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        AppDomain.CurrentDomain.RelativeSearchPath ?? string.Empty);
                    //nativeDLLPath = nativeDLLPath.Replace("")
                }

                return nativeDLLPath;
            }
            set
            {
                if (dllHandle != IntPtr.Zero)
                    throw new InvalidOperationException("Native Library is already loaded");
                nativeDLLPath = value;
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll")]
        internal static extern void GetSystemInfo([MarshalAs(UnmanagedType.Struct)] ref SYSTEM_INFO lpSystemInfo);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr LoadLibrary(string fileName);

        //private const string HunspellX86DllName = "LinguisticTools\\Spell\\Engine\\Hunspellx86.dll";
        private const string HunspellX86DllName = "Spell\\Engine\\Hunspellx86.dll";
        //private const string HunspellX64DllName = "LinguisticTools\\Spell\\Engine\\Hunspellx64.dll";
        private const string HunspellX64DllName = "Spell\\Engine\\Hunspellx64.dll";

        internal static void ReferenceNativeHunspellDll()
        {
            lock (nativeDllReferenceCountLock)
            {
                if (nativeDllReferenceCount == 0)
                {
                    if (dllHandle != IntPtr.Zero)
                        throw new InvalidOperationException("Native Dll handle is not Zero");
                    try
                    {
                        SYSTEM_INFO systemInfo = new SYSTEM_INFO();
                        GetSystemInfo(ref systemInfo);
                        string dllPath = NativeDLLPath;
                        switch (systemInfo.wProcessorArchitecture)
                        {
                            case PROCESSOR_ARCHITECTURE.Intel:
                                dllPath = Path.Combine(dllPath, HunspellX86DllName);
                                dllHandle = LoadLibrary(dllPath);
                                if (dllHandle == IntPtr.Zero)
                                    throw new DllNotFoundException(string.Format("Hunspell Intel 32Bit DLL not found: {0}", dllPath));
                                break;
                            case PROCESSOR_ARCHITECTURE.Amd64:
                                dllPath = Path.Combine(dllPath, HunspellX64DllName);
                                dllHandle = LoadLibrary(dllPath);
                                if (dllHandle == IntPtr.Zero)
                                    throw new DllNotFoundException(string.Format("Hunspell AMD 64Bit DLL not found: {0}", dllPath));
                                break;
                            default:
                                throw new NotSupportedException("Hunspell is not available for ProcessorArchitecture: " + systemInfo.wProcessorArchitecture);
                        }
                        HunspellInit = GetDelegate<HunspellInitDelegate>("HunspellInit");
                        HunspellFree = GetDelegate<HunspellFreeDelegate>("HunspellFree");
                        HunspellAdd = GetDelegate<HunspellAddDelegate>("HunspellAdd");
                        HunspellAddWithAffix = GetDelegate<HunspellAddWithAffixDelegate>("HunspellAddWithAffix");
                        HunspellRemove = GetDelegate<HunspellRemoveDelegate>("HunspellRemove");
                        HunspellSpell = GetDelegate<HunspellSpellDelegate>("HunspellSpell");
                        HunspellSuggest = GetDelegate<HunspellSuggestDelegate>("HunspellSuggest");
                        HunspellAnalyze = GetDelegate<HunspellAnalyzeDelegate>("HunspellAnalyze");
                        HunspellStem = GetDelegate<HunspellStemDelegate>("HunspellStem");
                        HunspellGenerate = GetDelegate<HunspellGenerateDelegate>("HunspellGenerate");
                        HyphenInit = GetDelegate<HyphenInitDelegate>("HyphenInit");
                        HyphenFree = GetDelegate<HyphenFreeDelegate>("HyphenFree");
                        HyphenHyphenate = GetDelegate<HyphenHyphenateDelegate>("HyphenHyphenate");
                    }
                    catch (Exception)
                    {
                        if (dllHandle != IntPtr.Zero)
                        {
                            FreeLibrary(dllHandle);
                            dllHandle = IntPtr.Zero;
                        }
                        throw;
                    }
                }
                ++nativeDllReferenceCount;
            }
        }

        internal static void UnReferenceNativeHunspellDll()
        {
            lock (nativeDllReferenceCountLock)
            {
                if (nativeDllReferenceCount <= 0)
                    throw new InvalidOperationException("native DLL reference count is <= 0");
                --nativeDllReferenceCount;
                if (nativeDllReferenceCount != 0)
                    return;
                if (dllHandle == IntPtr.Zero)
                    throw new InvalidOperationException("Native DLL handle is Zero");
                FreeLibrary(dllHandle);
                dllHandle = IntPtr.Zero;
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        private static T GetDelegate<T>(string procName) where T : class
        {
            IntPtr procAddress = GetProcAddress(dllHandle, procName);
            if (procAddress == IntPtr.Zero)
                throw new EntryPointNotFoundException("Function: " + procName);
            return Marshal.GetDelegateForFunctionPointer(procAddress, typeof(T)) as T;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool HunspellAddDelegate(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool HunspellAddWithAffixDelegate(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word, [MarshalAs(UnmanagedType.LPWStr)] string example);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr HunspellAnalyzeDelegate(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void HunspellFreeDelegate(IntPtr handle);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr HunspellGenerateDelegate(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word, [MarshalAs(UnmanagedType.LPWStr)] string word2);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr HunspellInitDelegate([MarshalAs(UnmanagedType.LPArray)] byte[] affixData, IntPtr affixDataSize, [MarshalAs(UnmanagedType.LPArray)] byte[] dictionaryData, IntPtr dictionaryDataSize, string key);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool HunspellRemoveDelegate(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool HunspellSpellDelegate(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr HunspellStemDelegate(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr HunspellSuggestDelegate(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void HyphenFreeDelegate(IntPtr handle);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr HyphenHyphenateDelegate(IntPtr handle, [MarshalAs(UnmanagedType.LPWStr)] string word);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr HyphenInitDelegate([MarshalAs(UnmanagedType.LPArray)] byte[] dictData, IntPtr dictDataSize);

        internal enum PROCESSOR_ARCHITECTURE : ushort
        {
            Intel = (ushort)0,
            MIPS = (ushort)1,
            Alpha = (ushort)2,
            PPC = (ushort)3,
            SHX = (ushort)4,
            ARM = (ushort)5,
            IA64 = (ushort)6,
            Alpha64 = (ushort)7,
            Amd64 = (ushort)9,
            Unknown = (ushort)65535,
        }

        internal struct SYSTEM_INFO
        {
            internal PROCESSOR_ARCHITECTURE wProcessorArchitecture;
            internal ushort wReserved;
            internal uint dwPageSize;
            internal IntPtr lpMinimumApplicationAddress;
            internal IntPtr lpMaximumApplicationAddress;
            internal IntPtr dwActiveProcessorMask;
            internal uint dwNumberOfProcessors;
            internal uint dwProcessorType;
            internal uint dwAllocationGranularity;
            internal ushort dwProcessorLevel;
            internal ushort dwProcessorRevision;
        }
    }
}
