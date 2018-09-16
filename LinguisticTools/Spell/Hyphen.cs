namespace LinguisticTools.Spell
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    public class Hyphen : IDisposable
    {
        private bool nativeDllIsReferenced;
        private IntPtr unmanagedHandle;

        public static string NativeDllPath
        {
            get
            {
                return MarshalHunspellDll.NativeDLLPath;
            }
            set
            {
                MarshalHunspellDll.NativeDLLPath = value;
            }
        }

        public bool IsDisposed { get; private set; }

        public Hyphen()
        {
        }

        public Hyphen(string dictFile)
        {
            this.Load(dictFile);
        }

        public Hyphen(byte[] dictFileData)
        {
            this.Load(dictFileData);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        public void Dispose(bool callFromDispose)
        {
            if (this.IsDisposed)
                return;
            this.IsDisposed = true;
            if (this.unmanagedHandle != IntPtr.Zero)
            {
                MarshalHunspellDll.HyphenFree(this.unmanagedHandle);
                this.unmanagedHandle = IntPtr.Zero;
            }
            if (this.nativeDllIsReferenced)
            {
                MarshalHunspellDll.UnReferenceNativeHunspellDll();
                this.nativeDllIsReferenced = false;
            }
            if (!callFromDispose)
                return;
            GC.SuppressFinalize((object)this);
        }

        public HyphenResult Hyphenate(string word)
        {
            if (this.unmanagedHandle == IntPtr.Zero)
                throw new InvalidOperationException("Dictionary is not loaded");
            if (word == null || word == string.Empty)
                return (HyphenResult)null;
            IntPtr ptr1 = MarshalHunspellDll.HyphenHyphenate(this.unmanagedHandle, word);
            IntPtr ptr2 = Marshal.ReadIntPtr(ptr1);
            int size = IntPtr.Size;
            IntPtr ptr3 = Marshal.ReadIntPtr(ptr1, size);
            int ofs1 = size + IntPtr.Size;
            IntPtr ptr4 = Marshal.ReadIntPtr(ptr1, ofs1);
            int ofs2 = ofs1 + IntPtr.Size;
            IntPtr ptr5 = Marshal.ReadIntPtr(ptr1, ofs2);
            int ofs3 = ofs2 + IntPtr.Size;
            IntPtr ptr6 = Marshal.ReadIntPtr(ptr1, ofs3);
            int num = ofs3 + IntPtr.Size;
            byte[] hyphenationPoints = new byte[Math.Max(word.Length - 1, 1)];
            string[] hyphenationRep = new string[Math.Max(word.Length - 1, 1)];
            int[] hyphenationPos = new int[Math.Max(word.Length - 1, 1)];
            int[] hyphenationCut = new int[Math.Max(word.Length - 1, 1)];
            for (int ofs4 = 0; ofs4 < word.Length - 1; ++ofs4)
            {
                hyphenationPoints[ofs4] = Marshal.ReadByte(ptr3, ofs4);
                if (ptr4 != IntPtr.Zero)
                {
                    IntPtr ptr7 = Marshal.ReadIntPtr(ptr4, ofs4 * IntPtr.Size);
                    if (ptr7 != IntPtr.Zero)
                        hyphenationRep[ofs4] = Marshal.PtrToStringUni(ptr7);
                    hyphenationPos[ofs4] = Marshal.ReadInt32(ptr5, ofs4 * 4);
                    hyphenationCut[ofs4] = Marshal.ReadInt32(ptr6, ofs4 * 4);
                }
            }
            return new HyphenResult(Marshal.PtrToStringUni(ptr2), hyphenationPoints, hyphenationRep, hyphenationPos, hyphenationCut);
        }

        public void Load(string dictFile)
        {
            dictFile = Path.GetFullPath(dictFile);
            if (!File.Exists(dictFile))
                throw new FileNotFoundException("DIC File not found: " + dictFile);
            byte[] dictFileData;
            using (FileStream fileStream = File.OpenRead(dictFile))
            {
                using (BinaryReader binaryReader = new BinaryReader((Stream)fileStream))
                    dictFileData = binaryReader.ReadBytes((int)fileStream.Length);
            }
            this.Load(dictFileData);
        }

        public void Load(byte[] dictFileData)
        {
            this.Init(dictFileData);
        }

        private void Init(byte[] dictionaryData)
        {
            if (this.unmanagedHandle != IntPtr.Zero)
                throw new InvalidOperationException("Dictionary is already loaded");
            MarshalHunspellDll.ReferenceNativeHunspellDll();
            this.nativeDllIsReferenced = true;
            this.unmanagedHandle = MarshalHunspellDll.HyphenInit(dictionaryData, new IntPtr(dictionaryData.Length));
        }
    }
}
