using System;
using System.IO;
using System.Reflection;
using System.Collections;

namespace xvalidatr {
    ///<summary>
    /// Generic command line XML Schema and XML files validator.
    ///</summary>
    public class App {
        private static String getAssemblyTitle(Assembly assembly) {
            foreach (Attribute attr in assembly.GetCustomAttributes(true)) {
                if (attr is AssemblyTitleAttribute) {
                    return (attr as AssemblyTitleAttribute).Title;
                }
            }
            return null;
        }

        ///<summary>
        /// Display About text.
        ///</summary>
        private static void About() {
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyCopyrightAttribute copyright;
            copyright = (AssemblyCopyrightAttribute)AssemblyCopyrightAttribute.GetCustomAttribute(assembly,
                                                                                                  typeof(AssemblyCopyrightAttribute));
            Version version = assembly.GetName().Version;
            string line = String.Format("{0} {1}.{2}.{3}", getAssemblyTitle(assembly), version.Major, version.Minor, version.Build);
            ColorConsole.PrintAbout(line);
            ColorConsole.PrintAbout(copyright.Copyright);
        }

        ///<summary>
        /// Retreives all file names from arguments passed to the application. Arguments are files that
        /// may contain wildcards.
        ///</summary>
        private static string[] getAllFiles(string[] args) {
            string[] paths = new string[args.Length - 1];
            for (int i = 0; i < args.Length - 1; i++) {
                paths[i] = args[i + 1];
            }
            ArrayList xmlFiles = new ArrayList();
            string directoryName = null;
            string directoryPath = null;
            string wildcardPath = null;
            bool found;
            foreach (string path in paths) {
                found = false;
                if (File.Exists(path)) {
                    FileInfo fileInfo = new FileInfo(path);
                    found = true;
                    xmlFiles.Add(fileInfo.FullName);
                }
                else if (Directory.Exists(path)) {
                    string[] files = Directory.GetFiles(path);
                    foreach (string file in files) {
                        FileInfo fileInfo = new FileInfo(file);
                        if (fileInfo.Extension == ".xml") {
                            found = true;
                            xmlFiles.Add(fileInfo.FullName);
                        }
                    }
                }
                else {
                    // Assuming wildcards.
                    directoryName = System.IO.Path.GetDirectoryName(path);
                    if (directoryName == null || directoryName.Length == 0) {
                        directoryName = System.Environment.CurrentDirectory;
                    }
                    directoryPath = System.IO.Path.GetFullPath(directoryName);
                    wildcardPath = System.IO.Path.GetFileName(path);
                    string[] wildcardFiles = Directory.GetFiles(directoryPath, wildcardPath);
                    if (wildcardFiles.Length > 0) {
                        found = true;
                        foreach (string fileName in wildcardFiles) {
                            xmlFiles.Add(fileName);
                        }
                    }
                }
                if (!found) {
                    ColorConsole.PrintError("'{0}': path not found or no XML found in path.", path);
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
