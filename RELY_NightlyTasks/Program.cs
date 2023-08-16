using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace RELY_ScheduledTasks
{
    public class Program
    {
        static void Main(string[] args)
        {
            //System.Diagnostics.Debugger.Break();
            string LogFileName = ConfigurationManager.AppSettings["S15ExtractLogFileName"] + "_" + DateTime.UtcNow.ToString("dd-MM-yyyy-hhmmss") + ".txt";
            if ( args.Length == 0)
            {
                Console.WriteLine("Please provide the required arguments.");
                WriteToFile("Please provide the required arguments.", LogFileName);
                return ;
            }
            IAPIRestClient ARC = new APIRestClient();
            try
            {
                string CompanyCode = args[0]; //reading CompanyCode from Command prompt
                if (String.Equals("S15Extracts",args[1],StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("S15 Extracts Generation Job execution started on {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                    WriteToFile("S15 Extracts Generation Job execution started on {0}", LogFileName);
                    string ExtractNameList = "";
                    string ExtractFileNameList = "";
                    GKeyValueViewModel FileFormat = ARC.GetKeyValue("S15ExtractFileFormat", CompanyCode);
                    WriteToFile("FileFormat:- " + FileFormat.Value, LogFileName);
                    //Getting list of extracts detail containing Extracts Name,their corresponding FileName and FileType
                    List<S15ExtractsViewModel> GridData = ARC.GetS15GridData(CompanyCode);
                    foreach (var data in GridData)
                    {
                        ExtractFileNameList = ExtractFileNameList + "," + data.FileName;
                        string ExtractName = data.Extracts + ':' + data.ExtractFileType;
                        ExtractNameList = ExtractNameList + "," + ExtractName;
                    }
                    ExtractFileNameList = ExtractFileNameList.TrimStart(',');
                    ExtractNameList = ExtractNameList.TrimStart(',');

                    WriteToFile("Extracts Names:- " + ExtractNameList + " on {0}.", LogFileName);
                    WriteToFile("Extract FileNames:- " + ExtractFileNameList + " on {0}.", LogFileName);
                    WriteToFile("Going to call API function for Extracts generation on {0}.", LogFileName);
                    Console.WriteLine("Going to call API function for Extracts generation on {0}.", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                    //Call to API method for Generating Extracts
                    //ARC.GetS15Extracts(DateTime.UtcNow, DateTime.UtcNow, ExtractNameList, CompanyCode, "Generate", ExtractFileNameList, FileFormat.Value);
                    ARC.GetS15Extracts(GridData, DateTime.UtcNow, DateTime.UtcNow, CompanyCode, "Generate", (FileFormat.Value).ToString());
                    WriteToFile("S15 Extracts Generation Job executed successfully on {0}", LogFileName);
                    Console.WriteLine("S15 Extracts Generation Job executed successfully on {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
                }
                
            }
            catch (Exception ex)
            {
                //log exception in File and DataBase
                WriteToFile("{0} Exception occured, starting logging into DataBase:- " + ex.Message.ToString() , LogFileName);
                var model = new GErrorLogViewModel
                {
                    UserName = "ConsoleAPP",Controller = "ConsoleAPP",Method = "Main",ErrorDateTime = DateTime.UtcNow, StackTrace = ex.Message.ToString(),
                    SourceProject = "ConsoleAPP", Status = "New", ErrorType = "System",ErrorDescription=ex.StackTrace
                };
                ARC.Add(model, null);
                WriteToFile("{0} Exception logged to DataBase.", LogFileName);
            }
        }

        //Method to Add entry in Log File
        private static void WriteToFile(string text,string LogFileName)
        {
            string Path = ConfigurationManager.AppSettings["LogFilePath"];
            
            //If path does not exist, create the directory
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }
            using (StreamWriter writer = new StreamWriter(Path + LogFileName, true))
            {
                writer.WriteLine(string.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
                writer.Close();
            }
        }

    }
}
