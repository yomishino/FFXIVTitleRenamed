using TitleRenamed.Strings;
using System;

namespace TitleRenamed.Entries
{
    public class TitleRenameEntry : IDisposable
    {
        public string RenamedTitle { get; private set; }
        public bool IsPrefixTitle { get; internal set; }
        public bool ToDisplay { get; internal set; }
        public bool RenameEnabled { get; internal set; }
        internal SeStringWrapper TitleString { get; private set; }


        public TitleRenameEntry(string renamed, bool isPrefix, bool toDisplay, bool enabled)
        {
            RenamedTitle = renamed ?? string.Empty;
            IsPrefixTitle = isPrefix;
            ToDisplay = toDisplay;
            RenameEnabled = enabled;
            TitleString = ClientStringHelper.CreateSeStringForNamePlateTitle(RenamedTitle);
        }

        public TitleRenameEntry(TitleRenameSaveEntry entry)
            : this(entry.RenamedTitle, entry.IsPrefixTitle, entry.ToDisplay, entry.RenameEnabled) { }

        public override string ToString()
            => $"RenamedTo:{RenamedTitle},IsPrefix:{IsPrefixTitle},ToDisplay:{ToDisplay},Enabled:{RenameEnabled},SeStr@{(long)TitleString.Ptr:X}";


        #region IDisposable impl

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //dispose managed state(managed objects)
                    TitleString.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                
                // set large fields to null

                disposedValue = true;
            }
        }

        //// Override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        //~TitleRenameEntry()
        //{
        //    // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //    Dispose(disposing: false);
        //}

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
