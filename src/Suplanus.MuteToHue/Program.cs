using System;
using System.Threading.Tasks;

namespace Suplanus.MuteToHue
{
  class Program
  {
    static async Task Main(string[] args)
    {
      Console.WriteLine("MuteToHue started");
      try
      {
        MuteToHue muteToHue = new MuteToHue();
        await muteToHue.StartAsync();
      }
      catch (Exception ex)
      {
        Console.WriteLine("Error: " + Environment.NewLine + ex);
      }
    }
  }
}
