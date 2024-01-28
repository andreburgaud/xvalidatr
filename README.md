# XvalidatR

XML Schema Validator for Windows, Mac OSX and Linux. `XvalidatR` uses external schemas (XSD files).

![xvalidatr](https://cloud.githubusercontent.com/assets/6396088/23348810/373079e4-fc73-11e6-9e62-732b58025064.png)

## Installation

1. Download the zip file for your machine from the GitHub release tab (https://github.com/andreburgaud/xvalidatr/releases).
1. Extract `xvalidatr` or `xvalidatr.exe` from the zip file
3. Copy the executable into a directory included in your `PATH`

## Usage Examples

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

### Build on Windows

TBD

### Build on Mac OSX or Linux

Prerequisite: install `dotnet` (https://learn.microsoft.com/en-us/dotnet/core/install/linux-ubuntu).


Debug build:

```
dotnet build
```

Release build:

```
dotnet publish
```

Standalone executable:

```
dotnet publish --configuration Release --self-contained --runtime <runtime_id>
```

For example, the `runtime_id` is `linux-x64` on a Linux x86-64 machine. You can find the list of all RIDs at https://learn.microsoft.com/en-us/dotnet/core/rid-catalog.


Clean the project:


```
dotnet clean
```
