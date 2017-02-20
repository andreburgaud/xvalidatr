# XvalidatR

XML Schema Validator for Windows. `XvalidatR uses external schemas (XSD files).

## Installation

1. Download the zip file from the releases tab
2. Extract `xvalidatr.exe` and `xvalidatr.exe.config` from the zip file
3. Copy those two files in a directory included in your `PATH`

## Usage

From the command line, run xvalidatr with at one arguments: one schema (XSD file), and at least one XML file.

### Usage Examples

To only validate the XML schema:

```
> xvalidatr books.xsd
```

To validate an XML file against an XML schema:

```
> xvalidatr books.xsd books.xml
```

To validate several XML files against an XML schema:

```
> xvalidatr books.xsd *.xml
```

To validate several XML files in a directory against XML schema:

```
> xvalidatr books.xsd xml
```

**Notes**:
* As of version 1.0.0, the latter option is not recursive and processes only files with extension '.xml' in a directory passed as a parameter.
* You can combine the different options (i.e. directory containing XML files, wild cards, individual files).
* The examples provided above can all be tested from the directory tests included in the reop.

## Build

Build the binary using visual studio or download the zip file from the releases tab.

