using System.Text.RegularExpressions;

namespace TLCGen.Helpers
{
    public static class NameSyntaxChecker
    {
        private static Regex ValidCNameRegex = new Regex(@"^[a-zA-Z_0-9]+$", RegexOptions.Compiled);
        private static Regex ValidFileNameRegex = new Regex(@"^[a-zA-Z_\-0-9]+$", RegexOptions.Compiled);

        public static bool IsValidCName(string name)
        {
            return ValidCNameRegex.IsMatch(name);
        }

        public static bool IsValidFileName(string name)
        {
            return ValidFileNameRegex.IsMatch(name);
        }
    }
}
