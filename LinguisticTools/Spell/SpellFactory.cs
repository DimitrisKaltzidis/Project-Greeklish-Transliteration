namespace LinguisticTools.Spell
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class SpellFactory : IDisposable
    {
        private object hunspellsLock = new object();
        private object hyphensLock = new object();
        private object myThesesLock = new object();
        private readonly int processors;
        private volatile bool disposed;
        private Semaphore hunspellSemaphore;
        private Stack<Hunspell> hunspells;
        private Semaphore hyphenSemaphore;
        private Stack<Hyphen> hyphens;
        private Semaphore myThesSemaphore;
        private Stack<MyThes> myTheses;

        public bool IsDisposed
        {
            get
            {
                return this.disposed;
            }
        }

        public SpellFactory(LanguageConfig config)
        {
            this.processors = config.Processors;
            if (!string.IsNullOrEmpty(config.HunspellAffFile))
            {
                this.hunspells = new Stack<Hunspell>(this.processors);
                for (int index = 0; index < this.processors; ++index)
                {
                    if (config.HunspellKey != null && config.HunspellKey != string.Empty)
                        this.hunspells.Push(new Hunspell(config.HunspellAffFile, config.HunspellDictFile, config.HunspellKey));
                    else
                        this.hunspells.Push(new Hunspell(config.HunspellAffFile, config.HunspellDictFile));
                }
            }
            if (!string.IsNullOrEmpty(config.HyphenDictFile))
            {
                this.hyphens = new Stack<Hyphen>(this.processors);
                for (int index = 0; index < this.processors; ++index)
                    this.hyphens.Push(new Hyphen(config.HyphenDictFile));
            }
            if (!string.IsNullOrEmpty(config.MyThesDatFile))
            {
                this.myTheses = new Stack<MyThes>(this.processors);
                for (int index = 0; index < this.processors; ++index)
                    this.myTheses.Push(new MyThes(config.MyThesDatFile));
            }
            this.hunspellSemaphore = new Semaphore(this.processors, this.processors);
            this.hyphenSemaphore = new Semaphore(this.processors, this.processors);
            this.myThesSemaphore = new Semaphore(this.processors, this.processors);
        }

        public bool Add(string word)
        {
            bool flag = false;
            lock (this.hunspellsLock)
            {
                foreach (Hunspell item_0 in this.hunspells)
                    flag = item_0.Add(word);
                return flag;
            }
        }

        public bool AddWithAffix(string word, string example)
        {
            bool flag = false;
            lock (this.hunspellsLock)
            {
                foreach (Hunspell item_0 in this.hunspells)
                    flag = item_0.AddWithAffix(word, example);
                return flag;
            }
        }

        public bool Remove(string word)
        {
            bool flag = false;
            lock (this.hunspellsLock)
            {
                foreach (Hunspell item_0 in this.hunspells)
                    flag = item_0.Remove(word);
                return flag;
            }
        }

        private Hunspell HunspellsPop()
        {
            if (this.IsDisposed)
                throw new ObjectDisposedException("SpellFactory");
            if (this.hunspells == null)
                throw new InvalidOperationException("Hunspell Dictionary isn't loaded");
            this.hunspellSemaphore.WaitOne();
            lock (this.hunspellsLock)
                return this.hunspells.Pop();
        }

        private void HunspellsPush(Hunspell hunspell)
        {
            lock (this.hunspellsLock)
                this.hunspells.Push(hunspell);
            this.hunspellSemaphore.Release();
        }

        private Hyphen HyphenPop()
        {
            if (this.IsDisposed)
                throw new ObjectDisposedException("SpellFactory");
            if (this.hyphens == null)
                throw new InvalidOperationException("Hyphen Dictionary isn't loaded");
            this.hyphenSemaphore.WaitOne();
            lock (this.hyphensLock)
                return this.hyphens.Pop();
        }

        private void HyphenPush(Hyphen hyphen)
        {
            lock (this.hyphensLock)
                this.hyphens.Push(hyphen);
            this.hyphenSemaphore.Release();
        }

        private MyThes MyThesPop()
        {
            if (this.IsDisposed)
                throw new ObjectDisposedException("SpellFactory");
            if (this.myTheses == null)
                throw new InvalidOperationException("MyThes Dictionary isn't loaded");
            this.myThesSemaphore.WaitOne();
            lock (this.myThesesLock)
                return this.myTheses.Pop();
        }

        private void MyThesPush(MyThes myThes)
        {
            lock (this.myThesesLock)
                this.myTheses.Push(myThes);
            this.myThesSemaphore.Release();
        }

        public List<string> Analyze(string word)
        {
            Hunspell hunspell = this.HunspellsPop();
            try
            {
                return hunspell.Analyze(word);
            }
            finally
            {
                this.HunspellsPush(hunspell);
            }
        }

        public void Dispose()
        {
            if (this.IsDisposed)
                return;
            this.disposed = true;
            for (int index = 0; index < this.processors; ++index)
                this.hunspellSemaphore.WaitOne();
            if (this.hunspells != null)
            {
                foreach (Hunspell hunspell in this.hunspells)
                    hunspell.Dispose();
            }
            this.hunspellSemaphore.Release(this.processors);
            this.hunspellSemaphore = (Semaphore)null;
            this.hunspells = (Stack<Hunspell>)null;
            for (int index = 0; index < this.processors; ++index)
                this.hyphenSemaphore.WaitOne();
            if (this.hyphens != null)
            {
                foreach (Hyphen hyphen in this.hyphens)
                    hyphen.Dispose();
            }
            this.hyphenSemaphore.Release(this.processors);
            this.hyphenSemaphore = (Semaphore)null;
            this.hyphens = (Stack<Hyphen>)null;
            for (int index = 0; index < this.processors; ++index)
                this.myThesSemaphore.WaitOne();
            if (this.myTheses != null)
            {
                foreach (MyThes myThes in this.myTheses)
                    ;
            }
            this.myThesSemaphore.Release(this.processors);
            this.myThesSemaphore = (Semaphore)null;
            this.myTheses = (Stack<MyThes>)null;
        }

        public List<string> Generate(string word, string sample)
        {
            Hunspell hunspell = this.HunspellsPop();
            try
            {
                return hunspell.Generate(word, sample);
            }
            finally
            {
                this.HunspellsPush(hunspell);
            }
        }

        public HyphenResult Hyphenate(string word)
        {
            Hyphen hyphen = this.HyphenPop();
            try
            {
                return hyphen.Hyphenate(word);
            }
            finally
            {
                this.HyphenPush(hyphen);
            }
        }

        public ThesResult LookupSynonyms(string word, bool useGeneration)
        {
            MyThes myThes = (MyThes)null;
            Hunspell hunspell = (Hunspell)null;
            try
            {
                myThes = this.MyThesPop();
                if (!useGeneration)
                    return myThes.Lookup(word);
                hunspell = this.HunspellsPop();
                return myThes.Lookup(word, hunspell);
            }
            finally
            {
                if (myThes != null)
                    this.MyThesPush(myThes);
                if (hunspell != null)
                    this.HunspellsPush(hunspell);
            }
        }

        public bool Spell(string word)
        {
            Hunspell hunspell = this.HunspellsPop();
            try
            {
                return hunspell.Spell(word);
            }
            finally
            {
                this.HunspellsPush(hunspell);
            }
        }

        public List<string> Stem(string word)
        {
            Hunspell hunspell = this.HunspellsPop();
            try
            {
                return hunspell.Stem(word);
            }
            finally
            {
                this.HunspellsPush(hunspell);
            }
        }

        public List<string> Suggest(string word)
        {
            Hunspell hunspell = this.HunspellsPop();
            try
            {
                return hunspell.Suggest(word);
            }
            finally
            {
                this.HunspellsPush(hunspell);
            }
        }
    }
}
