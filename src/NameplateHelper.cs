using TitleRenamed.Entries;
using TitleRenamed.Strings;

using Dalamud.Hooking;
using System;


namespace TitleRenamed
{
    internal sealed class NameplateHelper : IDisposable
    {
        private readonly TitleRenameMap renameMap;

        private const string SetNamePlateSignature = "48 89 5C 24 ?? 48 89 6C 24 ?? 56 57 41 54 41 56 41 57 48 83 EC 40 44 0F B6 E2";
        private delegate IntPtr SetNamePlateDelegate(IntPtr namePlateObj, bool isPrefixTitle, bool displayTitle, IntPtr title, IntPtr name, IntPtr fc, int iconId);
        private readonly IntPtr SetNamePlatePtr;
        private readonly Hook<SetNamePlateDelegate>? SetNamePlateHook;


        internal NameplateHelper(TitleRenameMap map)
        {
            renameMap = map ?? throw new ArgumentNullException(paramName: nameof(map));
            SetNamePlatePtr = Plugin.SigScanner.ScanText(SetNamePlateSignature);
            SetNamePlateHook = new Hook<SetNamePlateDelegate>(SetNamePlatePtr, SetNamePlateDetour);
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

        private unsafe IntPtr SetNamePlateDetour(IntPtr namePlateObj, bool isPrefixTitle, bool displayTitle, IntPtr title, IntPtr name, IntPtr fc, int iconId)
        {
            IntPtr Original()
                => SetNamePlateHook!.Original(namePlateObj, isPrefixTitle, displayTitle, title, name, fc, iconId);

            if (namePlateObj == IntPtr.Zero) return Original();
            if (title == IntPtr.Zero) return Original();

            if (!displayTitle) return Original();

            string before = $"Before: \"{ClientStringHelper.GetSeStringFromPtr(title)?.TextValue ?? string.Empty}\", prefix:{isPrefixTitle}, display:{isPrefixTitle}";
            bool modified = ModifyNamePlateTitle(ref isPrefixTitle, ref displayTitle, ref title);
            string after = $"After: \"{ClientStringHelper.GetSeStringFromPtr(title)?.TextValue ?? string.Empty}\", prefix:{isPrefixTitle}, display:{isPrefixTitle}";
#if DEBUG
            //if (modified)
            //    Util.LogDebug($"Modifying nameplate title:\n\t{before}\n\t{after}");
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
                title = renameEntry.TitleString.Ptr;
                isPrefixTitle = renameEntry.IsPrefixTitle;
                displayTitle = displayTitle && renameEntry.ToDisplay;   // prevent overriding to true when set to not displaying elsewhere
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
