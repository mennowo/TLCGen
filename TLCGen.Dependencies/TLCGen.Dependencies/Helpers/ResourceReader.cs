using System.IO;

namespace TLCGen.Helpers
{
    public static class ResourceReader
    {
        public static string GetResourceTextFile(string filename, object callingObject)
        {
            string result = string.Empty;

            var t = callingObject.GetType();
            using (Stream stream = t.Assembly.
                       GetManifestResourceStream(filename))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    result = sr.ReadToEnd();
                }
            }
            return result;
        }
    }
}
