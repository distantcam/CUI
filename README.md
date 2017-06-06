# CUI
**CUI** provides a simple state based console UI with screens and a menu system for creating sample applications.

### Example code
```
using System;
using System.Threading.Tasks;
using CUI;

class Program
{
    enum Screens
    {
        MainScreen,
        Processes,
        Process1,
        Process2
    }

    static void Main(string[] args)
    {
        var app = new ConsoleApplication<Screens>(Screens.MainScreen);

        app.AddMenuScreen(Screens.MainScreen)
            .WithMenuOption('p', "Select Process to run", Screens.Processes)
            .WithMenuOption('q', "Quit");

        app.AddMenuScreen(Screens.Processes)
            .WithMenuOption("Task 1 (long)", Screens.Process1)
            .WithMenuOption("Task 2 (short)", Screens.Process2);

        app.AddFunctionScreen(Screens.Process1)
            .SetAction(async () =>
            {
                Console.WriteLine("Running...");
                await Task.Delay(1000);
                Console.WriteLine("Almost done...");
                await Task.Delay(1000);
                Console.WriteLine("Just finishing up...");
                await Task.Delay(1000);
                Console.WriteLine("Done!");
            });

        app.AddFunctionScreen(Screens.Process2)
            .SetAction(() =>
            {
                Console.WriteLine("I'm done already!");
                return Task.CompletedTask;
            });

        app.Run();
        // Or app.RunAsync().GetAwaiter().GetResult();
    }
}
```
