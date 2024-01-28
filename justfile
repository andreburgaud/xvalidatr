
APP := "xvalidatr"
VERSION := "2.0.0"
RUNTIME := os() + "-" + arch()
RUNTIME_ID := if RUNTIME == "linux-x86_64" {
  "linux-x64"
} else if RUNTIME == "macos-aarch64" {
  "osx-arm64"
} else {
  "UNKNOWN"
}

# Default recipe (this list)
default:
    @echo "{{APP}} {{VERSION}}"
    @echo "OS: {{os()}}, OS Family: {{os_family()}}, architecture: {{arch()}}"
    @echo "Runtime   : {{RUNTIME}}"
    @echo "Runtime ID: {{RUNTIME_ID}}"
    @just --list

# Build the .NET project (debug)
build:
    dotnet build

# Publish a .NET project for deployment (release)
publish:
    dotnet publish --configuration Release --self-contained --runtime {{RUNTIME_ID}}
    zip -j xvalidatr_{{RUNTIME_ID}}_{{VERSION}}.zip ./bin/Release/net8.0/{{RUNTIME_ID}}/publish/{{APP}}

# Build and run the .NET project output
run:
    dotnet run

# Delete the generated files
clean:
    dotnet clean

# Push and tag the code to Github
github-push:
    @git push
    @git tag -a {{VERSION}} -m "Version {{VERSION}}"
    @git push origin --tags
