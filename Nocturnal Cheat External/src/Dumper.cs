using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Noturnal_Cheat_External
{
    public static class Dumper
    {
        public static void Run(string fileType = "cs") // will outomatically set the dumped files to .cs files
        {
            // path
            string dumperPath = Path.Combine(AppContext.BaseDirectory, "Dumper", "cs2-dumper.exe");

            // output path
            string outputPath = Path.Combine(AppContext.BaseDirectory, "Dumper", "output");

            // Full path to the batch file
            string batPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Nocturnal Cheat.bat");
            string exePath = Path.Combine(AppContext.BaseDirectory, "CS2Cheats.exe");

            // Batch file content
            // string content =
            //$"@echo off\r\n" +
            //$"start \"\" \"{exePath}\" --enable-cheats\r\n"; <- remove these if you want a .bat shortcut on the desktop

            // Write the batch file
            // File.WriteAllText(batPath, content); <- remove this too if you want the shortcut

            if (Directory.Exists(outputPath))
            {
                Directory.Delete(outputPath, true);
            }

            var cs2Process = WaitForCS2(1);

            if (cs2Process == null)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("[!!!] ");
                Console.ResetColor();

                Console.WriteLine("CS2 is not open, please open before using this cheat");
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("CS2 is running! Starting program...");

            if (!File.Exists(dumperPath))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("[!!!] ");
                Console.ResetColor();

                Console.WriteLine($"Dumper not found: {dumperPath}");
                return;
            }

            string arguments = $"--file-types {fileType}";

            var psi = new ProcessStartInfo
            {
                FileName = dumperPath,
                Arguments = arguments,
                WorkingDirectory = Path.GetDirectoryName(dumperPath),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            try
            {
                using var proc = Process.Start(psi);
                if (proc == null)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("[!?] ");
                    Console.ResetColor();

                    Console.WriteLine("Failed to start process.");
                    return;
                }

                string stdout = proc.StandardOutput.ReadToEnd();
                string stderr = proc.StandardError.ReadToEnd();

                bool exited = proc.WaitForExit(15000);
                if (!exited)
                {
                    Console.WriteLine("Dumper did not exit within timeout. Killing process.");
                    try { proc.Kill(); } catch { /* ignore */ }
                }

                if (!string.IsNullOrWhiteSpace(stderr))
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("--- ERRORS ---\n" + stderr);
                    return;
                }

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"Dumper exited: {exited}, ExitCode: {proc.ExitCode}");
                Console.WriteLine("--- DUMPED ---\n");

                Console.ResetColor();
                Console.WriteLine((string.IsNullOrWhiteSpace(stdout) ? "<empty>" : stdout));

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("---- DONE ----\n");

                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while running dumper: " + ex);
            }
        }

        public static Process WaitForCS2(int timeoutSeconds = 30)
        {
            string processName = "cs2";
            int waited = 0;

            while (waited < timeoutSeconds)
            {
                var cs2Process = Process.GetProcessesByName(processName).FirstOrDefault();
                if (cs2Process != null && cs2Process.MainWindowHandle != IntPtr.Zero)
                {
                    return cs2Process;
                }

                Thread.Sleep(1000);
                waited++;
            }

            return null;
        }
    }
}
