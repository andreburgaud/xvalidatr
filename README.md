# XvalidatR

XML Schema Validator for Windows, Mac OSX and Linux. `XvalidatR` uses external schemas (XSD files).

![xvalidatr](https://cloud.githubusercontent.com/assets/6396088/23348810/373079e4-fc73-11e6-9e62-732b58025064.png)

## Installation on Windows

1. Download the zip file from the releases tab (https://github.com/andreburgaud/xvalidatr/files/786713/XvalidatR_1.3.0.zip)
2. Extract `xvalidatr.exe` from the zip file
3. Copy `xvalidatr.exe` a directory included in your `PATH`

## Installation on Linux and Mac OSX

Prerequisite: Mono needs to be installed on the target system

1. Download the zip file from the releases tab (https://github.com/andreburgaud/xvalidatr/files/786713/XvalidatR_1.3.0.zip)
2. Extract `xvalidatr.exe` from the zip file
3. Copy `xvalidatr.exe` in a directory included in your `PATH`
4. In the same directory create a file named `xvalidatr` with the following content:

```
#!/bin/bash
path=$(dirname "$0")
mono "$path/xvalidatr.exe" $*
```

5. Make `xvalidatr` executable: `$ chmod +x xvalidatr`

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

Recommendation: Use Visual Studio.

### Build on Mac OSX or Linux

Assuming that you installed `mono`:

```
$ msbuild
```

or (deprecated)

```
$ xbuild
```

For a Release build:

```
$ msbuild /p:Configuration=Release
```

or (deprecated)

```
$ xbuild /p:Configuration=Release
```

To clean the project:


```
$ msbuild /t:Clean
```

or (deprecated)

```
$ xbuild /t:Clean
```
