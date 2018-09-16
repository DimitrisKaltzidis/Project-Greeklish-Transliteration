namespace LinguisticTools.TextCat.Common
{
    using System;
    using System.IO;
    using System.Text;

    public class TextReaderStream : Stream
    {
        private TextReader _textReader;

        private Encoding _encoding;
        private Encoder _encoder;
        private readonly int _maxByteCountPerChar;
        char[] _charBuffer;

        public TextReaderStream(TextReader textReader, Encoding encoding, int bufferSize = 4096)
        {
            this._textReader = textReader;
            this._encoding = encoding;
            this._maxByteCountPerChar = this._encoding.GetMaxByteCount(1);
            this._encoder = encoding.GetEncoder();
            if (bufferSize <= 0) throw new ArgumentOutOfRangeException("bufferSize", "zero or negative");
            this._charBuffer = new char[bufferSize];
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count <= 0) throw new ArgumentOutOfRangeException("count", "zero or negative");
            int charsToReadLeft = count/this._maxByteCountPerChar;
            if (charsToReadLeft == 0)
                throw new ArgumentOutOfRangeException("count", "too small count, read at least " + this._maxByteCountPerChar + " bytes");
            int totalBytesWritten = 0;
            while (charsToReadLeft > 0)
            {
                int currentChunkSize = Math.Min(charsToReadLeft, this._charBuffer.Length);
                int charsRead = this._textReader.ReadBlock(this._charBuffer, 0, currentChunkSize);
                int bytes = this._encoder.GetBytes(this._charBuffer, 0, charsRead, buffer, offset + totalBytesWritten, false);
                totalBytesWritten += bytes;
                charsToReadLeft -= charsRead;
                if (charsRead < currentChunkSize)
                    break;
            }
            return totalBytesWritten;
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override void Close()
        {
            try
            {
                if (this._textReader != null)
                {
                    this._textReader.Close();
                    this._textReader = null;
                }
            }
            finally
            {
                base.Close();
            }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        #region Not supperted

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        #endregion
    }
}