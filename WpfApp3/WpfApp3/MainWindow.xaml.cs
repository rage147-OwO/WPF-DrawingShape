using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace WpfApp3
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {

        VIewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new VIewModel(ellipse, canvas);
            DataContext = viewModel;
        }

        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (sender is Canvas)
            {
                viewModel.MouseMoveHandler(sender, e);

            }

        }

        private void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine(e.Source);
            //if e.Source is Canvas
            if (e.OriginalSource is Ellipse)
            {
                viewModel.MouseDownHandler(sender, e);
            }
        }


    }
}
