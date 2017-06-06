using System;
using System.Threading;
using System.Threading.Tasks;
using CUI;

class Program
{
    enum Screens
    {
        MainScreen,
        Processes,
        Process1,
        Process2,
        Process3
    }

    static void Main(string[] args)
    {
        var app = new ConsoleApplication<Screens>(Screens.MainScreen);

        app.AddMenuScreen(Screens.MainScreen)
            .WithMenuOption('p', "Select Process to run", Screens.Processes)
            .WithMenuOption('q', "Quit");

        app.AddMenuScreen(Screens.Processes)
            .WithMenuOption("Task 1 (long)", Screens.Process1)
            .WithMenuOption("Task 2 (short)", Screens.Process2)
            .WithMenuOption("Task 3 (switch context)", Screens.Process3);

        app.AddFunctionScreen(Screens.Process1)
            .SetAction(async () =>
            {
                Console.WriteLine($"[Thread: {Thread.CurrentThread.ManagedThreadId}] Running...");
                await Task.Delay(1000);
                Console.WriteLine($"[Thread: {Thread.CurrentThread.ManagedThreadId}] Almost done...");
                await Task.Delay(1000);
                Console.WriteLine($"[Thread: {Thread.CurrentThread.ManagedThreadId}] Just finishing up...");
                await Task.Delay(1000);
                Console.WriteLine($"[Thread: {Thread.CurrentThread.ManagedThreadId}] Done!");
            });

        app.AddFunctionScreen(Screens.Process2)
            .SetAction(() =>
            {
                Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] I'm done already!");
                return Task.CompletedTask;
            });

        app.AddFunctionScreen(Screens.Process3)
            .SetAction(async () =>
            {
                Console.WriteLine($"[Thread: {Thread.CurrentThread.ManagedThreadId}] Running...");
                await Task.Delay(1000).ConfigureAwait(false);
                Console.WriteLine($"[Thread: {Thread.CurrentThread.ManagedThreadId}] Almost done...");
                await Task.Delay(1000).ConfigureAwait(false);
                Console.WriteLine($"[Thread: {Thread.CurrentThread.ManagedThreadId}] Just finishing up...");
                await Task.Delay(1000).ConfigureAwait(false);
                Console.WriteLine($"[Thread: {Thread.CurrentThread.ManagedThreadId}] Done!");
            });

        app.Run();
    }
}