using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace SimmaykersTestTask.Extensions
{
    public static class StringExtension
    {
        public static string CreateStringOutOfCollection(IEnumerable<GraphData> coll)
        {
            var sb = new StringBuilder();
            foreach (var dot in coll)
            {
                sb.Append(dot.X.ToString() + "\t");
                sb.Append(dot.Y.ToString());

                sb.AppendLine();
            }

            return sb.ToString();
        }
        public static string[] CreateStringArrFromClipboard()
        {
            var dataObject = Clipboard.GetDataObject();
            var dataObjectText = dataObject.GetData(DataFormats.Text);
            var textFromDataObject = dataObjectText.ToString();
            var textAfterRegex = Regex.Replace(textFromDataObject, @"\t|\n|\r", " ");
            return textAfterRegex.Split(' ');
        }

        public static void SaveDataToJsonFile(IEnumerable<GraphData> data)
        {
            var jsonString = JsonSerializer.Serialize(data);
            File.WriteAllText("table.json", jsonString);
        }

        public static IEnumerable<GraphData>? LoadDataFromJsonFile()
        {
            var textFromFile = File.ReadAllText("table.json");
            var jsonObject = JsonSerializer.Deserialize<IEnumerable<GraphData>>(textFromFile);
            if (jsonObject is IEnumerable<GraphData> data)
            {
                return data;
            }
            return null;
        }
    }
}
