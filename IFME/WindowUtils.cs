using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

public static class WindowUtils
{
    public static void EnableAcrylic(IWin32Window window, Color blurColor)
    {
        if (!IsWindowsPlatform())
            return;

        if (window is null)
            throw new ArgumentNullException(nameof(window));

        if (window.Handle == IntPtr.Zero)
            throw new InvalidOperationException("Window handle is not initialized.");

        if (!IsAcrylicSupported())
            return;

        var accentPolicy = new AccentPolicy
        {
            AccentState = ACCENT.ENABLE_ACRYLICBLURBEHIND,
            AccentFlags = (uint)AccentFlags.DrawAllBorders,
            GradientColor = ToAbgr(blurColor),
            AnimationId = 0
        };

        try
        {
            int accentStructSize = Marshal.SizeOf<AccentPolicy>();
            IntPtr accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accentPolicy, accentPtr, false);

            var data = new WindowCompositionAttributeData
            {
                Attribute = WCA.ACCENT_POLICY,
                Data = accentPtr,
                DataLength = accentStructSize
            };

            SetWindowCompositionAttribute(new HandleRef(window, window.Handle), data);
            Marshal.FreeHGlobal(accentPtr);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"EnableAcrylic failed: {ex.Message}");
        }
    }

    private static bool IsWindowsPlatform()
    {
        return Environment.OSVersion.Platform == PlatformID.Win32NT;
    }

    private static bool IsAcrylicSupported()
    {
        return Environment.OSVersion.Version >= new Version(10, 0, 17134);
    }

    private static uint ToAbgr(Color color)
    {
        return ((uint)color.A << 24)
             | ((uint)color.B << 16)
             | ((uint)color.G << 8)
             | color.R;
    }

    [DllImport("user32.dll")]
    private static extern int SetWindowCompositionAttribute(HandleRef hWnd, WindowCompositionAttributeData data);

    [StructLayout(LayoutKind.Sequential)]
    private struct WindowCompositionAttributeData
    {
        public WCA Attribute;
        public IntPtr Data;
        public int DataLength;
    }

    private enum WCA
    {
        ACCENT_POLICY = 19
    }

    private enum ACCENT
    {
        DISABLED = 0,
        ENABLE_GRADIENT = 1,
        ENABLE_TRANSPARENTGRADIENT = 2,
        ENABLE_BLURBEHIND = 3,
        ENABLE_ACRYLICBLURBEHIND = 4,
        INVALID_STATE = 5
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct AccentPolicy
    {
        public ACCENT AccentState;
        public uint AccentFlags;
        public uint GradientColor;
        public uint AnimationId;
    }

    [Flags]
    private enum AccentFlags
    {
        DrawLeftBorder = 0x20,
        DrawTopBorder = 0x40,
        DrawRightBorder = 0x80,
        DrawBottomBorder = 0x100,
        DrawAllBorders = DrawLeftBorder | DrawTopBorder | DrawRightBorder | DrawBottomBorder
    }
}
