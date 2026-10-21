using System.Diagnostics;

namespace TLCGen.Models
{
    /// <summary>
    /// Debug trace of changes to priority ingrepen and their meldingen (which UI action changed which melding or
    /// dummy detector). Written to the debug output with prefix "[prio]"; calls are compiled out of Release builds.
    /// </summary>
    public static class PrioTrace
    {
        [Conditional("DEBUG")]
        public static void Write(string message)
        {
            Debug.WriteLine($"[prio] {message}");
        }
    }
}
