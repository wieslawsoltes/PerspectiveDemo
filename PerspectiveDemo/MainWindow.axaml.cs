using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;

namespace PerspectiveDemo
{
    public partial class MainWindow : Window
    {
        private Selected? _selected;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            var canvas = this.FindControl<Canvas>("Canvas");
            var rectangle = this.FindControl<Rectangle>("Rectangle");

            PointerPressed += (_, _) =>
            {
                if (_selected is null)
                {
                    AddSelected(rectangle, canvas);
                }
            };
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void AddSelected(Control control, Canvas canvas)
        {
            var layer = AdornerLayer.GetAdornerLayer(canvas);
            if (layer is null)
            {
                return;
            }

            _selected = new Selected
            {
                [AdornerLayer.AdornedElementProperty] = canvas,
                IsHitTestVisible = true,
                ClipToBounds = false,
                Control = control
            };

            ((ISetLogicalParent) _selected).SetParent(canvas);
            layer.Children.Add(_selected);
        }
     
        private void RemoveSelected(Canvas canvas)
        {
            var layer = AdornerLayer.GetAdornerLayer(canvas);
            if (layer is null || _selected is null)
            {
                return;
            }

            layer.Children.Remove(_selected);
            ((ISetLogicalParent) _selected).SetParent(null);
            _selected = null;
        }
    }
}
