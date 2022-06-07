using InteractiveDataDisplay.WPF;
using SimmaykersTestTask.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SimmaykersTestTask.Extensions;
using System.Windows;

namespace SimmaykersTestTask.ViewModels
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        private GraphData selectedGraphData;
        private RelayCommand addCommand;
        private RelayCommand removeCommand;
        private RelayCommand copyToClipboardCommand;
        private RelayCommand pasteFromClipboardCommand;
        private RelayCommand saveDataToJsonFileCommand;
        private RelayCommand loadDataFromJsonFileCommand;
        public ObservableCollection<GraphData> GraphDatas { get; set; }

        public GraphData SelectedGraphData
        {
            get { return selectedGraphData; }
            set
            {
                selectedGraphData = value;
                OnPropertyChanged("SelectedGraphData");
            }
        }

        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ?? (addCommand = new(obj =>
                {
                    GraphDatas.Add(new());
                }));
            }
        }

        public RelayCommand RemoveCommand
        {
            get
            {
                return removeCommand ?? (removeCommand = new(obj =>
                {
                    var graphData = obj as GraphData;
                    if (graphData is not null)
                        GraphDatas.Remove(graphData);
                },
                (obj) => GraphDatas.Count > 0));
            }
        }

        public RelayCommand CopyToClipboardCommand
        {
            get
            {
                return copyToClipboardCommand ?? (copyToClipboardCommand = new(obj =>
                {
                    var str = StringExtension.CreateStringOutOfCollection(GraphDatas);
                    var clip = new DataObject();
                    clip.SetData(DataFormats.Text, str);
                    Clipboard.Clear();
                    Clipboard.SetDataObject(clip);
                }));
            }
        }

        public RelayCommand PasteFromClipboardCommand
        {
            get
            {
                return pasteFromClipboardCommand ?? (pasteFromClipboardCommand = new(obj =>
                {
                    var strArr = StringExtension.CreateStringArrFromClipboard();
                    GraphDatas.Clear();
                    for (var i = 0; i < strArr.Count() - 1; i++)
                    {
                        if (strArr[i].Equals(""))
                            continue;

                        GraphDatas.Add(new()
                        {
                            X = double.Parse(strArr[i]),
                            Y = double.Parse(strArr[i + 1])
                        });
                        i++;
                    }
                }));
            }
        }

        public RelayCommand SaveDataToJsonFileCommand
        {
            get
            {
                return saveDataToJsonFileCommand ?? (saveDataToJsonFileCommand = new(obj =>
                {
                    StringExtension.SaveDataToJsonFile(GraphDatas);
                }));
            }
        }

        public RelayCommand LoadDataFromJsonFileCommand
        {
            get
            {
                return loadDataFromJsonFileCommand ?? (loadDataFromJsonFileCommand = new(obj =>
                {
                    var collFromJson = StringExtension.LoadDataFromJsonFile();
                    foreach (var item in collFromJson)
                    {
                        GraphDatas.Add(item);
                    }
                }));
            }
        }

        public ApplicationViewModel()
        {
            GraphDatas = new();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged is not null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
