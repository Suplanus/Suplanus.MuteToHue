using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Q42.HueApi;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.ColorConverters.Original;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;

namespace Suplanus.MuteToHue
{
    public class MuteToHue
    {
        LocatedBridge _bridge;
        ILocalHueClient _client;

        public async void StartAsync()
        {
            Console.WriteLine("Monitoring...");
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
                    Arguments = $"-c \"{escapedArgs}\"",
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
            var engine = Python.CreateEngine(); // Extract Python language engine from their grasp
            var scope = engine.CreateScope();
            var filename = @"/Users/moz/Documents/GitHub/Suplanus.MuteToHue/src/ControlViaPhue.py";

            //string content = File.ReadAllText(filename, Encoding.UTF8);
            //string stateString = FirstCharToUpper(newState.ToString());
            //content = content.Replace("$STATE$", stateString);
            //ScriptSource source = engine.CreateScriptSourceFromString(content,Microsoft.Scripting.SourceCodeKind.File); // Load the script
            //object result = source.Execute();


            var tempFile = Path.Combine(Path.GetTempPath(), "MuteToHue.py");
            string content = File.ReadAllText(filename, Encoding.UTF8);
            string stateString = FirstCharToUpper(newState.ToString());
            content = content.Replace("$STATE$", stateString);
            File.WriteAllText(tempFile, content, Encoding.UTF8);
            var quote = "\"";
            var command = $"python {quote}{tempFile}{quote}";
            var result = ReadFromBash(command);

        }

        public static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
                throw new ArgumentException("ARGH!");
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
    }
}