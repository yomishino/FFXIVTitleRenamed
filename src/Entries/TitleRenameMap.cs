using System;
using System.Collections.Generic;
using System.Linq;

namespace TitleRenamed.Entries
{
    public sealed class TitleRenameMap : Dictionary<string, TitleRenameEntry>, IDisposable
    {
        public TitleRenameMap() : base() { }


        public TitleRenameSaveEntry[] ToSaveEntryArray()
            => this.Select(pair => new TitleRenameSaveEntry(pair.Key, pair.Value)).ToArray();

        public void FromSaveEntryArray (TitleRenameSaveEntry[]? array, bool clearCurrent = true)
        {
            if (clearCurrent)
                Clear();
            if (array == null) return;
            foreach (var entry in array)
            {
                if (entry == null) continue;
                if (!this.TryAdd(entry.Title, new TitleRenameEntry(entry)))
                    Util.LogWarning($"Unable to add from saved entry: {entry}");
            }
        }

        public new void Add(string title, TitleRenameEntry entry)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("title is null or empty");
            if (entry == null)
                throw new ArgumentNullException(paramName: nameof(entry));
            base.Add(title, entry);
        }

        public new bool TryAdd(string title, TitleRenameEntry entry)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("title is null or empty");
            if (entry == null)
                throw new ArgumentNullException(paramName: nameof(entry));
            return base.TryAdd(title, entry);
        }

        public void Add(string title, string renamed, bool isPrefix,  bool toDisplay, bool enabled = true)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("title is null or empty");
            base.Add(title, new TitleRenameEntry(renamed, isPrefix, toDisplay, enabled));
        }

        public bool TryAdd(string title, string renamed, bool isPrefix, bool toDisplay, bool enabled = true)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("title is null or empty");
            return base.TryAdd(title, new TitleRenameEntry(renamed, isPrefix, toDisplay, enabled));
        }

        public void EnableOrDisableEntry(string title, bool enabled = true)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("title is null or empty");
            this[title].RenameEnabled = enabled;
        }

        public bool TryToggleRenameEnabled(string title, bool enabled = true)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("title is null or empty");
            if (base.TryGetValue(title, out var entry))
            {
                entry.RenameEnabled = false;
                return true;
            }
            return false;
        }

        public bool TryToggleIsPrefixTitle(string title, bool isPrefixTitle = true)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("title is null or empty");
            if (base.TryGetValue(title, out var entry))
            {
                entry.IsPrefixTitle = isPrefixTitle;
                return true;
            }
            return false;
        }

        public bool TryToggleToDisplay(string title, bool toDisplay = true)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("title is null or empty");
            if (base.TryGetValue(title, out var entry))
            {
                entry.ToDisplay = toDisplay;
                return true;
            }
            return false;
        }

        // TODO: edit renamed title

        public new bool Remove(string title)
        {
            bool removed = base.Remove(title, out var entry);
            if (removed && entry != null)
                entry.Dispose();
            return removed;
        }

        public new void Clear()
        {
            foreach (var entry in this.Values)
            {
                entry?.Dispose();
            }
            base.Clear();
        }

        public void Dispose() => Clear();
    }
}
