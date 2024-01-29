# XvalidatR

XML Schema Validator for Windows, Mac OSX and Linux. `XvalidatR` uses external schemas (XSD files).

![xvaidatr_2 0 0](https://github.com/andreburgaud/xvalidatr/assets/6396088/c885f2cd-20e7-4311-b97c-a4d7d23c9604)

## Installation

1. Download the zip file for your machine from the GitHub release tab (https://github.com/andreburgaud/xvalidatr/releases).
1. Extract `xvalidatr` or `xvalidatr.exe` from the zip file
3. Copy the executable into a directory included in your `PATH`

## Usage Examples

To only validate the XML schema:

```
xvalidatr books.xsd
```

To validate an XML file against an XML schema:

```
xvalidatr books.xsd books.xml
```

To validate several XML files against an XML schema:

```
xvalidatr books.xsd *.xml
```

To validate several XML files in a directory against XML schema:

```
xvalidatr books.xsd xml
```

**Notes**:
* You can combine the different options (i.e. directory containing XML files, wild cards, individual files).
* The examples provided above can all be tested from the directory tests included in the repository.
* The search is recursive when passing a valid directory as argument. It searches for all `*.xml` files in the given directory and all subdirectories.
* To restrict XML file validation to a directory level (not its subdirectories), the argument can be a directory and a wild card for the files. For example, if the directory is named `xml`, the argument can be given as: `xml/*.xml`. This will prevent to search any xml file with extention `.xml` in each subdirectory.

## Build

### Build on Windows

**Prerequisite**: Install `dotnet` (see https://dotnet.microsoft.com/en-us/download)

Debug build:

```
dotnet build
```

Standalone release executable:

```
dotnet publish --configuration Release --self-contained --runtime win-x64
```

To create a zip file with the standalone executable (assuming version 2.0.0):

```
powershell -Command "Compress-Archive -Path 'bin\Release\net8.0\win-x64\publish\xvalidatr.exe' -DestinationPath 'xvalidatr_win-x64_2.0.0.zip'"
```

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

Standalone release executable:

```
dotnet publish --configuration Release --self-contained --runtime <runtime_id>
```

For example, the `runtime_id` is `linux-x64` on a Linux x86-64 machine and `osx-arm64` for a Mac M1 or M2. You can find the list of all RIDs at https://learn.microsoft.com/en-us/dotnet/core/rid-catalog.


Clean the project:


```
dotnet clean
```
