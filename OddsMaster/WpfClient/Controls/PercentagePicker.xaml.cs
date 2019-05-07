using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OddsMaster
{
    /// <summary>
    /// Interaction logic for PercentagePicker.xaml
    /// </summary>
    public partial class PercentagePicker : UserControl
    {
        public PercentagePicker()
        {
            InitializeComponent();
        }

        private void HandleMouseDown(object sender, MouseButtonEventArgs e)
        {
            var mousePosition = Mouse.GetPosition(MainGrid);
            var selectedRatio = mousePosition.X / MainGrid.ActualWidth;
        }
    }
}
