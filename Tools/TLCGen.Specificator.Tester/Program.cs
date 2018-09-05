using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Specificator.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            var sp = new SpecificatorPlugin(true)
            {
                Controller = Helpers.TLCGenSerialization.DeSerialize<ControllerModel>(@"C:\Users\menno\Documents\temp\SP16\SP16.tlc")
            };
            
            DataAccess.TLCGenControllerDataProvider.Default.ControllerFileName =
                sp.ControllerFileName = @"C:\Users\menno\Documents\temp\SP16\SP16.tlc";

            sp.SpecificatorVM.Data = new SpecificatorDataModel()
            {
                EMail = "testEmail",
                Organisatie = "testOrg",
                Postcode = "12345",
                Stad = "New York",
                Straat = "Broadway",
                TelefoonNummer = "0123456789",
                Website = "www.www.www"
            };
            sp.SpecificatorVM.GenerateSpecification();
        }
    }
}
