using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using KMJiggler.Native;

namespace KMJiggler
{
    public class WindowBlur
    {
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled",
            typeof(bool), typeof(WindowBlur), new PropertyMetadata(false, OnIsEnabledChanged));

        public static readonly DependencyProperty WindowBlurProperty = DependencyProperty.RegisterAttached("WindowBlur",
            typeof(WindowBlur), typeof(WindowBlur), new PropertyMetadata(null, OnWindowBlurChanged));

        private Window window;

        public static void SetIsEnabled(DependencyObject element, bool value) => element.SetValue(IsEnabledProperty, value);

        public static bool GetIsEnabled(DependencyObject element) => (bool) element.GetValue(IsEnabledProperty);

        public static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not Window window) return;

            if (true.Equals(e.OldValue))
            {
                GetWindowBlur(window)?.Detach();
                window.ClearValue(WindowBlurProperty);
            }

            if (!true.Equals(e.NewValue)) return;

            var blur = new WindowBlur();
            blur.Attach(window);
            window.SetValue(WindowBlurProperty, blur);
        }

        public static void SetWindowBlur(DependencyObject element, WindowBlur value) => element.SetValue(WindowBlurProperty, value);

        public static WindowBlur GetWindowBlur(DependencyObject element) => (WindowBlur) element.GetValue(WindowBlurProperty);

        private static void OnWindowBlurChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not Window window) return;

            (e.OldValue as WindowBlur)?.Detach();
            (e.NewValue as WindowBlur)?.Attach(window);
        }

        private void Attach(Window window)
        {
            this.window = window;
            var source = (HwndSource) PresentationSource.FromVisual(window);
            if (source == null)
            {
                window.SourceInitialized += this.OnSourceInitialized;
            }
            else
            {
                this.AttachCore();
            }
        }

        private void Detach()
        {
            try
            {
                this.DetachCore();
            }
            finally
            {
                this.window = null;
            }
        }

        private void OnSourceInitialized(object sender, EventArgs e)
        {
            ((Window) sender).SourceInitialized -= this.OnSourceInitialized;
            this.AttachCore();
        }

        private void AttachCore() => EnableBlur(this.window);

        private void DetachCore() => this.window.SourceInitialized += this.OnSourceInitialized;

        private static void EnableBlur(Window window)
        {
            var windowHelper = new WindowInteropHelper(window);

            var accent = new AccentPolicy
            {
                AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND
            };

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);

            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData
            {
                Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                SizeOfData = accentStructSize,
                Data = accentPtr
            };

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);
    }

    namespace Native
    {
        internal enum AccentState
        {
            ACCENT_DISABLED,
            ACCENT_ENABLE_GRADIENT,
            ACCENT_ENABLE_TRANSPARENTGRADIENT,
            ACCENT_ENABLE_BLURBEHIND,
            ACCENT_INVALID_STATE
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }

        internal struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        internal enum WindowCompositionAttribute
        {
            WCA_ACCENT_POLICY = 19
        }
    }
}