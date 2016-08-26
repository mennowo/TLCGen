using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL
{
    public partial class CCOLCodeGenerator
    {
        protected void GenerateVisualStudioProjectFiles(ControllerDataModel data)
        {
            string templatefilename = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "ccolvsprojtemplate.xml");
            if(File.Exists(templatefilename))
            {
                string[] projtemplate = File.ReadAllLines(templatefilename);
                StringBuilder sb = new StringBuilder();
                foreach(string line in projtemplate)
                {
                    string writeline = line;

                    // If conditions
                    if(line.StartsWith("__IF"))
                    {
                        string condition = line.Replace("__IF", "");
                        switch(condition)
                        {
                            case "OV":
                                break;
                            case "MV":
                                break;
                            case "PTP":
                                break;
                            case "SYNC":
                                break;
                            case "MS":
                                break;
                            case "NOTMS":
                                break;
                            case "KS":
                                break;
                        }
                    }

                    // After the ifs, all __ indicate a needed replacement
                    if (writeline.Contains("__"))
                    {
                        writeline.Replace("__CONTROLLERNAME", data.Naam);
                        writeline.Replace("__GUID", data.Naam);
#warning TODO!
                    }
                }
            }
        }
    }
}
