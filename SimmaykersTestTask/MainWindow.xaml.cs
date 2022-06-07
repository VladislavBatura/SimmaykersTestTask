using InteractiveDataDisplay.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

namespace SimmaykersTestTask
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<GraphData> graphData = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        public Task LoadGraphs(IEnumerable<GraphData> graphDatas)
        {
            var x = graphDatas.Select(item => item.x).ToArray();
            var y = graphDatas.Select(item => item.y).ToArray();

            linegraph.Plot(x, y); // x and y are IEnumerable<double>
            return Task.CompletedTask;
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            graphData.Add(new());
            tableInput.Items.Refresh();
            await LoadGraphs(graphData);
        }

        private void tableInput_Initialized(object sender, EventArgs e)
        {
            tableInput.ItemsSource = graphData;
        }

        private async void tableInput_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {

            //var obj = graphData.LastOrDefault();
            //obj.y = Function(obj.x);
            //await LoadGraphs(graphData);
        }

        private double Function(double x)
        {
            var k = (double)new Random().Next(10);
            var b = (double)new Random().Next(10);
            return k * x + b;
        }

        private async void tableInput_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (e.Column is DataGridBoundColumn column)
                {
                    var bindingPath = (column.Binding as Binding).Path.Path;
                    if (bindingPath.Equals("x"))
                    {
                        var rowIndex = e.Row.GetIndex();
                        var el = e.EditingElement as TextBox;
                        var x = double.Parse(el.Text);
                        var y = Function(x);
                        var dataDot = graphData[rowIndex];
                        dataDot.y = y;
                        dataDot.x = x;

                        LoadGraphs(graphData);
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = tableInput.SelectedItem;
            if (selectedItem is not null)
            {
                if (selectedItem is GraphData dat)
                {
                    graphData.Remove(dat);
                    tableInput.Items.Refresh();
                }
            }
        }
    }
}
