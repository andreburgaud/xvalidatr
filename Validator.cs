using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace xvalidatr;

/// <summary>
///     Main Validation class.
/// </summary>
public class Validator {
    private readonly string _nameSpace = string.Empty;
    private bool _error;
    private int _nonValidXmlCpt;
    private string _pathSchema = string.Empty;
    private XmlReaderSettings? _settings;
    private int _validXmlCpt;
    private bool _warning;

    /// <summary>
    ///     Unique constructor. Does not take any parameter. Displays "about" info.
    /// </summary>
    public Validator(string? schema) {
        if (schema is null) return;
        _pathSchema = schema;
        _nameSpace = GetNameSpace();
        ValidateSchema();
    }

    /// <summary>
    ///     Validates a schema.
    /// </summary>
    private void ValidateSchema() {
        const string title = "XML Schema Validation:";
        var schemaFile = _pathSchema;
        ColorConsole.PrintAction(title);
        try {
            var file = new FileInfo(_pathSchema);
            if (!file.Exists) {
                ColorConsole.PrintBright($"{_pathSchema}:");
                ColorConsole.PrintError("not found.");
                Environment.Exit(2);
            }
            else {
                schemaFile = file.FullName;
            }
        }
        catch (ArgumentException) {
            // Not very elegant. Found "Illegal characters in path", assuming wildcards.
            var directoryName = Path.GetDirectoryName(_pathSchema);
            if (string.IsNullOrEmpty(directoryName)) directoryName = Environment.CurrentDirectory;
            var directoryPath = Path.GetFullPath(directoryName);
            var wildcardPath = Path.GetFileName(_pathSchema);
            var wildcardFiles = Directory.GetFiles(directoryPath, wildcardPath);
            switch (wildcardFiles.Length) {
                case 0:
                    ColorConsole.PrintBright($"{_pathSchema}:");
                    ColorConsole.PrintError("not found.");
                    Environment.Exit(2);
                    break;
                case > 1:
                    ColorConsole.PrintBright($"{_pathSchema}:");
                    ColorConsole.PrintError(
                        "includes more than one potential schema files. Restrict the path to only one schema file.");
                    Environment.Exit(2);
                    break;
                default:
                    schemaFile = wildcardFiles[0];
                    _pathSchema = schemaFile;
                    break;
            }
        }

        try {
            ColorConsole.WriteBright($"{schemaFile}: ");
            _ = XmlSchema.Read(XmlReader.Create(_pathSchema), ValidationCallback);
            if (_error)
                ColorConsole.PrintError("Schema error(s).");
            else
                ColorConsole.PrintSuccess("OK");
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
    }

    private void CreateXmlReaderSettings() {
        _settings = new XmlReaderSettings();
        try {
            _settings.ValidationEventHandler += ValidationCallback;
            _settings.ValidationType = ValidationType.Schema;
            _settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
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

    /// <summary>
    ///     Handler used during the validation (XSD and XML).
    /// </summary>
    private void ValidationCallback(object? sender, ValidationEventArgs args) {
        if (!_error) Console.WriteLine();
        _error = true;
        switch (args.Severity) // There are only two severity types: Warning and Error
        {
            case XmlSeverityType.Warning:
                ColorConsole.WriteWarning("WARNING: ");
                break;
            case XmlSeverityType.Error:
                ColorConsole.WriteError("ERROR: ");
                break;
        }

        Console.Write($"line {args.Exception.LineNumber}, pos {args.Exception.LinePosition}, ");
        Console.WriteLine(args.Message); // Print the error to the screen.
    }

    /// <summary>
    ///     Extracts the namespace (targetNamespace) used in the XML schema. This
    ///     namespace will be used during the different validation process (XSD
    ///     and XML). XPath is used to extract this information.
    /// </summary>
    private string GetNameSpace() {
        const string title = "Extracting Namespace:";
        ColorConsole.PrintAction(title);

        var ns = "";
        var file = new FileInfo(_pathSchema);
        if (!file.Exists) {
            ColorConsole.PrintError($"{_pathSchema}: not found.");
            Environment.Exit(2);
        }

        try {
            var doc = new XPathDocument(file.FullName);
            var nav = doc.CreateNavigator();
            var ni = nav.Select("/*/@targetNamespace");
            if (ni.MoveNext())
                if (ni.Current is not null)
                    ns = ni.Current.Value;
        }
        catch (XmlException ex) {
            ColorConsole.PrintError($"Schema Error: {ex.Message}");
            Environment.Exit(3);
        }

        ColorConsole.WriteBright($"{file.FullName}: ");
        Console.WriteLine(ns.Length > 0 ? $"Uses namespace '{ns}'" : "Does not use a particular namespace.");
        return ns;
    }

    /// <summary>
    ///     Validates the XML files.
    /// </summary>
    /// <param name="xmlFiles">
    ///     XML files.
    /// </param>
    public void ValidateXmlFiles(IEnumerable<string> xmlFiles) {
        const string title = "XML File(s) Validation:";
        ColorConsole.PrintAction(title);
        CreateXmlReaderSettings();
        foreach (var xmlFile in xmlFiles) {
            var fi = new FileInfo(xmlFile);
            if (fi.Exists) // The function parameter is a file path
                ValidateXmlFile(xmlFile);
        }
    }

    ///<summary>Validates an XML file against a given schema.</summary>
    ///<param name="xmlFile">Path of the XML file to validate.</param>
    private void ValidateXmlFile(string xmlFile) {
        _error = false;
        _warning = false;
        ColorConsole.WriteBright($"{xmlFile}: ");
        try {
            var doc = new XmlDocument();
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

    /// <summary>
    ///     Prints the number of files processed, the number of success and number
    ///     of failure.
    /// </summary>
    public int PrintReport() {
        ColorConsole.PrintAction("XML Validation Summary:");
        var total = _validXmlCpt + _nonValidXmlCpt;
        if (total == 0) {
            ColorConsole.PrintError("XML file(s) or XML directory not found.");
        }
        else {
            ColorConsole.PrintBright($"{total} XML file(s) processed.");
            if (_validXmlCpt > 0) ColorConsole.PrintSuccess($"{_validXmlCpt} valid XML file(s).");
            if (_nonValidXmlCpt > 0) ColorConsole.PrintError($"{_nonValidXmlCpt} non valid XML file(s).");
        }

        return _nonValidXmlCpt;
    }
}