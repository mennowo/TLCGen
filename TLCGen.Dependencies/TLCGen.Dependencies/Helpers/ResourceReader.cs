using System.IO;

namespace TLCGen.Helpers
{
    public static class ResourceReader
    {
        public static string GetResourceTextFile(string filename, object callingObject)
        {
            var result = string.Empty;

            var t = callingObject.GetType();
            using (var stream = t.Assembly.
                       GetManifestResourceStream(filename))
            {
                using (var sr = new StreamReader(stream))
                {
                    result = sr.ReadToEnd();
                }
            }
            return result;
        }
    }
}
