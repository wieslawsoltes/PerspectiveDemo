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

namespace PerspectiveDemo
{
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
            var canvas = this.FindControl<Canvas>("Canvas");
            var rectangle = this.FindControl<Rectangle>("Rectangle");

            var width = rectangle.Width;
            var height = rectangle.Height;

            var ul = this.FindControl<Thumb>("UL");
            var ur = this.FindControl<Thumb>("UR");
            var ll = this.FindControl<Thumb>("LL");
            var lr = this.FindControl<Thumb>("LR");

            var ptUL = new Point(Canvas.GetLeft(ul), Canvas.GetTop(ul));
            var ptUR = new Point(Canvas.GetLeft(ur), Canvas.GetTop(ur));
            var ptLL = new Point(Canvas.GetLeft(ll), Canvas.GetTop(ll));
            var ptLR = new Point(Canvas.GetLeft(lr), Canvas.GetTop(lr));

            var result = ComputeMatrix(new Size(width, height), ptUL, ptUR, ptLL, ptLR);

            rectangle.RenderTransformOrigin = RelativePoint.Center;
            rectangle.RenderTransform = new MatrixTransform(result);
        }

        static Point MapPoint(Matrix matrix, Point point)
        {
            return new Point(
                (point.X * matrix.M11) + (point.Y * matrix.M21) + matrix.M31,
                (point.X * matrix.M12) + (point.Y * matrix.M22) + matrix.M32);
        }

        static Matrix ComputeMatrix(Size size, Point ptUL, Point ptUR, Point ptLL, Point ptLR)
        {
            // Scale transform
            var S = Matrix.CreateScale(1 / size.Width, 1 / size.Height);

            // Affine transform
            var A = new Matrix(
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
            //A.TryInvert(out var inverseA);
            //var abPoint = MapPoint(inverseA, ptLR);
            //var a = abPoint.X;
            //var b = abPoint.Y;
            double den = A.M11 * A.M22 - A.M12 * A.M21;
            double a = (A.M22 * ptLR.X - A.M21 * ptLR.Y + A.M21 * A.M32 - A.M22 * A.M31) / den;
            double b = (A.M11 * ptLR.Y - A.M12 * ptLR.X + A.M12 * A.M31 - A.M11 * A.M32) / den;

            var scaleX = a / (a + b - 1);
            var scaleY = b / (a + b - 1);

            var N = new Matrix(
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
            //var result = S * N * A;
            
            // TODO: Multiply does not include perspective
            var result = Multiply(Multiply(S, N), A);

            return result;
        }

        public static Matrix Multiply(Matrix a, Matrix b)
        {
            return new Matrix(
                (a.M11 * b.M11) + (a.M12 * b.M21) + (a.M13 * b.M31),
                (a.M11 * b.M12) + (a.M12 * b.M22) + (a.M13 * b.M32),
                (a.M11 * b.M13) + (a.M12 * b.M23) + (a.M13 * b.M33),
                
                (a.M21 * b.M11) + (a.M22 * b.M21) + (a.M23 * b.M31),
                (a.M21 * b.M12) + (a.M22 * b.M22) + (a.M23 * b.M32),
                (a.M21 * b.M13) + (a.M22 * b.M23) + (a.M23 * b.M33),

                (a.M31 * b.M11) + (a.M32 * b.M21) + (a.M33 * b.M31),
                (a.M31 * b.M12) + (a.M32 * b.M22) + (a.M33 * b.M32),
                (a.M31 * b.M13) + (a.M32 * b.M23) + (a.M33 * b.M33));
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