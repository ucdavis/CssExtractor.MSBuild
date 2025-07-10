# CssExtractor.MSBuild

## Overview

CssExtractor.MSBuild is a custom MSBuild task designed to extract CSS classes and styles from various file types, deduplicate them, and write them to a single output file. This output can be utilized by CSS processors such as Tailwind CSS. The task is configurable via regular expressions, allowing users to specify which files to exclude and how to extract CSS classes.

## Features

- Automatically extracts CSS classes from HTML, CSHTML, Razor, and C# files using built-in patterns
- Includes comprehensive patterns for standard HTML class attributes and common web frameworks
- Supports custom regex patterns for extracting dynamically generated CSS classes in C# code
- Deduplicates CSS classes to ensure a clean output
- Smart default output locations for library vs application projects
- Configurable file inclusion/exclusion patterns

## Installation

To use CssExtractor.MSBuild in your project, include it as a PackageReference in your project file (`.csproj`):

```xml
<!-- CSS Extraction -->
<ItemGroup>
  <PackageReference Include="CssExtractor.MSBuild" Version="1.3.0">
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    <!-- To make the build task available to consumers, omit the PrivateAssets element -->
    <PrivateAssets>all</PrivateAssets>
  </PackageReference>
</ItemGroup>
```

> Note: If you don't see xml comments in the above code block, see the [readme](https://github.com/ucdavis/CssExtractor.MSBuild) on GitHub.

## Configuration

CssExtractor.MSBuild works out-of-the-box with sensible defaults. It automatically scans common file types (HTML, CSHTML, Razor, and C# files) using built-in patterns that detect:

- Standard HTML `class` attributes
- React-style `className` attributes  
- Various CSS-in-JS patterns
- Common templating syntaxes

### Built-in Patterns

The extractor includes comprehensive built-in regex patterns for finding CSS classes in:
- HTML files (`**/*.html`)
- Razor files (`**/*.razor`) 
- CSHTML files (`**/*.cshtml`)
- C# files (`**/*.cs`)

### Custom Patterns for Dynamic CSS Classes

While the built-in patterns handle most standard cases, you can define custom patterns to extract CSS classes that are dynamically generated in your C# code. Suppose you have some fluent API calls like:

```csharp
var component = builder.WithClass("btn btn-primary");
var icon = element.WithIcon("fas fa-user");
```

You can define custom extraction patterns in your project file as follows:

```xml
<PropertyGroup>
  <CssExtractorCustomPatterns>
    \.WithClass\s*\(\s*"([^"]+)"\s*\)
    ;
    \.WithIcon\s*\(\s*"([^"]+)"\s*\)
  </CssExtractorCustomPatterns>
</PropertyGroup>
```

### Advanced Configuration

There is another way to specify custom patterns that is transitive, meaning consumers of your library can also benefit from these patterns without needing to redefine them. This is useful for libraries that want to provide a consistent extraction experience.

Create a file named `content/css-extractor-patterns.txt` in your project as follows:

```txt
\.WithClass\s*\(\s*"([^"]+)"\s*\)
\.WithIcon\s*\(\s*"([^"]+)"\s*\)
```

Then, update your project file to pack it:

```xml
<ItemGroup>
    <None Include="content/css-extractor-patterns.txt" Pack="true" PackagePath="content/css-extractor-patterns.txt" />
</ItemGroup>
```

You can also customize files to include/exclude and output location:

```xml
<PropertyGroup>
  <!-- Custom output file location -->
  <CssExtractorOutputFile>$(MSBuildProjectDirectory)/styles/extracted-classes.txt</CssExtractorOutputFile>
</PropertyGroup>

<ItemGroup>
  <!-- Add additional file types to scan -->
  <CssExtractorIncludeFiles Include="**/*.tsx" />
  <CssExtractorIncludeFiles Include="**/*.jsx" />
  
  <!-- Exclude additional directories -->
  <CssExtractorExcludeFiles Include="**/tests/**" />
</ItemGroup>
```

## License

This project is licensed under the MIT License. See the LICENSE file for more details.