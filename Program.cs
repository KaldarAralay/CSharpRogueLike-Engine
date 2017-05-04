using System;
using System.Media;
using Console = SadConsole.Consoles.Console;

namespace RogueSharpSadConsoleSamples
{
   public static class Program
   {
      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      public static void Main(string[] args)
      {
           

            using ( var game = new RogueGame() )
            game.Run();
      }
   }
}
