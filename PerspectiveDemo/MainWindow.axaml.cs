// https://github.com/AvaloniaUI/Avalonia/pull/6192
// https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/transforms/non-affine
// http://www.charlespetzold.com/blog/2007/08/250638.html
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;

namespace PerspectiveDemo
{
    public class CustomCanvas : Canvas
    {
        public SKRect _rect;
        public SKMatrix _matrix;

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            
            context.Custom(new SkiaCustomDraw(_rect, _matrix));
        }

        class SkiaCustomDraw : ICustomDrawOperation
        {
            private SKRect _rect;
            private SKMatrix _matrix;

            public SkiaCustomDraw(SKRect rect, SKMatrix matrix)
            {
                _rect = rect;
                _matrix = matrix;
            }
            
            public void Dispose()
            {
            }

            public bool HitTest(Point p)
            {
                return false;
            }

            public void Render(IDrawingContextImpl context)
            {
                if (context is ISkiaDrawingContextImpl drawingContextImpl)
                {
                    var canvas = drawingContextImpl.SkCanvas;

                    canvas.Save();
                    var result = canvas.TotalMatrix;
                    result = result.PreConcat(_matrix);
                    canvas.SetMatrix(result);
                    //canvas.SetMatrix(_matrix);
                    canvas.DrawRect(_rect, new SKPaint() { Style = SKPaintStyle.Fill, Color = SKColors.Red});
                    canvas.Restore();
                }
            }

            public Rect Bounds { get; }
            public bool Equals(ICustomDrawOperation? other)
            {
                return false;
            }
        }
    }
    
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            //UpdateTransform();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void UpdateTransform()
        {
            var canvas = this.FindControl<CustomCanvas>("Canvas");
            var rectangle = this.FindControl<Rectangle>("Rectangle");

            var ul = this.FindControl<Thumb>("UL");
            var ur = this.FindControl<Thumb>("UR");
            var ll = this.FindControl<Thumb>("LL");
            var lr = this.FindControl<Thumb>("LR");

            var ptUL = new SKPoint((float)Canvas.GetLeft(ul), (float)Canvas.GetTop(ul));
            var ptUR = new SKPoint((float)Canvas.GetLeft(ur), (float)Canvas.GetTop(ur));
            var ptLL = new SKPoint((float)Canvas.GetLeft(ll), (float)Canvas.GetTop(ll));
            var ptLR = new SKPoint((float)Canvas.GetLeft(lr), (float)Canvas.GetTop(lr));

            canvas._rect = SKRect.Create(
                (float)Canvas.GetLeft(rectangle),
                (float)Canvas.GetTop(rectangle),
                (float)rectangle.Width,
                (float)rectangle.Height);

            canvas._matrix = ComputeMatrix(canvas._rect, ptUL, ptUR, ptLL, ptLR);
            
            canvas.InvalidateVisual();
        }

        static SKPoint MapPoint(SKMatrix matrix, SKPoint point)
        {
            return new SKPoint(
                (point.X * matrix.ScaleX) + (point.Y * matrix.SkewX) + matrix.TransX,
                (point.X * matrix.SkewY) + (point.Y * matrix.ScaleY) + matrix.TransY);
        }

        static SKMatrix ComputeMatrix(SKRect rect, SKPoint ptUL, SKPoint ptUR, SKPoint ptLL, SKPoint ptLR)
        {
            // Scale transform
            SKMatrix S = SKMatrix.MakeScale(1 / rect.Size.Width, 1 / rect.Size.Height);
            //SKMatrix T = SKMatrix.MakeTranslation(1 / -rect.Left, 1 / -rect.Top);

            // Affine transform
            SKMatrix A = new SKMatrix
            {
                ScaleX = ptUR.X - ptUL.X,
                SkewY = ptUR.Y - ptUL.Y,
                SkewX = ptLL.X - ptUL.X,
                ScaleY = ptLL.Y - ptUL.Y,
                TransX = ptUL.X,
                TransY = ptUL.Y,
                Persp2 = 1
            };

            // Non-Affine transform
            SKMatrix inverseA;
            A.TryInvert(out inverseA);
            SKPoint abPoint = inverseA.MapPoint(ptLR);
            float a = abPoint.X;
            float b = abPoint.Y;

            float scaleX = a / (a + b - 1);
            float scaleY = b / (a + b - 1);

            SKMatrix N = new SKMatrix
            {
                ScaleX = scaleX,
                ScaleY = scaleY,
                Persp0 = scaleX - 1,
                Persp1 = scaleY - 1,
                Persp2 = 1
            };

            // Multiply S * N * A
            SKMatrix result = SKMatrix.MakeIdentity();
            SKMatrix.PostConcat(ref result, S);
            //SKMatrix.PostConcat(ref result, T);
            SKMatrix.PostConcat(ref result, N);
            SKMatrix.PostConcat(ref result, A);

            return result;
        }
        
        static SKMatrix ComputeMatrix2(SKSize size, SKPoint ptUL, SKPoint ptUR, SKPoint ptLL, SKPoint ptLR)
        {
            // Scale transform
            var S = SKMatrix.CreateScale(1 / size.Width, 1 / size.Height);

            // Affine transform
            var A = new SKMatrix(
                ptUR.X - ptUL.X,
                ptUR.Y - ptUL.Y, 
                0,
                ptLL.X - ptUL.X,
                ptLL.Y - ptUL.Y, 
                0, 
                ptUL.X,
                ptUL.Y,
                1);

            // Non-Affine transform
            A.TryInvert(out var inverseA);
            //var abPoint = MapPoint(inverseA, ptLR);
            var abPoint = inverseA.MapPoint(ptLR);
            var a = abPoint.X;
            var b = abPoint.Y;

            var scaleX = a / (a + b - 1);
            var scaleY = b / (a + b - 1);

            var N = new SKMatrix(
                scaleX,
                0,
                scaleX - 1,
                0,
                scaleY,
                scaleY - 1,
                0,
                0,
                1);

            // Multiply S * N * A
            var result = SKMatrix.CreateIdentity();
            result = result.PreConcat(S);
            result = result.PreConcat(N);
            result = result.PreConcat(A);
  
            return result;
        }

        private void OnDragDelta(object? sender, VectorEventArgs e)
        {
            if (sender is Thumb thumb)
            {
                Canvas.SetLeft(thumb, Canvas.GetLeft(thumb) + e.Vector.X);
                Canvas.SetTop(thumb, Canvas.GetTop(thumb) + e.Vector.Y);
                UpdateTransform();
            }
        }
    }
}