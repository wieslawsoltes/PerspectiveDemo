using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace PerspectiveDemo
{
    public class AdornerCanvas : TemplatedControl
    {
        private Thumb _t;
        private Thumb _b;
        private Thumb _l;
        private Thumb _r;
        private Thumb _ul;
        private Thumb _ur;
        private Thumb _ll;
        private Thumb _lr;
        private Thumb _drag;

        public static readonly StyledProperty<Canvas> CanvasProperty = 
            AvaloniaProperty.Register<AdornerCanvas, Canvas>(nameof(Canvas));

        public static readonly StyledProperty<Control> ControlProperty = 
            AvaloniaProperty.Register<AdornerCanvas, Control>(nameof(Control));

        public Canvas Canvas
        {
            get => GetValue(CanvasProperty);
            set => SetValue(CanvasProperty, value);
        }

        public Control Control
        {
            get => GetValue(ControlProperty);
            set => SetValue(ControlProperty, value);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _t = e.NameScope.Find<Thumb>("PART_T");
            _b = e.NameScope.Find<Thumb>("PART_B");
            _l = e.NameScope.Find<Thumb>("PART_L");
            _r = e.NameScope.Find<Thumb>("PART_R");
            _ul = e.NameScope.Find<Thumb>("PART_UL");
            _ur = e.NameScope.Find<Thumb>("PART_UR");
            _ll = e.NameScope.Find<Thumb>("PART_LL");
            _lr = e.NameScope.Find<Thumb>("PART_LR");
            _drag = e.NameScope.Find<Thumb>("PART_DRAG");

            _t.DragDelta += OnDragDeltaT;
            _b.DragDelta += OnDragDeltaB;
            _l.DragDelta += OnDragDeltaL;
            _r.DragDelta += OnDragDeltaR;
            _ul.DragDelta += OnDragDeltaUL;
            _ur.DragDelta += OnDragDeltaUR;
            _ll.DragDelta += OnDragDeltaLL;
            _lr.DragDelta += OnDragDeltaLR;
            _drag.DragDelta += OnDragDeltaDRAG;
        }

        private Rect GetRect()
        {
            var ptUL = new Point(Canvas.GetLeft(_ul), Canvas.GetTop(_ul));
            var ptUR = new Point(Canvas.GetLeft(_ur), Canvas.GetTop(_ur));
            var ptLL = new Point(Canvas.GetLeft(_ll), Canvas.GetTop(_ll));
            var ptLR = new Point(Canvas.GetLeft(_lr), Canvas.GetTop(_lr));

            var left = Math.Min(Math.Min(ptUL.X, ptUR.X), Math.Min(ptLL.X, ptLR.X));
            var top = Math.Min(Math.Min(ptUL.Y, ptUR.Y), Math.Min(ptLL.Y, ptLR.Y));
            var right = Math.Max(Math.Max(ptUL.X, ptUR.X), Math.Max(ptLL.X, ptLR.X));
            var bottom = Math.Max(Math.Max(ptUL.Y, ptUR.Y), Math.Max(ptLL.Y, ptLR.Y));
            var width = Math.Abs(right - left);
            var height = Math.Abs(bottom - top);

            return new Rect(left, top, width, height);
        }

        private void UpdateControl(Control control, Rect rect)
        {
            Canvas.SetLeft(control, rect.Left);
            Canvas.SetTop(control, rect.Top);

            control.Width = rect.Width;
            control.Height = rect.Height;
        }

        private void UpdateThumbs(Rect rect)
        {
            Canvas.SetLeft(_t, rect.Left + rect.Width / 2);
            Canvas.SetTop(_t, rect.Top);

            Canvas.SetLeft(_b, rect.Left + rect.Width / 2);
            Canvas.SetTop(_b, rect.Top + rect.Height);

            Canvas.SetLeft(_l, rect.Left);
            Canvas.SetTop(_l, rect.Top + rect.Height / 2);
            
            Canvas.SetLeft(_r, rect.Left + rect.Width);
            Canvas.SetTop(_r, rect.Top + rect.Height / 2);

            Canvas.SetLeft(_ul, rect.Left);
            Canvas.SetTop(_ul, rect.Top);

            Canvas.SetLeft(_ur, rect.Left + rect.Width);
            Canvas.SetTop(_ur, rect.Top);

            Canvas.SetLeft(_ll, rect.Left);
            Canvas.SetTop(_ll, rect.Top + rect.Height);

            Canvas.SetLeft(_lr, rect.Left + rect.Width);
            Canvas.SetTop(_lr, rect.Top + rect.Height);
   
            Canvas.SetLeft(_drag, rect.Left);
            Canvas.SetTop(_drag, rect.Top);
            _drag.Width = rect.Width;
            _drag.Height = rect.Height;
        }

        private void OnDragDeltaT(object? sender, VectorEventArgs e)
        {
            var rect = GetRect();
            var inflated = rect.Inflate(new Thickness(0, -e.Vector.Y, 0, 0));
            UpdateThumbs(inflated);
            UpdateControl(Control, inflated);
        }

        private void OnDragDeltaB(object? sender, VectorEventArgs e)
        {
            var rect = GetRect();
            var inflated = rect.Inflate(new Thickness(0, 0, 0, e.Vector.Y));
            UpdateThumbs(inflated);
            UpdateControl(Control, inflated);
        }

        private void OnDragDeltaL(object? sender, VectorEventArgs e)
        {
            var rect = GetRect();
            var inflated = rect.Inflate(new Thickness(-e.Vector.X, 0, 0, 0));
            UpdateThumbs(inflated);
            UpdateControl(Control, inflated);
        }

        private void OnDragDeltaR(object? sender, VectorEventArgs e)
        {
            var rect = GetRect();
            var inflated = rect.Inflate(new Thickness(0, 0, e.Vector.X, 0));
            UpdateThumbs(inflated);
            UpdateControl(Control, inflated);
        }
 
        private void OnDragDeltaUL(object? sender, VectorEventArgs e)
        {
            var rect = GetRect();
            var inflated = rect.Inflate(new Thickness(-e.Vector.X, -e.Vector.Y, 0, 0));
            UpdateThumbs(inflated);
            UpdateControl(Control, inflated);
        }

        private void OnDragDeltaUR(object? sender, VectorEventArgs e)
        {
            var rect = GetRect();
            var inflated = rect.Inflate(new Thickness(0, -e.Vector.Y, e.Vector.X, 0));
            UpdateThumbs(inflated);
            UpdateControl(Control, inflated);
        }

        private void OnDragDeltaLL(object? sender, VectorEventArgs e)
        {
            var rect = GetRect();
            var inflated = rect.Inflate(new Thickness(-e.Vector.X, 0, 0, e.Vector.Y));
            UpdateThumbs(inflated);
            UpdateControl(Control, inflated);
        }

        private void OnDragDeltaLR(object? sender, VectorEventArgs e)
        {
            var rect = GetRect();
            var inflated = rect.Inflate(new Thickness(0, 0, e.Vector.X, e.Vector.Y));
            UpdateThumbs(inflated);
            UpdateControl(Control, inflated);
        }

        private void OnDragDeltaDRAG(object? sender, VectorEventArgs e)
        {
            var left = Canvas.GetLeft(_drag) + e.Vector.X;
            var top = Canvas.GetTop(_drag) + e.Vector.Y;
            Canvas.SetLeft(_drag, left);
            Canvas.SetTop(_drag, top);
            var rect = new Rect(left, top, _drag.Width, _drag.Height);
            UpdateThumbs(rect);
            UpdateControl(Control, rect);
        }
    }
}