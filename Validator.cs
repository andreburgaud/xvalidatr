using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Reflection;
using System.Collections;

namespace xvalidatr {
    ///<summary>
    /// Main Validation class.
    ///</summary>
    public class Validator {
        bool _error = false;
        bool _warning = false;
        string _nameSpace = string.Empty;
        string _pathSchema = string.Empty;
        int _validXmlCpt = 0;
        int _nonValidXmlCpt = 0;
        XmlReaderSettings? _settings;

        ///<summary>
        /// Unique constructor. Does not take any parameter. Displays "about" info.
        ///</summary>
        public Validator(string? schema) {
            if (schema is not null) {
                _pathSchema = schema;
                _nameSpace = getNameSpace();
                ValidateSchema();
            }
        }

        ///<summary>
        /// Validates a schema.
        ///</summary>
        public bool ValidateSchema() {
            string title = "XML Schema Validation:";
            string schemaFile = _pathSchema;
            ColorConsole.PrintAction(title);
            try {
                FileInfo file = new FileInfo(_pathSchema);
                if (!file.Exists) {
                    ColorConsole.PrintBright($"{_pathSchema}:");
                    ColorConsole.PrintError("not found.");
                    Environment.Exit(2);
                }
                else {
                    schemaFile = file.FullName;
                }
            }
            catch (System.ArgumentException) {
                // Not very elegant. Found "Illegal characters in path", assuming wildcards.
                string? directoryName = Path.GetDirectoryName(_pathSchema);
                if (directoryName == null || directoryName.Length == 0) {
                    directoryName = Environment.CurrentDirectory;
                }
                string directoryPath = Path.GetFullPath(directoryName);
                string wildcardPath = Path.GetFileName(_pathSchema);
                string[] wildcardFiles = Directory.GetFiles(directoryPath, wildcardPath);
                if (wildcardFiles.Length == 0) {
                    ColorConsole.PrintBright($"{_pathSchema}:");
                    ColorConsole.PrintError("not found.");
                    Environment.Exit(2);
                }
                else if (wildcardFiles.Length > 1) {
                    ColorConsole.PrintBright($"{_pathSchema}:");
                    ColorConsole.PrintError("includes more than one potential schema files. Restrict the path to only one schema file.");
                    Environment.Exit(2);
                }
                else {
                    schemaFile = wildcardFiles[0];
                    _pathSchema = schemaFile;
                }
            }
            try {
                ColorConsole.WriteBright($"{schemaFile}: ");
                _ = XmlSchema.Read(XmlReader.Create(_pathSchema), new ValidationEventHandler(ValidationCallback));
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

        private void CreateXmlReaderSettings() {
            _settings = new XmlReaderSettings();
            try {
                _settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallback);
                _settings.ValidationType = ValidationType.Schema;
                _settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
                _settings.Schemas.Add(_nameSpace, _pathSchema);
            }
            catch (XmlSchemaException ex) {
                ColorConsole.PrintError($"Schema Error: {ex.Message}");
            }
            catch (XmlException ex) {
                ColorConsole.PrintError($"Schema Error: {ex.Message}");
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
            Console.Write($"line {args.Exception.LineNumber}, pos {args.Exception.LinePosition}, ");
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
                ColorConsole.PrintError($"{_pathSchema}: not found.");
                Environment.Exit(2);
            }
            try {
                XPathDocument doc = new XPathDocument(file.FullName);
                XPathNavigator nav = doc.CreateNavigator();
                XPathNodeIterator ni = nav.Select("/*/@targetNamespace");
                if (ni.MoveNext()) {
                    if (ni.Current is not null) {
                        ns = ni.Current.Value;
                    }
                }
            }
            catch (XmlException ex) {
                ColorConsole.PrintError($"Schema Error: {ex.Message}");
                Environment.Exit(3);
            }
            ColorConsole.WriteBright($"{file.FullName}: ");
            if (ns.Length > 0) {
                Console.WriteLine($"Uses namespace '{ns}'");
            }
            else {
                Console.WriteLine("Does not use a particular namespace.");
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
            CreateXmlReaderSettings();
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
            ColorConsole.WriteBright($"{xmlFile}: ");
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
                ColorConsole.PrintBright($"{total} XML file(s) processed.");
                if (_validXmlCpt > 0) {
                    ColorConsole.PrintSuccess($"{_validXmlCpt} valid XML file(s).");
                }
                if (_nonValidXmlCpt > 0) {
                    ColorConsole.PrintError($"{_nonValidXmlCpt} non valid XML file(s).");
                }
            }
            return _nonValidXmlCpt;
        }
    }
}