using ProcessMemory64;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Noturnal_Cheat_External
{
    public static class FileSearcher
    {
        public class FoundInt
        {
            public int Value { get; set; }
            public int Line { get; set; }
            public string LineText { get; set; }
            public string Scope { get; set; }
            public int CharIndex { get; set; }
            public string FilePath { get; set; }
            public override string ToString() =>
                $"{FilePath}:{Line} [{Scope}] = 0x{Value:X} -> {LineText}";
        }

        // Find all matches of the variable in a single file
        public static List<FoundInt> FindIntsInFile(string filePath, string variableName)
        {
            var results = new List<FoundInt>();

            if (!File.Exists(filePath))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("[!] ");

                Console.ResetColor();
                Console.WriteLine($"File not found: {filePath}");
                return results;
            }
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("[...] ");

            Console.ResetColor();
            Console.WriteLine($"Searching '{filePath}' for '{variableName}'...");

            string text = File.ReadAllText(filePath);

            // Regex: optional access modifier, optional static, const + (n?)int + variableName = (hex or decimal)
            string pattern = $@"\b(?:public|private|protected|internal)?\s*(?:static\s+)?const\s+n?int\s+{Regex.Escape(variableName)}\s*=\s*(?<value>0x[0-9A-Fa-f]+|\d+)";
            var matches = Regex.Matches(text, pattern, RegexOptions.Compiled | RegexOptions.Multiline);

            if (matches.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("[-] ");

                Console.ResetColor();
                Console.WriteLine($"No matches found for '{variableName}' in {filePath}");

                return results;
            }

            // prepare line array once
            var lines = Regex.Split(text, "\r\n|\r|\n");

            // regex to find nearest namespace/class/struct going backwards
            var scopeRegex = new Regex(@"\b(?:namespace|class|struct)\s+([A-Za-z_]\w*)", RegexOptions.Compiled | RegexOptions.RightToLeft);

            foreach (Match m in matches)
            {
                string valueStr = m.Groups["value"].Value;
                int value;
                try
                {
                    value = valueStr.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
                        ? Convert.ToInt32(valueStr, 16)
                        : int.Parse(valueStr);
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("[!] ");

                    Console.ResetColor();
                    Console.WriteLine($"Failed to parse value: {valueStr}");
                    continue;
                }

                int charIndex = m.Index;
                int lineNumber = 1 + text.Take(charIndex).Count(c => c == '\n'); // 1-based line number
                string lineText = (lineNumber - 1 < lines.Length) ? lines[lineNumber - 1].Trim() : "";

                string scope = null;
                // find the last namespace/class/struct before this match
                Match scopeMatch = scopeRegex.Match(text, charIndex);
                if (scopeMatch.Success) scope = scopeMatch.Groups[1].Value;

                var foundInt = new FoundInt
                {
                    Value = value,
                    Line = lineNumber,
                    LineText = lineText,
                    Scope = scope,
                    CharIndex = charIndex,
                    FilePath = filePath
                };
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("[+] ");

                Console.ResetColor();
                Console.WriteLine($"Found: {foundInt}");
                results.Add(foundInt);
            }

            return results;
        }

        public static int SearchFileForInt(string filePath, string variableName, int occurrence = -1)
        {
            var found = FindIntsInFile(filePath, variableName);
            if (found == null || found.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("[-] ");

                Console.ResetColor();
                Console.WriteLine($"No results for '{variableName}' in {filePath}");
                return -1;
            }

            FoundInt chosen;
            if (occurrence == -1) chosen = found.Last();
            else if (occurrence >= 1 && occurrence <= found.Count) chosen = found[occurrence - 1];
            else chosen = found.First();

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("[*] ");

            Console.ResetColor();
            Console.WriteLine($"Returning value {chosen.Value} (0x{chosen.Value:X}) from {chosen.FilePath}:{chosen.Line}");


            return chosen.Value;
        }
    }
}
