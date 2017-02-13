using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TLCGen.Models;
using TLCGen.Settings;

namespace TLCGen.Importers.TabC
{
    public class TabCImportHelperOutcome
    {
        public List<FaseCyclusModel> Fasen { get; set; }
        public List<ConflictModel> Conflicten { get; set; }

        public TabCImportHelperOutcome()
        {
            Fasen = new List<FaseCyclusModel>();
            Conflicten = new List<ConflictModel>();
        }
    }

    public static class TabCImportHelper
    {
        public static TabCImportHelperOutcome GetNewData(string[] lines)
        {
            TabCImportHelperOutcome outcome = new TabCImportHelperOutcome();

            // Compile a list of Phases with conflicts from the file
            foreach (string line in lines)
            {
                if (Regex.IsMatch(line, @"^\s+TO_max\["))
                {
                    string fc1 = Regex.Replace(line, @"^\s*TO_max\s*\[\s*(fc[0-9]+).*", "$1");
                    string fc2 = Regex.Replace(line, @"^\s*TO_max\s*\[\s*fc[0-9]+\s*\]\s*\[\s*(fc[0-9]+).*", "$1");
                    string _conf = Regex.Replace(line, @"^\s*TO_max\s*\[\s*fc[0-9]+\s*\]\s*\[\s*fc[0-9]+\s*\]\s*=\s*(([0-9]+|FK|GK|GKL)).*", "$1");

                    int conf = 0;
                    if (_conf == "FK") conf = -2;
                    else if (_conf == "GK") conf = -3;
                    else if (_conf == "GKL") conf = -4;
                    else if (!Int32.TryParse(_conf, out conf))
                    {
                        if (lines.Count() <= 1)
                            throw new NotImplementedException($"Conflict van {fc1} naar {fc2} heeft een foutieve waarde: {_conf}");
                    }

                    FaseCyclusModel _fcm1 = null;
                    foreach (FaseCyclusModel fcm in outcome.Fasen)
                    {
                        if (fcm.Naam == fc1.Replace("fc", ""))
                        {
                            _fcm1 = fcm;
                            break;
                        }
                    }
                    if (_fcm1 == null)
                    {
                        _fcm1 = new FaseCyclusModel();
                        _fcm1.Naam = fc1.Replace("fc", "");
                        DefaultsProvider.Default.SetDefaultsOnModel(_fcm1);
                        outcome.Fasen.Add(_fcm1);
                    }

                    FaseCyclusModel _fcm2 = null;
                    foreach (FaseCyclusModel fcm in outcome.Fasen)
                    {
                        if (fcm.Naam == fc2.Replace("fc", ""))
                        {
                            _fcm2 = fcm;
                            break;
                        }
                    }
                    if (_fcm2 == null)
                    {
                        _fcm2 = new FaseCyclusModel();
                        _fcm2.Naam = fc2.Replace("fc", "");
                        DefaultsProvider.Default.SetDefaultsOnModel(_fcm2);
                        outcome.Fasen.Add(_fcm2);
                    }
                    outcome.Conflicten.Add(new ConflictModel() { FaseVan = _fcm1.Naam, FaseNaar = _fcm2.Naam, Waarde = conf });
                }
            }
            return outcome;
        }
    }
}
