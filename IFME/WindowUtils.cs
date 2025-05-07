using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

/// <summary>
/// Provides a method to enable Windows acrylic blur behind a window on supported Windows 10+ systems.
/// Automatically skips execution on non-Windows platforms (e.g., Linux via Mono).
/// </summary>
public static class WindowUtils
{
    /// <summary>
    /// Enables acrylic blur behind the given window with a specified color tint.
    /// Skips execution on non-Windows platforms or unsupported Windows versions.
    /// </summary>
    /// <param name="window">The window to apply the effect to.</param>
    /// <param name="blurColor">The tint color (used in ABGR).</param>
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
            unsafe
            {
                var accentStructSize = Marshal.SizeOf<AccentPolicy>();
                SetWindowCompositionAttribute(
                    new HandleRef(window, window.Handle),
                    new WindowCompositionAttributeData
                    {
                        Attribute = WCA.ACCENT_POLICY,
                        Data = &accentPolicy,
                        DataLength = accentStructSize
                    });
            }
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
        // Windows 10 version 1803 (build 17134) or newer
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
    private static extern int SetWindowCompositionAttribute(HandleRef hWnd, in WindowCompositionAttributeData data);

    private unsafe struct WindowCompositionAttributeData
    {
        public WCA Attribute;
        public void* Data;
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
