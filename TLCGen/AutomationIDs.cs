using System.Runtime.CompilerServices;

namespace TLCGen
{
    public static class AutomationIds
    {
        public static readonly string MainWindow = Create();
        public static readonly string FileMenu = Create();
        public static readonly string NewFileMenuItem = Create();
        public static readonly string OpenFileMenuItem = Create();
        public static readonly string SaveFileMenuItem = Create();
        public static readonly string SaveAsFileMenuItem = Create();
        public static readonly string CloseFileMenuItem = Create();
        public static readonly string ControllerView = Create();

        private static string Create([CallerMemberName] string name = null)
        {
            return name;
        }
    }
}
