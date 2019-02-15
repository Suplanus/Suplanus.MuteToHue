using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
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
      Console.WriteLine("Init Hue...");
      await InitHueAsync();

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
            await SetHueStateAsync(newState);
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

    public async Task InitHueAsync()
    {
      Console.WriteLine("Get bridges...");
      IBridgeLocator locator = new HttpBridgeLocator();
      var bridgeIPs = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));
      foreach (var b in bridgeIPs)
      {
        _bridge = b;
        break;
      }

      Console.WriteLine("Get Hue app key...");
      _client = new LocalHueClient(_bridge.IpAddress);
      var appKey = await _client.RegisterAsync("MuteToHue", Environment.MachineName);

      Console.WriteLine("Register Hue app...");
      _client.Initialize(appKey);
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

    public async Task SetHueStateAsync(bool newState)
    {
      var lightCommand = new LightCommand();
      lightCommand.On = newState;

      var color = new RGBColor("FF0000");
      lightCommand.SetColor(color);

      IEnumerable<Light> lights = await _client.GetLightsAsync();
      foreach (var light in lights)
      {
        // client.SendCommandAsync(lightCommand, lightName);
      }
    }
  }
}