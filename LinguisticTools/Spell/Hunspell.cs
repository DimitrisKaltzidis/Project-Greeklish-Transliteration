namespace LinguisticTools.Spell
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;

    public class Hunspell : IDisposable
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

        public Hunspell()
        {
        }
        public Hunspell(Stream affFile, Stream dictFile, string key = null)
        {
            this.Load(affFile, dictFile, key);
        }


        public Hunspell(string affFile, string dictFile, string key = null)
        {
            this.Load(affFile, dictFile, key);
        }

        public Hunspell(byte[] affixFileData, byte[] dictionaryFileData, string key = null)
        {
            this.Load(affixFileData, dictionaryFileData, key);
        }

        private void HunspellInit(byte[] affixData, byte[] dictionaryData, string key)
        {
            if (this.unmanagedHandle != IntPtr.Zero)
                throw new InvalidOperationException("Dictionary is already loaded");
            MarshalHunspellDll.ReferenceNativeHunspellDll();
            this.nativeDllIsReferenced = true;
            this.unmanagedHandle = MarshalHunspellDll.HunspellInit(affixData, new IntPtr(affixData.Length), dictionaryData, new IntPtr(dictionaryData.Length), key);
        }

        public bool Add(string word)
        {
            if (this.unmanagedHandle == IntPtr.Zero)
                throw new InvalidOperationException("Dictionary is not loaded");
            int num = MarshalHunspellDll.HunspellAdd(this.unmanagedHandle, word) ? 1 : 0;
            return this.Spell(word);
        }

        public bool AddWithAffix(string word, string example)
        {
            if (this.unmanagedHandle == IntPtr.Zero)
                throw new InvalidOperationException("Dictionary is not loaded");
            int num = MarshalHunspellDll.HunspellAddWithAffix(this.unmanagedHandle, word, example) ? 1 : 0;
            return this.Spell(word);
        }

        public List<string> Analyze(string word)
        {
            if (this.unmanagedHandle == IntPtr.Zero)
                throw new InvalidOperationException("Dictionary is not loaded");
            List<string> list = new List<string>();
            IntPtr analyzeDelegate = MarshalHunspellDll.HunspellAnalyze(this.unmanagedHandle, word);
            int num = 0;
            for (IntPtr result = Marshal.ReadIntPtr(analyzeDelegate, num * IntPtr.Size); result != IntPtr.Zero; result = Marshal.ReadIntPtr(analyzeDelegate, num * IntPtr.Size))
            {
                ++num;
                list.Add(Marshal.PtrToStringUni(result));
            }
            return list;
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
                MarshalHunspellDll.HunspellFree(this.unmanagedHandle);
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

        public List<string> Generate(string word, string sample)
        {
            if (this.unmanagedHandle == IntPtr.Zero)
                throw new InvalidOperationException("Dictionary is not loaded");
            List<string> list = new List<string>();
            IntPtr nativeDelegate = MarshalHunspellDll.HunspellGenerate(this.unmanagedHandle, word, sample);
            int num = 0;
            for (IntPtr result = Marshal.ReadIntPtr(nativeDelegate, num * IntPtr.Size); result != IntPtr.Zero; result = Marshal.ReadIntPtr(nativeDelegate, num * IntPtr.Size))
            {
                ++num;
                list.Add(Marshal.PtrToStringUni(result));
            }
            return list;
        }

        public void Load(string affFile, string dictFile, string key = null)
        {
            affFile = Path.GetFullPath(affFile);
            if (!File.Exists(affFile))
                throw new FileNotFoundException("AFF File not found: " + affFile);
            dictFile = Path.GetFullPath(dictFile);
            if (!File.Exists(dictFile))
                throw new FileNotFoundException("DIC File not found: " + dictFile);
            byte[] affixFileData;
            using (FileStream fileStream = File.OpenRead(affFile))
            {
                using (BinaryReader binaryReader = new BinaryReader((Stream)fileStream))
                    affixFileData = binaryReader.ReadBytes((int)fileStream.Length);
            }
            byte[] dictionaryFileData;
            using (FileStream fileStream = File.OpenRead(dictFile))
            {
                using (BinaryReader binaryReader = new BinaryReader((Stream)fileStream))
                    dictionaryFileData = binaryReader.ReadBytes((int)fileStream.Length);
            }
            this.Load(affixFileData, dictionaryFileData, key);
        }

        public void Load(Stream affFile, Stream dictFile, string key = null)
        {
            byte[] affixFileData;
            using (BinaryReader binaryReader = new BinaryReader(affFile))
            {
                affixFileData = binaryReader.ReadBytes((int)affFile.Length);
            }
            byte[] dictionaryFileData;
            using (BinaryReader binaryReader = new BinaryReader(dictFile))
            {
                dictionaryFileData = binaryReader.ReadBytes((int)dictFile.Length);
            }
            this.Load(affixFileData, dictionaryFileData, key);
        }

        public void Load(byte[] affixFileData, byte[] dictionaryFileData, string key = null)
        {
            this.HunspellInit(affixFileData, dictionaryFileData, key);
        }

        public bool Remove(string word)
        {
            if (this.unmanagedHandle == IntPtr.Zero)
                throw new InvalidOperationException("Dictionary is not loaded");
            int num = MarshalHunspellDll.HunspellRemove(this.unmanagedHandle, word) ? 1 : 0;
            return !this.Spell(word);
        }

        public bool Spell(string word)
        {
            if (this.unmanagedHandle == IntPtr.Zero)
                throw new InvalidOperationException("Dictionary is not loaded");
            return MarshalHunspellDll.HunspellSpell(this.unmanagedHandle, word);
        }

        public List<string> Stem(string word)
        {
            if (this.unmanagedHandle == IntPtr.Zero)
                throw new InvalidOperationException("Dictionary is not loaded");
            List<string> list = new List<string>();
            IntPtr nativeDelegate = MarshalHunspellDll.HunspellStem(this.unmanagedHandle, word);
            int num = 0;
            for (IntPtr result = Marshal.ReadIntPtr(nativeDelegate, num * IntPtr.Size); result != IntPtr.Zero; result = Marshal.ReadIntPtr(nativeDelegate, num * IntPtr.Size))
            {
                ++num;
                list.Add(Marshal.PtrToStringUni(result));
            }
            return list;
        }

        public List<string> Suggest(string word)
        {
            if (this.unmanagedHandle == IntPtr.Zero)
                throw new InvalidOperationException("Dictionary is not loaded");
            List<string> list = new List<string>();
            IntPtr nativeDelegate = MarshalHunspellDll.HunspellSuggest(this.unmanagedHandle, word);
            int num = 0;
            for (IntPtr result = Marshal.ReadIntPtr(nativeDelegate, num * IntPtr.Size); result != IntPtr.Zero; result = Marshal.ReadIntPtr(nativeDelegate, num * IntPtr.Size))
            {
                ++num;
                list.Add(Marshal.PtrToStringUni(result));
            }
            return list;
        }
    }
}
