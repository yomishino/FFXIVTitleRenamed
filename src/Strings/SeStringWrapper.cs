using Dalamud.Game.Text.SeStringHandling;
using System;
using System.Runtime.InteropServices;

namespace TitleRenamed.Strings
{
    public class SeStringWrapper : IDisposable
    {
        public readonly SeString SeString = null!;
        internal readonly IntPtr Ptr;
        private bool disposedValue;
        public bool IsDisposed => disposedValue;

        public SeStringWrapper(SeString seStr)
        {
            SeString = seStr ?? throw new ArgumentNullException(paramName: nameof(seStr));

            byte[] bytes = seStr.Encode();
            Ptr = Marshal.AllocHGlobal(bytes.Length + 1);
            Marshal.Copy(bytes, 0, Ptr, bytes.Length);
        }

        public override string ToString()
            => $"SeString(\"{SeString.TextValue}\", @{(long)Ptr:X})";

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                if (Ptr != IntPtr.Zero)
                    Marshal.FreeHGlobal(Ptr);

                disposedValue = true;
            }
        }

        ~SeStringWrapper()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}