using System;

namespace Suplanus.MuteToHue
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("MuteToHue started");
      try
      {
        MuteToHue muteToHue = new MuteToHue();
        muteToHue.StartAsync();
      }
      catch (Exception ex)
      {
        Console.WriteLine("Error: " + Environment.NewLine + ex);
      }
    }
  }
}
