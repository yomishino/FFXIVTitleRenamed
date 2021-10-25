using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Client.System.String;
using System;
using System.Text;
using System.Runtime.InteropServices;
using Dalamud.Game.Text.SeStringHandling;

namespace TitleRenamed.Strings
{
    public static class ClientStringHelper
    {
        public static char TitleLeftBracket => '《';
        public static char TitleRightBracket => '》';


        public unsafe static string GetRawString(Utf8String utf8String, bool noNewline = false)
        {
            string s = Encoding.UTF8.GetString(utf8String.StringPtr, Math.Max(0, (int)utf8String.BufUsed)).TrimEnd('\0');
            return noNewline ? s.Replace("\n", "{\\n}") : s;
        }

        public unsafe static SeString? GetSeStringFromPtr(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero) return null;
            byte b;
            int offset = 0;
            while ((b = *(byte*)(ptr + offset)) != 0)
                offset++;
            byte[] bytes = new byte[offset];
            Marshal.Copy(ptr, bytes, 0, offset);
            return SeString.Parse(bytes);
        }

        public unsafe static string GetSanitizedTitleFromSeStringPtr(IntPtr ptr)
            => ptr == IntPtr.Zero 
            ? string.Empty 
            : (GetSeStringFromPtr(ptr)?.TextValue.Trim(TitleLeftBracket).Trim(TitleRightBracket) ?? string.Empty);

        public static SeStringWrapper CreateSeString(string text)
        {
            if (text == null)
                throw new ArgumentNullException(paramName: nameof(text));
            return new SeStringWrapper(text + '\0');
        }

        public static SeStringWrapper CreateSeStringForNamePlateTitle(string titleText)
            => CreateSeString(TitleLeftBracket + titleText + TitleRightBracket);

        internal unsafe static void DisposeClientSeStringAtPtr(IntPtr ptr)
        {
            SeString? s = GetSeStringFromPtr(ptr);
            if (s!= null)
                IMemorySpace.Free(ptr.ToPointer(), (ulong)s.Encode().Length);
        }
    }
}
