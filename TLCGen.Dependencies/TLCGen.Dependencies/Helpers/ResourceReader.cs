using System;
using System.IO;

namespace TLCGen.Helpers
{
    public static class ResourceReader
    {
        public static string GetResourceTextFile(string filename, object callingObject, Type type = null)
        {
            var result = string.Empty;

            if (type == null) type = callingObject.GetType();
            using (var stream = type.Assembly.
                       GetManifestResourceStream(filename))
            {
                using (var sr = new StreamReader(stream ?? throw new InvalidOperationException()))
                {
                    result = sr.ReadToEnd();
                }
            }
            return result;
        }
    }
}
