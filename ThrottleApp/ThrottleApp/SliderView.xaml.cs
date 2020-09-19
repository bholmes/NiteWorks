using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ThrottleApp
{
	[XamlCompilation (XamlCompilationOptions.Compile)]
	public partial class SliderView : ContentView
	{
		public static readonly BindableProperty IndicatorValueProperty = BindableProperty.Create (nameof (IndicatorValue), typeof (float), typeof (SliderView), default (float), defaultBindingMode: BindingMode.OneWay);
		public float IndicatorValue { get => (float)GetValue (IndicatorValueProperty); set => SetValue (IndicatorValueProperty, value); }

		public static readonly BindableProperty IndicatorValueRequestProperty = BindableProperty.Create (nameof (IndicatorValueRequest), typeof (float), typeof (SliderView), default (float), defaultBindingMode: BindingMode.OneWayToSource);
		public float IndicatorValueRequest { get => (float)GetValue (IndicatorValueRequestProperty); set => SetValue (IndicatorValueRequestProperty, value); }

		public static readonly BindableProperty SensitivityFactorProperty = BindableProperty.Create (nameof (SensitivityFactor), typeof (float), typeof (SliderView), default (float));
		public float SensitivityFactor { get => (float)GetValue (SensitivityFactorProperty); set => SetValue (SensitivityFactorProperty, value); }

		public static readonly BindableProperty InvertProperty = BindableProperty.Create (nameof (Invert), typeof (bool), typeof (SliderView), default (bool), defaultBindingMode: BindingMode.OneWay);
		public bool Invert { get => (bool)GetValue (InvertProperty); set => SetValue (InvertProperty, value); }

		const float heightMargin = .1f;
		const float widthMargin = .05f;

		float trackingStartPos = 0f;
		bool trackingPan = false;

		public SliderView ()
		{
			IndicatorValue = 0;
			SensitivityFactor = 1f;
			InitializeComponent ();
		}

		protected override void OnPropertyChanged ([CallerMemberName] string propertyName = null)
		{
			base.OnPropertyChanged (propertyName);

			if (propertyName == nameof (IndicatorValue))
				skView.InvalidateSurface ();
		}

		void OnCanvasViewPaintSurface (object sender, SKPaintSurfaceEventArgs args)
		{
			var info = args.Info;
			var surface = args.Surface;
			var canvas = surface.Canvas;

			canvas.Clear ();

			var fillPaint = new SKPaint
			{
				Style = SKPaintStyle.Fill,
				Color = BackgroundColor.ToSKColor (),
			};

			var line1Paint = new SKPaint
			{
				Style = SKPaintStyle.Stroke,
				Color = Color.Gray.ToSKColor (),
				StrokeWidth = 5,
			};

			canvas.DrawRect (0, 0, info.Width, info.Height, fillPaint);

			var marginXValue = info.Width * widthMargin;
			var marginYValue = info.Height * heightMargin;

			var lineYPos = ((1f - 2 * heightMargin) * IndicatorValue + heightMargin) * info.Height;
			if (!Invert)
				lineYPos = info.Height - lineYPos;
			canvas.DrawLine (marginXValue, lineYPos, info.Width - marginXValue, lineYPos, line1Paint);

			var midX = info.Width / 2f;
			canvas.DrawLine (midX, marginYValue, midX, info.Height - marginYValue, line1Paint);
		}
		private void PanUpdated (object sender, PanUpdatedEventArgs e)
		{
			switch (e.StatusType)
			{
				case GestureStatus.Started:
					trackingPan = true;
					trackingStartPos = IndicatorValue;
					break;
				case GestureStatus.Running:
					if (trackingPan)
					{
						var tVal = trackingStartPos + ((float)(SensitivityFactor * e.TotalY * (Invert ? 1f : -1f) / Height)) / (1 - 2f * heightMargin);

						if (tVal > 1) tVal = 1f;
						else if (tVal < 0) tVal = 0;
						if (tVal != IndicatorValueRequest)
						{
							IndicatorValueRequest = tVal;
						}
					}
					break;
				case GestureStatus.Completed:
				case GestureStatus.Canceled:
					trackingPan = false;
					break;
				default:
					break;
			}
		}


	}
}