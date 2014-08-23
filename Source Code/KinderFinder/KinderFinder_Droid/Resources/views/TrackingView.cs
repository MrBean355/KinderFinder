using Android.Content;
using Android.Views;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Graphics.Drawables.Shapes;

using System.Collections.Generic;

namespace KinderFinder_Droid {

	class TrackingDot {
		const int DOT_SIZE = 15;
		const int DOT_COLOUR_RED = 255;
		const int DOT_COLOUR_GREEN = 200;
		const int DOT_COLOUR_BLUE = 255;

		readonly ShapeDrawable Shape;
		int PosX = 0, PosY = 0;

		public TrackingDot() {
			var paint = new Paint();
			paint.SetARGB(DOT_COLOUR_RED, DOT_COLOUR_GREEN, DOT_COLOUR_BLUE, 0);
			paint.SetStyle(Paint.Style.Fill);

			Shape = new ShapeDrawable(new OvalShape());
			Shape.Paint.Set(paint);
			Shape.SetBounds(PosX, PosY, PosX + DOT_SIZE, PosY + DOT_SIZE);
		}

		public void Move(int x, int y) {
			PosX = x;
			PosY = y;
			Shape.SetBounds(PosX, PosY, PosX + DOT_SIZE, PosY + DOT_SIZE);
		}

		public void Draw(Canvas canvas) {
			Shape.Draw(canvas);
		}
	}

	public class TrackingView : View {
		const int DOT_SIZE = 20;

		List<TrackingDot> Dots;

		public TrackingView(Context context) : base(context) {
			Dots = new List<TrackingDot>();

			for (int i = 0; i < 10; i++) {
				TrackingDot dot = new TrackingDot();
				dot.Move(i * 25, i * 25);

				Dots.Add(dot);
			}
		}

		protected override void OnDraw(Canvas canvas) {
			foreach (var dot in Dots)
				dot.Draw(canvas);
		}
	}
}

