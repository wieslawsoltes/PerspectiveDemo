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
            var t = this.FindControl<Thumb>("T");
            var b = this.FindControl<Thumb>("B");
            var l = this.FindControl<Thumb>("L");
            var r = this.FindControl<Thumb>("R");
            
            var ul = this.FindControl<Thumb>("UL");
            var ur = this.FindControl<Thumb>("UR");
            var ll = this.FindControl<Thumb>("LL");
            var lr = this.FindControl<Thumb>("LR");

            var drag = this.FindControl<Thumb>("DRAG");

            Canvas.SetLeft(t, rect.Left + rect.Width / 2);
            Canvas.SetTop(t, rect.Top);

            Canvas.SetLeft(b, rect.Left + rect.Width / 2);
            Canvas.SetTop(b, rect.Top + rect.Height);

            Canvas.SetLeft(l, rect.Left);
            Canvas.SetTop(l, rect.Top + rect.Height / 2);
            
            Canvas.SetLeft(r, rect.Left + rect.Width);
            Canvas.SetTop(r, rect.Top + rect.Height / 2);

            Canvas.SetLeft(ul, rect.Left);
            Canvas.SetTop(ul, rect.Top);

            Canvas.SetLeft(ur, rect.Left + rect.Width);
            Canvas.SetTop(ur, rect.Top);

            Canvas.SetLeft(ll, rect.Left);
            Canvas.SetTop(ll, rect.Top + rect.Height);

            Canvas.SetLeft(lr, rect.Left + rect.Width);
            Canvas.SetTop(lr, rect.Top + rect.Height);
   
            Canvas.SetLeft(drag, rect.Left);
            Canvas.SetTop(drag, rect.Top);
            drag.Width = rect.Width;
            drag.Height = rect.Height;
        }

        private void OnDragDeltaT(object? sender, VectorEventArgs e)
        {
            var rect = GetRect();
            var inflated = rect.Inflate(new Thickness(0, -e.Vector.Y, 0, 0));
            UpdateThumbs(inflated);
            UpdateRectangle(inflated);
        }

        private void OnDragDeltaB(object? sender, VectorEventArgs e)
        {
            var rect = GetRect();
            var inflated = rect.Inflate(new Thickness(0, 0, 0, e.Vector.Y));
            UpdateThumbs(inflated);
            UpdateRectangle(inflated);
        }

        private void OnDragDeltaL(object? sender, VectorEventArgs e)
        {
            var rect = GetRect();
            var inflated = rect.Inflate(new Thickness(-e.Vector.X, 0, 0, 0));
            UpdateThumbs(inflated);
            UpdateRectangle(inflated);
        }

        private void OnDragDeltaR(object? sender, VectorEventArgs e)
        {
            var rect = GetRect();
            var inflated = rect.Inflate(new Thickness(0, 0, e.Vector.X, 0));
            UpdateThumbs(inflated);
            UpdateRectangle(inflated);
        }
 
        private void OnDragDeltaUL(object? sender, VectorEventArgs e)
        {
            var rect = GetRect();
            var inflated = rect.Inflate(new Thickness(-e.Vector.X, -e.Vector.Y, 0, 0));
            UpdateThumbs(inflated);
            UpdateRectangle(inflated);
        }

        private void OnDragDeltaUR(object? sender, VectorEventArgs e)
        {
            var rect = GetRect();
            var inflated = rect.Inflate(new Thickness(0, -e.Vector.Y, e.Vector.X, 0));
            UpdateThumbs(inflated);
            UpdateRectangle(inflated);
        }

        private void OnDragDeltaLL(object? sender, VectorEventArgs e)
        {
            var rect = GetRect();
            var inflated = rect.Inflate(new Thickness(-e.Vector.X, 0, 0, e.Vector.Y));
            UpdateThumbs(inflated);
            UpdateRectangle(inflated);
        }

        private void OnDragDeltaLR(object? sender, VectorEventArgs e)
        {
            var rect = GetRect();
            var inflated = rect.Inflate(new Thickness(0, 0, e.Vector.X, e.Vector.Y));
            UpdateThumbs(inflated);
            UpdateRectangle(inflated);
        }

        private void OnDragDeltaDRAG(object? sender, VectorEventArgs e)
        {
            var drag = this.FindControl<Thumb>("DRAG");
            var left = Canvas.GetLeft(drag) + e.Vector.X;
            var top = Canvas.GetTop(drag) + e.Vector.Y;
            Canvas.SetLeft(drag, left);
            Canvas.SetTop(drag, top);
            var rect = new Rect(left, top, drag.Width, drag.Height);
            UpdateThumbs(rect);
            UpdateRectangle(rect);
        }
    }
}