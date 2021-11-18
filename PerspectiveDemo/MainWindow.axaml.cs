using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;

namespace PerspectiveDemo
{
    public partial class MainWindow : Window
    {
        private Selected? _selection;

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
                if (_selection is null)
                {
                    AddSelection(rectangle, canvas);
                }
            };
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void AddSelection(Control control, Canvas canvas)
        {
            var layer = AdornerLayer.GetAdornerLayer(control);
            if (layer is null)
            {
                return;
            }

            _selection = new Selected
            {
                [AdornerLayer.AdornedElementProperty] = control,
                IsHitTestVisible = true,
                ClipToBounds = false,
                Control = control,
                Canvas = canvas
            };

            ((ISetLogicalParent) _selection).SetParent(control);
            layer.Children.Add(_selection);
        }
     
        private void RemoveSelection(Control control)
        {
            var layer = AdornerLayer.GetAdornerLayer(control);
            if (layer is null || _selection is null)
            {
                return;
            }

            layer.Children.Remove(_selection);
            ((ISetLogicalParent) _selection).SetParent(null);
            _selection = null;
        }
    }
}
