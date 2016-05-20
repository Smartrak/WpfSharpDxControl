using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using SharpDX;

namespace WpfSharpDxControl
{
	/// <summary>
	/// Creates internal Hwnd to host DirectXComponent within a control in the window.
	/// </summary>
	public class HwndWrapper : HwndHost
	{
		protected IntPtr Hwnd { get; private set; }
		private HwndSource _source;

		protected HwndWrapper()
		{
			Unloaded += OnUnloaded;
		}

		protected override void Dispose(bool disposing)
		{
			Unloaded -= OnUnloaded;

			base.Dispose(disposing);
		}

		private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
		{
			Dispose();
		}

		protected override HandleRef BuildWindowCore(HandleRef hwndParent)
		{
			_source = new HwndSource(new HwndSourceParameters("DirectXWrapper", (int)Width, (int)Height)
			{
				ParentWindow = hwndParent.Handle,
				WindowStyle = NativeMethods.WS_VISIBLE | NativeMethods.WS_CHILD,
			});

			Hwnd = _source.Handle;

			return new HandleRef(this, _source.Handle);
		}

		protected override void DestroyWindowCore(HandleRef hwnd)
		{
			Hwnd = IntPtr.Zero;

			Utilities.Dispose(ref _source);
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			UpdateWindowPos();

			base.OnRenderSizeChanged(sizeInfo);
		}
	}

	internal class NativeMethods
	{
		public const int WS_CHILD = 0x40000000;
		public const int WS_VISIBLE = 0x10000000;
	}
}
