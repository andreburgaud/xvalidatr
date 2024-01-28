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
                    if (attr is not null) {
                        return (attr as AssemblyTitleAttribute).Title;
                    }
                }
            }
            return "";
        }

        ///<summary>
        /// Display About text.
        ///</summary>
        private static void About() {
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyCopyrightAttribute copyright;
            if (assembly is not null) {
                copyright = (AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyCopyrightAttribute));
                Version? version = assembly.GetName().Version;
                if (version is not null) {
                    ColorConsole.PrintAbout($"{GetAssemblyTitle(assembly)} {version.Major}.{version.Minor}.{version.Build}");
                }
                if (copyright is not null) {
                    ColorConsole.PrintAbout(copyright.Copyright);
                }
            }
        }

        ///<summary>
        /// Retrieves all file names from arguments passed to the application. Arguments are files that
        /// may contain wildcards.
        ///</summary>
        private static string[] getAllFiles(string[] args) {
            string[] paths = new string[args.Length - 1];
            for (int i = 0; i < args.Length - 1; i++) {
                paths[i] = args[i + 1];
            }
            ArrayList xmlFiles = new ArrayList();
            bool found;
            foreach (string path in paths) {
                found = false;
                if (File.Exists(path)) {
                    FileInfo fileInfo = new FileInfo(path);
                    found = true;
                    xmlFiles.Add(fileInfo.FullName);
                }
                else if (Directory.Exists(path)) {
                    var files = Directory.EnumerateFiles(path, "*.xml", SearchOption.AllDirectories);
                    foreach (string fileName in files) {
                        Console.WriteLine(fileName);
                        xmlFiles.Add(fileName);
                    }
                    found = files == null ? false : true;
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
        private static void Usage() {
            ColorConsole.PrintWarning("Usage:");
            Console.WriteLine("    xvalidatr <schema_file> [ <xml_files> ]");
            Environment.Exit(1);
        }

        ///<summary>
        /// Main method: instantiate the Validator class and calls the validate
        /// methods.
        ///</summary>
        public static int Main(string[] args) {
            int res = 0;
            About();
            if (args.Length < 1) {
                Usage();
            }
            Validator validator = new Validator(args[0]); // args[0] is xsd
            if (args.Length > 1) {
                // 2nd argument is XML or directory containing XML files
                string[] xmlFiles = getAllFiles(args);
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
