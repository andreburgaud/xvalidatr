using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Security.Cryptography.X509Certificates;

namespace xvalidatr {
    ///<summary>
    /// Generic command line XML Schema and XML files validator.
    ///</summary>
    public class App {
        private static String GetAssemblyTitle(Assembly assembly) {
            foreach (Attribute attr in assembly.GetCustomAttributes(true).Cast<Attribute>()) {
                if (attr is AssemblyTitleAttribute) {
                    return (attr as AssemblyTitleAttribute)?.Title ?? "";
                }
            }
            return "";
        }

        ///<summary>
        /// Display About text.
        ///</summary>
        private static void About(Assembly assembly) {
            //AssemblyCopyrightAttribute? copyright;
            Attribute? copyright;
            if (assembly is not null) {
                copyright = Attribute.GetCustomAttribute(assembly, typeof(AssemblyCopyrightAttribute));
                Version? version = assembly.GetName().Version;
                if (version is not null) {
                    ColorConsole.PrintAbout($"{GetAssemblyTitle(assembly)} {version.Major}.{version.Minor}.{version.Build}");
                }
                if (copyright is not null) {
                    ColorConsole.PrintAbout(((AssemblyCopyrightAttribute)copyright).Copyright);
                }
            }
        }

        ///<summary>
        /// Retrieves all file names from arguments passed to the application. Arguments are files that
        /// may contain wildcards.
        ///</summary>
        private static string[] GetAllFiles(string[] args) {
            string[] paths = new string[args.Length - 1];
            for (int i = 0; i < args.Length - 1; i++) {
                paths[i] = args[i + 1];
            }
            ArrayList xmlFiles = [];
            bool found;
            foreach (string path in paths) {
                found = false;
                if (File.Exists(path)) {
                    var fileInfo = new FileInfo(path);
                    found = true;
                    xmlFiles.Add(fileInfo.FullName);
                }
                else if (Directory.Exists(path)) {
                    var files = Directory.EnumerateFiles(path, "*.xml", SearchOption.AllDirectories);
                    foreach (string fileName in files) {
                        xmlFiles.Add(fileName);
                    }
                    found = files != null;
                }
                if (!found) {
                    ColorConsole.PrintError($"'{path}': path not found or no XML found in path.");
                }
            }
            return (string[])xmlFiles.ToArray(typeof(String));
        }

        ///<summary>
        /// Display the usage when no parameterm is passed to the executable.
        ///</summary>
        private static void Usage(Assembly assembly) {
            ColorConsole.PrintWarning("Description:");
            Console.WriteLine("    Validate one or more XML files against an XML Schema Definition (XSD) file.");
            Console.WriteLine();
            ColorConsole.PrintWarning("Usage:");
            var exe = GetAssemblyTitle(assembly);
            Console.WriteLine($"    {exe} [OPTIONS]");
            Console.WriteLine($"    {exe} <schema_file> [ <xml_files> | <xml_dir> ]");
            Console.WriteLine();
            ColorConsole.PrintWarning("Options:");
            Console.WriteLine("    -h, --help         Print help");
            Console.WriteLine("    -v, --version      Print version info");
            Console.WriteLine();
            ColorConsole.PrintWarning("Examples:");
            Console.WriteLine($"    {exe} books.xsd books.xml");
            Console.WriteLine($"    {exe} books.xsd books1.xml books2.xml");
            Console.WriteLine($"    {exe} books.xsd xml_dir");
        }

        ///<summary>
        /// Main method: instantiate the Validator class and calls the validate
        /// methods.
        ///</summary>
        public static int Main(string[] args) {
            int res = 0;
            Assembly assembly = Assembly.GetExecutingAssembly();

            if (args.Length < 1) {
                About(assembly);
                Usage(assembly);
                Environment.Exit(1);
            }

            switch (args[0]) {
                case "-h" or "--help":
                    About(assembly);
                    Usage(assembly);
                    Environment.Exit(0);
                    break;
                case "-v" or "--version":
                    Version? version = assembly.GetName().Version;
                    if (version is not null) {
                        ColorConsole.PrintSuccess($"{GetAssemblyTitle(assembly)} {version.Major}.{version.Minor}.{version.Build}");
                    }
                    Environment.Exit(0);
                    break;
            }

            About(assembly);
            var validator = new Validator(args[0]); // args[0] is xsd
            if (args.Length > 1) {
                // 2nd argument is XML or directory containing XML files
                string[] xmlFiles = GetAllFiles(args);
                if (xmlFiles.Length > 0) {
                    validator.validateXmlFiles(xmlFiles);
                }
                else {
                    ColorConsole.PrintError("No XML files found.");
                }
                res = validator.printReport();
            }
            return res;
        }
    }
}
