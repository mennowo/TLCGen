using System;
using System.IO;
using System.Linq;

namespace TLCGen.FolderVersionRenamer
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Any(x => x == "--help"))
            {
                DisplayHelp();
                return;
            }
            var largs = args.ToList();
            var iDir = largs.IndexOf("--tlcgen-folder");
            var iMDir = largs.IndexOf("--target-folder");
            if(iDir < 0 || iDir >= largs.Count - 1 || iMDir < 0 || iMDir >= largs.Count - 1)
            {
                Console.WriteLine("Incorrect arguments given. Displaying help.");
                Console.WriteLine();
                DisplayHelp();
                return;
            }
            var dir = largs[iDir + 1];
            var mdir = largs[iMDir + 1];
            var tlcgen = Path.Combine(dir, "TLCGen.exe");
            if (!Directory.Exists(dir))
            {
                Console.WriteLine("The given argument is not a path to an existing folder.");
            }
            if (!File.Exists(tlcgen))
            {
                Console.WriteLine("The given folder does not contain TLCGen.exe.");
            }
            //var ver = System.Diagnostics.FileVersionInfo.GetVersionInfo(tlcgen);
            var ass = System.Reflection.Assembly.LoadFrom(tlcgen);
            var ver = ass.GetName().Version;
            var dat = GetLinkerTime(ass);
            Console.WriteLine("TLCGen.exe found:");
            Console.WriteLine($"- version: {ver}");
            Console.WriteLine($"- date: {dat.ToLongDateString()}");
            var od = Directory.GetParent(dir);
            var ndirName = $"{dat.Year}-{dat.Month:00}-{dat.Day:00}-TLCGen_v{ver.Major}.{ver.Minor}.{ver.Build}.{ver.Revision}";
            var ndir = Path.Combine(od.FullName, ndirName);
            Console.WriteLine("Now copying...");
            try
            {
                if (Directory.Exists(ndir))
                {
                    Directory.Delete(ndir, true);
                } 
                DirectoryCopy(dir, ndir, true);
                Console.WriteLine("Renamed:");
                Console.WriteLine($"- old: {dir}");
                Console.WriteLine($"- new: {ndir}");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while renaming: " + e.ToString());
                return;
            }

            var zipf = Path.Combine(od.FullName, ndirName + ".zip");
            Console.WriteLine("Now packing...");
            try
            {
                if (File.Exists(zipf)) File.Delete(zipf);
                System.IO.Compression.ZipFile.CreateFromDirectory(ndir, zipf);
                Console.WriteLine($"Packed: {zipf}");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while packing: " + e.ToString());
                return;
            }
            Console.WriteLine("Now moving packed file...");
            try
            {
                if (Directory.Exists(mdir))
                {
                    var mz = Path.Combine(mdir, ndirName + ".zip");
                    File.Move(zipf, mz);
                    Console.WriteLine($"Moved to: {mz}");
                }
                else
                {
                    Console.WriteLine($"Folder '{mdir}' does not exist: could not move.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while moving: " + e.ToString());
                return;
            }
        }

        static void DisplayHelp()
        {
            Console.WriteLine("TLCGen Folder Version Renamer");
            Console.WriteLine("-----------------------------");
            Console.WriteLine();
            Console.WriteLine(
                "This utility can be used to copy a TLCGen folder to a new folder " +
                "with in its name the date and version of TLCGen in the original folder. " +
                "To this end, the program accepts a single argument, indicating the folder " +
                "to look for TLCGen.exe. The renamed folder is meant to be packed as the " +
                "'portable' version of TLCGen.");
            Console.WriteLine();
            Console.WriteLine(
                "Arguments: " +
                "   --help: display this help text" +
                "   --tlcgen-folder: the folder containing TLCGen.exe" +
                "   --target-folder: the folder to move the resulting zip to");
            Console.WriteLine("Example usage:");
            Console.WriteLine("    TLCGen.FolderVersionRenamer --tlcgen-folder \"C:\\test\\TLCGen\\bin\\Release\" --target-folder \"C:\\test\\TLCGen\\published\"");
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public static DateTime GetLinkerTime(System.Reflection.Assembly assembly, TimeZoneInfo target = null)
        {
            var filePath = assembly.Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;

            var buffer = new byte[2048];

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                stream.Read(buffer, 0, 2048);

            var offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

            var tz = target ?? TimeZoneInfo.Local;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, tz);

            return localTime;
        }
    }
}
