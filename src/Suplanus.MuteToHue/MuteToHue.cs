using System;
using System.Threading.Tasks;
using Q42.HueApi;
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
    }

    public async Task InitHueAsync()
    {
      Console.WriteLine("Get bridges...");
      IBridgeLocator locator = new HttpBridgeLocator(); 
      var bridgeIPs = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5)); // todo: BOOM
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
  }
}