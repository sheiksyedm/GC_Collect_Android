using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Graphics;
using Android.Content;
using System.Threading;
using System;
using Android.Util;

namespace GC_Collect_Android
{
    [Activity(Label = "GC_Collect_Android", MainLauncher = true)]
    public class MainActivity : Activity
    {
        Timer _timer;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            DisplayMetrics displayMetrics = Resources.DisplayMetrics;
            float screenHeight = displayMetrics.HeightPixels;
            base.OnCreate(savedInstanceState);
            _timer = new Timer(timer_Tick, null, 0, 1000 * 5);
            var mainLayout = new MainLayout(this);
            mainLayout.DrawColor = Color.Yellow;
            mainLayout.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, 800);
            SetContentView(mainLayout);
        }
        private void timer_Tick(object state)
        {
            GC.Collect();
        }
      
    }

    /// <summary>
    /// Main layout
    /// </summary>
    public class MainLayout : FrameLayout
    {
        internal SubLayout SubLayoutView;
        internal DrawLayout DrawLayoutView;
        public Color DrawColor { get; set; }
        public MainLayout(Context context) : base(context)
        {
            SubLayoutView = new SubLayout(context);
            SubLayoutView.MainLayout = this;
            DrawLayoutView = new DrawLayout(context);
            DrawLayoutView.MainLayout = this;
            SubLayoutView.AddView(DrawLayoutView);
            this.AddView(SubLayoutView);
        }
    }

    /// <summary>
    /// Sub layout 
    /// </summary>
    public class SubLayout : FrameLayout
    {
        private ScaleGestureDetector scaleDetector;
        internal MainLayout MainLayout;
        public SubLayout(Context context) : base(context)
        {
            this.scaleDetector = new ScaleGestureDetector(context, new ScaleListener(this));
            this.SetWillNotDraw(false);
            base.Clickable = true;
        }
        public override bool OnTouchEvent(MotionEvent e)
        {
            this.scaleDetector.OnTouchEvent(e);
            MainLayout.DrawLayoutView.OnTouchEvent(e);
            return base.OnTouchEvent(e);
        }
    }

    /// <summary>
    /// Drawing layout 
    /// </summary>
    public class DrawLayout:FrameLayout
    {
        internal Path CustomPath { get; set; }
        float xValue, yValue;
        internal MainLayout MainLayout;
        Paint paint;
        public DrawLayout(Context context) : base(context)
        {
            this.SetWillNotDraw(false);
            paint = new Paint();
            SetBackgroundColor(Color.Red);
            this.Focusable = true;
        }
        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            float[] values = new float[9];
            paint.Color = MainLayout.DrawColor;
            paint.AntiAlias = true;
            paint.Dither = true;
            paint.StrokeJoin = Paint.Join.Round;
            paint.StrokeCap = Paint.Cap.Round;
            paint.FilterBitmap = true;
            paint.SetStyle(Paint.Style.Stroke);
            paint.Flags = PaintFlags.AntiAlias;
            paint.StrokeWidth = 5;
            if (CustomPath != null)
                canvas.DrawPath(CustomPath, paint);
        }
        public override bool OnTouchEvent(MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:

                    CustomPath = new Path();
                    xValue = e.GetX();
                    yValue = e.GetY();
                    CustomPath.MoveTo(xValue, yValue);
                    break;
                case MotionEventActions.Move:
                    float x2 = (e.GetX() + this.xValue) / 2;
                    float y2 = (e.GetY() + this.yValue) / 2;
                    CustomPath.QuadTo(this.xValue, this.yValue, x2, y2);
                   
                    this.xValue = e.GetX();
                    this.yValue = e.GetY();
                    float[] points = new float[6];
                    break;
            }
            this.Invalidate();
            return base.OnTouchEvent(e);
        }
    }
    public class ScaleListener : ScaleGestureDetector.SimpleOnScaleGestureListener
    {
        private FrameLayout subLayout;
        public ScaleListener(FrameLayout layout)
        {
            this.subLayout = layout;
        }
    }
   
    
}

