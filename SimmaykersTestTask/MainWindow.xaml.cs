using InteractiveDataDisplay.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        }

        public Task LoadGraphs(IEnumerable<GraphData> graphDatas)
        {
            var x = graphDatas.Select(item => item.x).ToArray();
            var y = graphDatas.Select(item => item.y).ToArray();

            linegraph.Plot(x, y);
            return Task.CompletedTask;
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

                        await LoadGraphs(graphData);
                    }
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = tableInput.SelectedItem;
            if (selectedItem is not null)
            {
                if (selectedItem is GraphData dat)
                {
                    graphData.Remove(dat);
                    LoadGraphs(graphData);
                    tableInput.Items.Refresh();
                }
            }
        }

        private void CopyTableButton_Click(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            foreach(var dot in graphData)
            {
                sb.Append(dot.x.ToString() + "\t");
                sb.Append(dot.y.ToString());

                sb.AppendLine();
            }

            var clip = new DataObject();
            clip.SetData(DataFormats.Text, sb.ToString());
            Clipboard.Clear();
            Clipboard.SetDataObject(clip);
        }

        private void PasteTableButton_Click(object sender, RoutedEventArgs e)
        {
            var dataObject = Clipboard.GetDataObject();
            var dataObjectText = dataObject.GetData(DataFormats.Text);
            var textFromDataObject = dataObjectText.ToString();
            var textAfterRegex = Regex.Replace(textFromDataObject, @"\t|\n|\r", " ");
            var splitedText = textAfterRegex.Split(' ');
            graphData.Clear();
            for(var i = 0; i < splitedText.Count() - 1; i++)
            {
                if (splitedText[i].Equals(""))
                    continue;

                graphData.Add(new()
                {
                    x = double.Parse(splitedText[i]),
                    y = double.Parse(splitedText[i + 1])
                });
                i++;
            }

            tableInput.Items.Refresh();
        }

        private void SaveTableButton_Click(object sender, RoutedEventArgs e)
        {
            var a = JsonSerializer.Serialize<IEnumerable<GraphData>>(graphData);
            File.WriteAllText("table.json", a);
        }

        private void LoadTableButton_Click(object sender, RoutedEventArgs e)
        {
            var a = File.ReadAllText("table.json");
            var b = JsonSerializer.Deserialize<IEnumerable<GraphData>>(a);
            if (b is IEnumerable<GraphData> data)
            {
                graphData.AddRange(data);
                tableInput.Items.Refresh();
                LoadGraphs(graphData);
            }
        }
    }
}
