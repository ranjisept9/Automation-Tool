using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace MobiMetrics
{
    abstract partial class MdsProcess
    {
        public void Mdslogic()
        {
            try
            {

                //var st1 = DateTime.Today.ToString();
                //var et1 = st1.AddDays(-1);
                //var temp1 = DateTime.Today.ToString("yyyy-MM-dd HH:MM");//, CultureInfo.CurrentCulture);
                //var duration = ConfigurationManager.AppSettings["duration"];
                //var temp2 = DateTime.Today.AddHours(-int.Parse(duration)).ToString("yyyy-MM-dd HH:mm");//, CultureInfo.CurrentCulture);
                //string StartTime = "-from:" + '"'+ temp2 + '"';
                //string EndTime = "-to:" + '"' + temp1 + '"';
                string StartTime = "-from:\"2014-02-11 09:00\"";
                string EndTime = "-to:\"2014-02-11 10:00\"";

                int waitTimeInMinutes = 60;
                string endpoint = ConfigurationManager.AppSettings["Endpoint"];
                string tablename = ConfigurationManager.AppSettings["tablename"];
                string outputPath = ConfigurationManager.AppSettings["outputPath"];
                //var temp = outputPath.Substring(5);
                if (File.Exists(outputPath.Substring(5)))
                    File.Delete(outputPath.Substring(5));
                //   string clientcert = ConfigurationManager.AppSettings["clientcert"];
                string query = ConfigurationManager.AppSettings["ChargingQuery"];
                string arguments = string.Format("{0} {1} {2} {3} {4} {5}", endpoint, tablename, outputPath, StartTime, EndTime, query);
                var procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + arguments) { UseShellExecute = false };

                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                proc.WaitForExit(waitTimeInMinutes * 60 * 1000);

                if (!proc.HasExited)
                    throw new Exception(string.Format("Process could not be finished in {0} minutes.", waitTimeInMinutes));
                //string t = proc.StandardOutput.ReadToEnd();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


    }
}
