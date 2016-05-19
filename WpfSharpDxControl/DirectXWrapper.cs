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
	public class DirectXWrapper : HwndHost
	{
		private HwndSource _source;

		public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
			"Content", typeof(Visual), typeof(DirectXWrapper),
			new PropertyMetadata(OnContentChanged));

		public Visual Content
		{
			get { return (Visual)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}

		public DirectXWrapper()
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

		private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var hwndHost = (DirectXWrapper)d;

			if (e.OldValue != null)
				hwndHost.RemoveLogicalChild(e.OldValue);

			if (e.NewValue != null)
			{
				hwndHost.AddLogicalChild(e.NewValue);
				if (hwndHost._source != null)
					hwndHost._source.RootVisual = (Visual)e.NewValue;
			}
		}

		protected override HandleRef BuildWindowCore(HandleRef hwndParent)
		{
			var param = new HwndSourceParameters("DirectXWrapper", (int)Width, (int)Height)
			{
				ParentWindow = hwndParent.Handle,
				WindowStyle = NativeMethods.WS_VISIBLE | NativeMethods.WS_CHILD,
			};

			_source = new HwndSource(param)
			{
				RootVisual = Content
			};

			return new HandleRef(this, _source.Handle);
		}

		protected override void DestroyWindowCore(HandleRef hwnd)
		{
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
