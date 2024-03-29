using TitleRenamed.Entries;
using TitleRenamed.Strings;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using System;


namespace TitleRenamed
{
    internal sealed class NameplateHelper : IDisposable
    {
        private readonly TitleRenameMap renameMap;

        private const string SetNamePlateSignature = "E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? 48 8B 5C 24 ?? 45 38 BE";
        private delegate IntPtr SetNamePlateDelegate(IntPtr namePlateObj, bool isPrefixTitle, bool displayTitle, IntPtr title, IntPtr name, IntPtr fc, IntPtr prefix, int iconId);
        [Signature(SetNamePlateSignature, DetourName = nameof(SetNamePlateDetour))]
        private readonly Hook<SetNamePlateDelegate>? SetNamePlateHook;


        internal NameplateHelper(TitleRenameMap map)
        {
            renameMap = map ?? throw new ArgumentNullException(paramName: nameof(map));
        }

        internal void EnableHook()
        {
            if (SetNamePlateHook == null || SetNamePlateHook.IsDisposed)
                throw new InvalidOperationException("SetNamePlateHook not set up or is disposed");
            SetNamePlateHook.Enable();
        }

        internal void DisableHook()
        {
            if (SetNamePlateHook == null || SetNamePlateHook.IsDisposed)
                throw new InvalidOperationException("SetNamePlateHook not set up or is disposed");
            SetNamePlateHook.Disable();
        }

        private void DisposeHook()
        {
            if (SetNamePlateHook != null && !SetNamePlateHook.IsDisposed)
            {
                SetNamePlateHook.Disable();
                SetNamePlateHook.Dispose();
            }
        }

        private unsafe IntPtr SetNamePlateDetour(IntPtr namePlateObj, bool isPrefixTitle, bool displayTitle, IntPtr title, IntPtr name, IntPtr fc, IntPtr prefix, int iconId)
        {
            IntPtr Original()
                => SetNamePlateHook!.Original(namePlateObj, isPrefixTitle, displayTitle, title, name, fc, prefix, iconId);

            if (namePlateObj == IntPtr.Zero) return Original();
            if (title == IntPtr.Zero) return Original();

            if (!displayTitle) return Original();

            string before = $"Before: \"{ClientStringHelper.GetSeStringFromPtr(title)?.TextValue ?? string.Empty}\", prefix:{isPrefixTitle}, display:{isPrefixTitle}";
            bool modified = ModifyNamePlateTitle(ref isPrefixTitle, ref displayTitle, ref title);
            string after = $"After: \"{ClientStringHelper.GetSeStringFromPtr(title)?.TextValue ?? string.Empty}\", prefix:{isPrefixTitle}, display:{isPrefixTitle}";
#if DEBUG
            if (modified)
                Util.LogDebug($"Modifying nameplate title:\n\t{before}\n\t{after}");
#endif 

            return Original();
        }

        private unsafe bool ModifyNamePlateTitle(ref bool isPrefixTitle, ref bool displayTitle, ref IntPtr title)
        {
            string oldTitle = ClientStringHelper.GetSanitizedTitleFromSeStringPtr(title);
            if (renameMap.TryGetValue(oldTitle, out var renameEntry))
            {
                if (renameEntry == null) return false;    // just in case; shouldnt be null here
                if (!renameEntry.RenameEnabled) return false;
                if (renameEntry.TitleString.IsDisposed)
                {
                    Util.LogError($"Renaming \"{oldTitle}\" to {renameEntry.RenamedTitle} failed: TitleString disposed");
                    return false;
                }
                IntPtr oldTitlePtr = title;
                title = renameEntry.TitleString.Ptr;
                isPrefixTitle = renameEntry.IsPrefixTitle;
                displayTitle = displayTitle && renameEntry.ToDisplay;   // prevent overriding to true when set to not displaying elsewhere
                ClientStringHelper.DisposeClientSeStringAtPtr(oldTitlePtr);
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            DisposeHook();
        }
    }
}
