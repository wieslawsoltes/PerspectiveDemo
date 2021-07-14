using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

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
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private Rect GetRect()
        {
            var ul = this.FindControl<Thumb>("UL");
            var ur = this.FindControl<Thumb>("UR");
            var ll = this.FindControl<Thumb>("LL");
            var lr = this.FindControl<Thumb>("LR");

            var ptUL = new Point(Canvas.GetLeft(ul), Canvas.GetTop(ul));
            var ptUR = new Point(Canvas.GetLeft(ur), Canvas.GetTop(ur));
            var ptLL = new Point(Canvas.GetLeft(ll), Canvas.GetTop(ll));
            var ptLR = new Point(Canvas.GetLeft(lr), Canvas.GetTop(lr));

            var left = Math.Min(Math.Min(ptUL.X, ptUR.X), Math.Min(ptLL.X, ptLR.X));
            var top = Math.Min(Math.Min(ptUL.Y, ptUR.Y), Math.Min(ptLL.Y, ptLR.Y));
            var right = Math.Max(Math.Max(ptUL.X, ptUR.X), Math.Max(ptLL.X, ptLR.X));
            var bottom = Math.Max(Math.Max(ptUL.Y, ptUR.Y), Math.Max(ptLL.Y, ptLR.Y));
            var width = Math.Abs(right - left);
            var height = Math.Abs(bottom - top);

            return new Rect(left, top, width, height);
        }

        private void UpdateRectangle(Rect rect)
        {
            var rectangle = this.FindControl<Rectangle>("Rectangle");

            Canvas.SetLeft(rectangle, rect.Left);
            Canvas.SetTop(rectangle, rect.Top);

            rectangle.Width = rect.Width;
            rectangle.Height = rect.Height;
        }

        private void UpdateThumbs(Rect rect)
        {
            var ul = this.FindControl<Thumb>("UL");
            var ur = this.FindControl<Thumb>("UR");
            var ll = this.FindControl<Thumb>("LL");
            var lr = this.FindControl<Thumb>("LR");

            Canvas.SetLeft(ul, rect.Left);
            Canvas.SetTop(ul, rect.Top);

            Canvas.SetLeft(ur, rect.Left + rect.Width);
            Canvas.SetTop(ur, rect.Top);

            Canvas.SetLeft(ll, rect.Left);
            Canvas.SetTop(ll, rect.Top + rect.Height);

            Canvas.SetLeft(lr, rect.Left + rect.Width);
            Canvas.SetTop(lr, rect.Top + rect.Height);
        }

        private void OnDragDeltaUL(object? sender, VectorEventArgs e)
        {
            if (sender is Thumb thumb)
            {
                var rect = GetRect();
                var inflated = rect.Inflate(new Thickness(-e.Vector.X, -e.Vector.Y, 0, 0));
                UpdateThumbs(inflated);
                UpdateRectangle(inflated);
            }
        }

        private void OnDragDeltaUR(object? sender, VectorEventArgs e)
        {
            if (sender is Thumb thumb)
            {
                var rect = GetRect();
                var inflated = rect.Inflate(new Thickness(0, -e.Vector.Y, e.Vector.X, 0));
                UpdateThumbs(inflated);
                UpdateRectangle(inflated);
            }
        }

        private void OnDragDeltaLL(object? sender, VectorEventArgs e)
        {
            if (sender is Thumb thumb)
            {
                var rect = GetRect();
                var inflated = rect.Inflate(new Thickness(-e.Vector.X, 0, 0, e.Vector.Y));
                UpdateThumbs(inflated);
                UpdateRectangle(inflated);
            }
        }

        private void OnDragDeltaLR(object? sender, VectorEventArgs e)
        {
            if (sender is Thumb thumb)
            {
                var rect = GetRect();
                var inflated = rect.Inflate(new Thickness(0, 0, e.Vector.X, e.Vector.Y));
                UpdateThumbs(inflated);
                UpdateRectangle(inflated);
            }
        }
    }
}