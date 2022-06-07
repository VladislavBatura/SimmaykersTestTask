using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SimmaykersTestTask.ViewModels;

namespace SimmaykersTestTask
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<GraphData> graphData = new();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new ApplicationViewModel();
        }

        public Task LoadGraphs(IEnumerable<GraphData> graphDatas)
        {
            var x = graphDatas.Select(item => item.X).ToArray();
            var y = graphDatas.Select(item => item.Y).ToArray();

            linegraph.Plot(x, y);
            return Task.CompletedTask;
        }

        private void TakeVmAndLoadGraphs()
        {
            var dt = DataContext as ApplicationViewModel;
            LoadGraphs(dt.GraphDatas);
        }

        private void tableInput_Initialized(object sender, EventArgs e)
        {
            tableInput.ItemsSource = graphData;
        }

        private double Function(double x)
        {
            var k = (double)new Random().Next(10);
            var b = (double)new Random().Next(10);
            return k * x + b;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            TakeVmAndLoadGraphs();
        }

        private void PasteTableButton_Click(object sender, RoutedEventArgs e)
        {
            TakeVmAndLoadGraphs();
        }

        private void LoadTableButton_Click(object sender, RoutedEventArgs e)
        {
            TakeVmAndLoadGraphs();
        }

        private void tableInput_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (tableInput.SelectedItem is null)
            {
                return;
            }

            var dg = sender as DataGrid;
            dg.RowEditEnding -= tableInput_RowEditEnding;
            dg.CommitEdit();
            dg.Items.Refresh();
            dg.RowEditEnding += tableInput_RowEditEnding;

            var data = tableInput.SelectedItem as GraphData;

            var y = Function(data.X);
            data.Y = y;
            TakeVmAndLoadGraphs();
        }
    }
}
