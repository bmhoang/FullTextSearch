﻿using System.IO;
using System.Threading;

namespace Protsyk.PMS.FullText.Core.Common.Persistance
{
    public class MemoryStorage : StreamStorage<MemoryStream>
    {
        public MemoryStorage()
            : base(new MemoryStream()) { }

        public IPersistentStorage GetReference()
        {
            return new NonDisposable(this);
        }

        private class NonDisposable : IPersistentStorage
        {
            private readonly IPersistentStorage instance;

            public NonDisposable(IPersistentStorage instance)
            {
                this.instance = instance;
            }

            public long Length => instance.Length;

            public void Dispose()
            {
            }

            public void Flush()
            {
                instance.Flush();
            }

            public int Read(long fileOffset, byte[] buffer, int offset, int count)
            {
                return instance.Read(fileOffset, buffer, offset, count);
            }

            public void ReadAll(long fileOffset, byte[] buffer, int offset, int count)
            {
                instance.ReadAll(fileOffset, buffer, offset, count);
            }

            public void WriteAll(long fileOffset, byte[] buffer, int offset, int count)
            {
                instance.WriteAll(fileOffset, buffer, offset, count);
            }
        }
    }
}
