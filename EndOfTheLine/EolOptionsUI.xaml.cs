using System.Windows.Controls;

namespace EndOfTheLine
{
    /// <summary>
    /// Interaction logic for EolOptionsUI.xaml
    /// </summary>
    public partial class EolOptionsUI : UserControl
    {
        public EolOptionsUI(object model)
        {
            DataContext = model;
            InitializeComponent();
        }
    }
}
