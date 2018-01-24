using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TLCGen.Helpers
{
    public static class NameSyntaxChecker
    {
        private static Regex ValidCNameRegex = new Regex(@"^[a-zA-Z_0-9]+$", RegexOptions.Compiled);

        public static bool IsValidName(string name)
        {
            return ValidCNameRegex.IsMatch(name);
        }
    }
}
