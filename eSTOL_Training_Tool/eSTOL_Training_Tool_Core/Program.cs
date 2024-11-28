using System;
using eSTOL_Training_Tool;

namespace Bombathlon
{
    class Program
    {
        static void Main(string[] args)
        {
            // Influx.GetInstance().deletAll();



            Console.WriteLine(
                "┌─────────────────────┐\n" +
                "│ eSTOL Training Tool │\n" +
                "└─────────────────────┘\n");

            Controller controller = new Controller();
            controller.Init();
            controller.Run();
        }
    }
}


