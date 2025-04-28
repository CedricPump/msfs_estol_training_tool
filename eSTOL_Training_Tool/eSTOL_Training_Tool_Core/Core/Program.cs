using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using eSTOL_Training_Tool_Core.Core;
using eSTOL_Training_Tool_Core.UI;

namespace Bombathlon
{
    static class Program
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
            Task controllerTask = Task.Run(() =>
            {
                controller.Run(); // Run the loop in the background
            });

            ApplicationConfiguration.Initialize();
            var form = new FormUI(controller);
            controller.SetUI(form);
            Application.Run(form);
        }
    }
}


