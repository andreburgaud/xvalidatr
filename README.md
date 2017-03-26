# XvalidatR

XML Schema Validator for Windows. `XvalidatR` uses external schemas (XSD files).

![xvalidatr](https://cloud.githubusercontent.com/assets/6396088/23348810/373079e4-fc73-11e6-9e62-732b58025064.png)

## Installation

1. Download the zip file from the releases tab (https://github.com/andreburgaud/xvalidatr/files/786713/XvalidatR_1.0.0.zip)
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
* You can combine the different options (i.e. directory containing XML files, wild cards, individual files).
* The examples provided above can all be tested from the directory tests included in the repository.
* The search is recursive when passing a valid directory as argument. It searches for all `*.xml` files in the given directory and all subdirectories.
* To restrict XML file validation to a directory level (not its subdirectories), the argument can be a directory and a wild card for the files. For example, if the directory is named `xml`, the argument can be given as: `xml/*.xml`. This will prevent to search any xml file with extention `.xml` in each subdirectory.

## Build

Build the binary using visual studio or download the zip file from the releases tab.

