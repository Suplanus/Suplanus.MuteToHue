using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Suplanus.MuteToHue
{
    public class MuteToHue
    {
        const string QUOTE = "\"";

        public void Start()
        {
            Console.WriteLine("Monitoring mute state...");
            bool isMuted = GetMuteState(); // Starting state
            while (true)
            {
                try
                {
                    // Read state
                    bool newState = GetMuteState();
                    if (isMuted != newState)
                    {
                        Console.WriteLine("Muted: " + isMuted + " --> " + newState);
                        SetHueState(newState);
                        isMuted = newState; // All OK, set new state
                    }

                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + Environment.NewLine + ex);
                }
            }

            // ReSharper disable once FunctionNeverReturns
        }

        public string ReadFromBash(string readMuteCommand)
        {
            var escapedArgs = readMuteCommand.Replace("\"", "\\\"");
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c {QUOTE}{escapedArgs}{QUOTE}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }

        public bool GetMuteState()
        {
            var readMuteCommand = "osascript -e 'output muted of (get volume settings)'";
            string isMutedString = ReadFromBash(readMuteCommand);
            isMutedString = isMutedString.TrimEnd(Environment.NewLine.ToCharArray()).ToUpper();
            bool newState = isMutedString == "TRUE";
            return newState;
        }

        public void SetHueState(bool newState)
        {
            // Read python script and replace placeholder
            var filename = @"/Users/moz/Documents/GitHub/Suplanus.MuteToHue/src/ControlViaPhue.py"; // todo: relative path
            var tempFile = Path.Combine(Path.GetTempPath(), "MuteToHue.py");
            string content = File.ReadAllText(filename, Encoding.UTF8);
            string stateString = FirstCharToUpper(newState.ToString());
            content = content.Replace("$STATE$", stateString);
            File.WriteAllText(tempFile, content, Encoding.UTF8);

            // Execute python script
            var command = $"python {QUOTE}{tempFile}{QUOTE}";
            var result = ReadFromBash(command);
        }

        public static string FirstCharToUpper(string input)
        {
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
    }
}