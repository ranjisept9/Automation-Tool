using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System.Data;

namespace MobiMetrics
{
    partial class Xmlparser 
    {
        /// <summary>
        /// XMLPs this instance.
        /// </summary>
        static void Xmlp()
        {
            //var filePath = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.FullName;
            //var file = Path.Combine(filePath, "chargingUSSC.xml");
            var file = ConfigurationManager.AppSettings["outputPath"];
          file = file.Substring(5);
            var doc = XDocument.Load(file);
            XName name = (XNamespace) "http://schemas.microsoft.com/Azure/Monitoring/DataAccess/2009/May/" +
                         "TableColumn";
            var columns =
                doc.Descendants(name)
                    .Select(
                        x =>
                            new
                            {
                                Name = x.Descendants().FirstOrDefault().Value,
                                DataType = x.Descendants().LastOrDefault().Value
                            });
            //DataSet data = new DataSet();
            DataTable table = new DataTable();
            foreach (var column in columns)
            {
                table.Columns.Add(column.Name, Type.GetType(column.DataType));
            }

            name = (XNamespace) "http://schemas.microsoft.com/Azure/Monitoring/DataAccess/2009/May/" + "TableRow";
            var rows =
                doc.Descendants(name)
                    .Descendants((XNamespace) "http://schemas.microsoft.com/Azure/Monitoring/DataAccess/2009/May/" +
                                 "Fields");
            foreach (var row in rows)
            {
                var dataRow = table.NewRow();
                int index = 0;
                foreach (var field in row.Descendants())
                {
                    dataRow[index] = field.Value;
                    index++;
                }
                table.Rows.Add(dataRow);
            }

            var carriers =
                table.AsEnumerable()
                    .Select(
                        r =>
                            new
                            {
                                CarrierName = r["CounterInstance"].ToString(),
                                CounterName = r["CounterName"].ToString()
                            });
            var distinctCarriers = carriers.Select(x => x.CarrierName).Distinct();
            var outFile = "Out.csv";
            if (File.Exists(outFile))
                File.Delete(outFile);
            var outSampleFile = "OutSample.txt";
            if (File.Exists(outSampleFile))
              File.Delete(outSampleFile);
            foreach (var item in carriers)
            {
                string content = Environment.NewLine + item.CarrierName.ToString() + "\t" +
                                 item.CounterName.ToString();
                File.AppendAllText(outSampleFile, content);
            }
           foreach (var dCarrierName in distinctCarriers)
            { 
                var total = carriers.Where(c => c.CarrierName == dCarrierName);
                var failure = total.Where(c => c.CounterName.ToString().Contains("Failure"));

                    double value = failure.Count()*100/total.Count();
                    string content = Environment.NewLine + dCarrierName.ToString() + "," + total.Count().ToString() +
                                     "," + failure.Count().ToString() + "," + value.ToString();
                    File.AppendAllText(outFile, content);
                
            }
        }
    }
}
