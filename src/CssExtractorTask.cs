using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CssExtractor.MSBuild
{
    public class CssExtractorTask : Microsoft.Build.Utilities.Task
    {
        public ITaskItem[] CssExtractorIncludeFiles { get; set; } = [];
        public string CssExtractorOutputFile { get; set; } = "extracted-classes.txt";
        public ITaskItem[] CssExtractorExcludeFiles { get; set; } = [];
        public string CssExtractorPatterns { get; set; } = "";

        public override bool Execute()
        {
            try
            {
                // Split patterns on ';', trim, and ignore empty
                var extractionPatterns = (CssExtractorPatterns ?? "")
                    .Split(';')
                    .Select(p => p.Trim())
                    .Where(p => !string.IsNullOrEmpty(p))
                    .ToArray();

                var excludeFiles = new HashSet<string>(
                    CssExtractorExcludeFiles?.Select(x => Path.GetFullPath(x.ItemSpec)) ?? [],
                    StringComparer.OrdinalIgnoreCase);

                var allFiles = CssExtractorIncludeFiles?.Select(f => f.ItemSpec) ?? [];
                var filesToProcess = allFiles
                    .Where(file => !excludeFiles.Contains(Path.GetFullPath(file)))
                    .Distinct()
                    .ToList();

                Log.LogMessage(MessageImportance.Normal, $"Processing {filesToProcess.Count} files with {extractionPatterns.Length} patterns");
                Log.LogMessage(MessageImportance.Normal, $"Patterns: {string.Join("; ", extractionPatterns)}");
                
                foreach (var file in filesToProcess.Take(5)) // Log first 5 files as sample
                {
                    Log.LogMessage(MessageImportance.Normal, $"File: {file}");
                }
                if (filesToProcess.Count > 5)
                {
                    Log.LogMessage(MessageImportance.Normal, $"... and {filesToProcess.Count - 5} more files");
                }

                var cssClasses = CssExtractor.ExtractCssClasses(filesToProcess, extractionPatterns);
                CssExtractor.WriteToFile(cssClasses, CssExtractorOutputFile.Trim());

                Log.LogMessage(MessageImportance.Normal, $"Extracted {cssClasses.Count} CSS classes to {CssExtractorOutputFile}");
                return true;
            }
            catch (Exception ex)
            {
                Log.LogError($"CSS extraction failed: {ex.Message}");
                return false;
            }
        }
    }
}