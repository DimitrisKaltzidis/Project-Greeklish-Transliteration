namespace LinguisticTools.TextCat.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class StringsTextReader : TextReader
    {
        private IEnumerator<string> _strings;

        private string _s;
        private string _sNext;
        private int _pos;
        private int _length;
        private bool _finished;
        private bool _currentLineIsNewLine;
        private bool _disposed;

        public StringsTextReader(IEnumerable<string> strings)
        {
            if (strings == null) throw new ArgumentNullException("strings");
            this._strings = strings.GetEnumerator();
            this._currentLineIsNewLine = true;
            if (this._strings.MoveNext() == false)
                this._finished = true;
            else
                this._sNext = this._strings.Current;
            this._length = this._sNext.Length;
            this._pos = this._length;
        }

        protected override void Dispose(bool disposing)
        {
            if (this._strings != null)
                this._strings.Dispose();
            this._strings = null;
            this._disposed = true;
            this._s = null;
            this._pos = 0;
            this._length = 0;
            base.Dispose(disposing);
        }
        public void CheckDisposed()
        {
            if (this._disposed)
                throw new ObjectDisposedException("This object has been already disposed");
        }

        public override int Peek()
        {
            this.CheckDisposed();
            if (this.HasNoTextLeftSkippingEmpties())
                return -1;
            return this._s[this._pos];
        }

        public override int Read()
        {
            int result = this.Peek();
            this._pos++;
            return result;
        }

        public override int Read(char[] buffer, int index, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (index < 0)
                throw new ArgumentOutOfRangeException("index");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count");
            if (buffer.Length - index < count)
                throw new ArgumentException("should have: (buffer.Length - index < count) true");
            
            this.CheckDisposed();
            if (this.HasNoTextLeftSkippingEmpties())
                return 0;
            int leftChars = this._length - this._pos;
            if (leftChars > count)
                leftChars = count;
            this._s.CopyTo(this._pos, buffer, index, leftChars);
            this._pos += leftChars;
            return leftChars;
        }

        public override string ReadToEnd()
        {
            this.CheckDisposed();
            var sb = new StringBuilder();
            var buffer = new char[4*1024];
            int readChars;
            while ((readChars = this.Read(buffer, 0, buffer.Length)) > 0)
                sb.Append(buffer, 0, readChars);
            return sb.ToString();
        }

        public override string ReadLine()
        {
            this.CheckDisposed();

            if (this.HasNoTextLeftForReadLine())
                return null;

            int index;
            for (index = this._pos; index < this._length; ++index)
            {
                char ch = this._s[index];
                switch (ch)
                {
                    case '\r':
                    case '\n':
                        string str = this._s.Substring(this._pos, index - this._pos);
                        this._pos = index + 1;
                        if (ch == 13 && this._pos < this._length && this._s[this._pos] == 10)
                            this._pos++;
                        return str;
                }
            }
            if(index > this._pos)
            {
                string str = this._s.Substring(this._pos, index - this._pos);
                this._pos = index;
                return str;
            }
            return string.Empty;
        }
        private bool HasNoTextLeftSkippingEmpties()
        {
            do
            {
                if (this.HasNoTextLeft())
                    return true;
            } while (this._pos == this._length);
            return false;
        }

        private bool HasNoTextLeft()
        {
            if (this._finished)
                return true;
            if (this._pos == this._length)
            {
                if (this._currentLineIsNewLine)
                {
                    this._s = this._sNext;
                }
                else
                {
                    if (this._strings.MoveNext() == false)
                    {
                        this._finished = true;
                        return true;
                    }
                    this._sNext = this._strings.Current;
                    this._s = Environment.NewLine;
                }
                this._pos = 0;
                this._length = this._s.Length;
                this._currentLineIsNewLine = !this._currentLineIsNewLine;
            }
            return false;
        }


        private bool HasNoTextLeftForReadLine()
        {
            if (this._finished)
                return true;
            if (this._pos == this._length)
            {
                if (this._sNext != null)
                {
                    this._s = this._sNext;
                    this._sNext = null;
                    this._currentLineIsNewLine = false;
                }
                else if (this._strings.MoveNext() == false)
                {
                    this._finished = true;
                    return true;
                }
                else
                {
                    this._s = this._strings.Current;
                }
                this._pos = 0;
                this._length = this._s.Length;
            }
            return false;
        }
    }
}
