#region Using directives
using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Reflection;
using System.Collections;
#endregion

namespace xvalidatr {
    ///<summary>
    /// Generic command line XML Schema and XML files validator.
    ///</summary>
    public class Validator {
        bool _error = false;
        bool _warning = false;
        string _nameSpace = "";
        string _pathSchema;
        int _validXmlCpt = 0;
        int _nonValidXmlCpt = 0;
        XmlReaderSettings _settings;

        ///<summary>
        /// Unique constructor. Does not take any parameter. Displays "about" info.
        ///</summary>
        public Validator(string schema) {
            _pathSchema = schema;
            _nameSpace = getNameSpace();
            validateSchema();
        }

        ///<summary>
        /// Validates a schema.
        ///</summary>
        public bool validateSchema() {
            string title = "XML Schema Validation:";
            string schemaFile = _pathSchema;
            ColorConsole.PrintAction(title);
            try {
                FileInfo file = new FileInfo(_pathSchema);
                if (!file.Exists) {
                    ColorConsole.PrintBright("{0}:", _pathSchema);
                    ColorConsole.PrintError("not found.");
                    Environment.Exit(2);
                }
                else {
                    schemaFile = file.FullName;
                }
            }
            catch (System.ArgumentException) {
                // Not very elegant. Found "Illegal characters in path", assuming wildcards.
                string directoryName = System.IO.Path.GetDirectoryName(_pathSchema);
                if (directoryName == null || directoryName.Length == 0) {
                    directoryName = System.Environment.CurrentDirectory;
                }
                string directoryPath = System.IO.Path.GetFullPath(directoryName);
                string wildcardPath = System.IO.Path.GetFileName(_pathSchema);
                string[] wildcardFiles = Directory.GetFiles(directoryPath, wildcardPath);
                if (wildcardFiles.Length == 0) {
                    ColorConsole.PrintBright("{0}:", _pathSchema);
                    ColorConsole.PrintError("not found.");
                    Environment.Exit(2);
                }
                else if (wildcardFiles.Length > 1) {
                    ColorConsole.PrintBright("{0}:", _pathSchema);
                    ColorConsole.PrintError("includes more than one potential schema files. Restrict the path to only one schema file.");
                    Environment.Exit(2);
                }
                else {
                    schemaFile = wildcardFiles[0];
                    _pathSchema = schemaFile;
                }
            }
            try {
                ColorConsole.WriteBright("{0}: ", schemaFile);
                XmlSchema.Read(XmlReader.Create(_pathSchema), new ValidationEventHandler(ValidationCallback));
                if (_error) {
                    ColorConsole.PrintError("Schema error(s).");
                }
                else {
                    ColorConsole.PrintSuccess("OK");
                }
            }
            catch (XmlSchemaException ex) {
                ColorConsole.PrintError(ex.Message);
                Environment.Exit(3);
            }
            catch (XmlException ex) {
                ColorConsole.PrintError(ex.Message);
                Environment.Exit(3);
            }
            catch (Exception ex) {
                ColorConsole.PrintError(ex.Message);
                Environment.Exit(3);
            }
            return _error;
        }

        private void createXmlReaderSettings() {
            _settings = new XmlReaderSettings();
            try {
                _settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallback);
                _settings.ValidationType = ValidationType.Schema;
                _settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
                _settings.Schemas.Add(_nameSpace, _pathSchema);
            }
            catch (XmlSchemaException ex) {
                ColorConsole.PrintError("Schema Error: {0}", ex.Message);
            }
            catch (XmlException ex) {
                ColorConsole.PrintError("Schema Error: {0}", ex.Message);
                Environment.Exit(3);
            }
        }

        ///<summary>
        /// Handler used during the validation (XSD and XML).
        ///</summary>
        private void ValidationCallback(object sender, ValidationEventArgs args) {
            if (!_error) { Console.WriteLine(); }
            _error = true;
            if (args.Severity == XmlSeverityType.Warning) {
                ColorConsole.WriteWarning("WARNING: ");
            }
            else if (args.Severity == XmlSeverityType.Error) {
                ColorConsole.WriteError("ERROR: ");
            }
            Console.Write("line {0}, pos {1}, ", args.Exception.LineNumber, args.Exception.LinePosition);
            Console.WriteLine(args.Message); // Print the error to the screen.
        }

        ///<summary>
        /// Extracts the namespace (targetNamespace) used in the XML schema. This
        /// namespace will be used during the different validation process (XSD
        /// and XML). XPath is used to extract this information.
        ///</summary>
        public string getNameSpace() {
            string title = "Extracting Namespace:";
            ColorConsole.PrintAction(title);

            string ns = "";
            FileInfo file = new FileInfo(_pathSchema);
            if (!file.Exists) {
                ColorConsole.PrintError("{0}: not found.", _pathSchema);
                Environment.Exit(2);
            }
            try {
                XPathDocument doc = new XPathDocument(file.FullName);
                XPathNavigator nav = doc.CreateNavigator();
                XPathNodeIterator ni = nav.Select("/*/@targetNamespace");
                if (ni.MoveNext()) {
                    ns = ni.Current.Value;
                }
            }
            catch (XmlException ex) {
                ColorConsole.PrintError("Schema Error: {0}", ex.Message);
                Environment.Exit(3);
            }
            ColorConsole.WriteBright("{0}: ", file.FullName);
            if (ns.Length > 0) {
                Console.WriteLine("Uses namespace '{1}'", file.FullName, ns);
            }
            else {
                Console.WriteLine("Does not use a particular namespace.", file.FullName);
            }
            return ns;
        }

        ///<summary>
        /// Triggers the XML validation. Determines if the parameter is a file or
        /// Directory.
        ///</summary>
        ///<param name="xml">
        /// Path to an XML file or a directory containing XML files.
        ///</param>
        public void validateXmlFiles(string[] xmlFiles) {
            string title = "XML File(s) Validation:";
            ColorConsole.PrintAction(title);
            createXmlReaderSettings();
            foreach (string xmlFile in xmlFiles) {
                FileInfo fi = new FileInfo(xmlFile);
                if (fi.Exists) { // The function parameter is a file path
                    validateXmlFile(xmlFile);
                }
            }
        }

        ///<summary>Validates an XML file against a given schema.</summary>
        ///<param name="xmlFile">Path of the XML file to validate.</param>
        private void validateXmlFile(string xmlFile) {
            _error = false;
            _warning = false;
            ColorConsole.WriteBright("{0}: ", xmlFile);
            try {
                XmlDocument doc = new XmlDocument();
                doc.Load(XmlReader.Create(xmlFile, _settings));
            }
            catch (XmlException ex) {
                _error = true;
                ColorConsole.PrintError(ex.Message);
            }
            catch (FileNotFoundException ex) {
                _error = true;
                ColorConsole.PrintError(ex.Message);
            }
            catch (Exception ex) {
                _error = true;
                ColorConsole.PrintError(ex.Message);
            }
            if (_error) {
                _nonValidXmlCpt += 1;
            }
            else if (_warning) {
                _validXmlCpt += 1;
            }
            else {
                _validXmlCpt += 1;
                ColorConsole.PrintSuccess("OK");
            }
        }

        ///<summary>
        /// Prints the number of files processed, the number of success and number
        /// of failure.
        ///</summary>
        public int printReport() {
            ColorConsole.PrintAction("XML Validation Summary:");
            int total = _validXmlCpt + _nonValidXmlCpt;
            if (total == 0) {
                ColorConsole.PrintError("XML file(s) or XML directory not found.");
            }
            else {
                ColorConsole.PrintBright("{0} XML file(s) processed.", total);
                if (_validXmlCpt > 0) {
                    ColorConsole.PrintSuccess("{0} valid XML file(s).", _validXmlCpt);
                }
                if (_nonValidXmlCpt > 0) {
                    ColorConsole.PrintError("{0} non valid XML file(s).", _nonValidXmlCpt);
                }
            }
            return _nonValidXmlCpt;
        }

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
