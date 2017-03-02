using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Console;
using static System.ConsoleColor;

namespace CUI
{
    public class ConsoleApplication<TScreen>
    {
        Dictionary<TScreen, IScreen<TScreen>> screens = new Dictionary<TScreen, IScreen<TScreen>>();
        TScreen startScreen;

        public ConsoleApplication(TScreen startScreen)
        {
            this.startScreen = startScreen;
        }

        public MenuScreen<TScreen> AddMenuScreen(TScreen screen)
        {
            var screenConfig = new MenuScreen<TScreen>(screen);
            screens.Add(screen, screenConfig);
            return screenConfig;
        }

        public FunctionScreen<TScreen> AddFunctionScreen(TScreen screen)
        {
            var screenConfig = new FunctionScreen<TScreen>(screen, startScreen);
            screens.Add(screen, screenConfig);
            return screenConfig;
        }

        public async Task Run()
        {
            var currentScreen = startScreen;

            await screens[currentScreen].Display();

            while (true)
            {
                var key = ReadKey(true);

                TScreen newScreen;
                if (screens[currentScreen].TryUpdate(key, out newScreen))
                {
                    currentScreen = newScreen;
                }
                else
                {
                    break;
                }

                await screens[currentScreen].Display();
            }
        }
    }

    public class MenuScreen<TScreen> : IScreen<TScreen>
    {
        TScreen screen;
        List<MenuItem<TScreen>> menuItems = new List<MenuItem<TScreen>>();
        int selectedItem = 0;
        int xpadding = 2;
        int ypadding = 2;

        public MenuScreen(TScreen screen)
        {
            this.screen = screen;
        }

        public MenuScreen<TScreen> WithMenuOption(string text, TScreen screen)
        {
            menuItems.Add(new MenuItem<TScreen> { Text = text, Screen = screen });
            return this;
        }

        public MenuScreen<TScreen> WithMenuOption(string text)
        {
            menuItems.Add(new MenuItem<TScreen> { Text = text });
            return this;
        }

        public MenuScreen<TScreen> WithMenuOption(char key, string text, TScreen screen)
        {
            menuItems.Add(new MenuItem<TScreen> { Key = key, Text = text, Screen = screen });
            return this;
        }

        public MenuScreen<TScreen> WithMenuOption(char key, string text)
        {
            menuItems.Add(new MenuItem<TScreen> { Key = key, Text = text });
            return this;
        }

        public MenuScreen<TScreen> SetPadding(int x, int y)
        {
            xpadding = x;
            ypadding = y;
            return this;
        }

        public Task Display()
        {
            Clear();
            for (int i = 0; i < menuItems.Count; i++)
            {
                var isSelected = selectedItem == i;
                SetCursorPosition(xpadding, i + ypadding);

                if (isSelected)
                    using (new ConsoleColorReset())
                    {
                        ConsoleUtil.SetColors(Black, White);
                        Write(">");
                    }
                else
                    Write(" ");
                Write($" {menuItems[i].Key} {menuItems[i].Text}");
            }
            SetCursorPosition(0, WindowHeight);
            return Task.CompletedTask;
        }

        public bool TryUpdate(ConsoleKeyInfo key, out TScreen screen)
        {
            screen = this.screen;
            if (selectedItem < menuItems.Count - 1 && key.Key == ConsoleKey.DownArrow)
            {
                selectedItem++;
                return true;
            }
            if (selectedItem > 0 && key.Key == ConsoleKey.UpArrow)
            {
                selectedItem--;
                return true;
            }
            if (key.Key == ConsoleKey.Enter)
            {
                if (!menuItems[selectedItem].IsTermination)
                    screen = menuItems[selectedItem].Screen;
                return !menuItems[selectedItem].IsTermination;
            }
            if (key.KeyChar != 0)
            {
                var menuItem = menuItems.FirstOrDefault(mi => mi.Key == key.KeyChar);
                if (menuItem != null)
                {
                    if (!menuItem.IsTermination)
                        screen = menuItem.Screen;
                    return !menuItem.IsTermination;
                }
            }
            return true;
        }
    }

    public class FunctionScreen<TScreen> : IScreen<TScreen>
    {
        TScreen screen;
        Func<Task> action;
        TScreen nextScreen;

        public FunctionScreen(TScreen screen, TScreen nextScreen)
        {
            this.screen = screen;
            this.nextScreen = nextScreen;
        }

        public FunctionScreen<TScreen> SetAction(Func<Task> action)
        {
            this.action = action;
            return this;
        }

        public FunctionScreen<TScreen> SetNextScreen(TScreen nextScreen)
        {
            this.nextScreen = nextScreen;
            return this;
        }

        public async Task Display()
        {
            Clear();
            if (action != null)
                await action();
            else
                using (new ConsoleColorReset())
                {
                    ConsoleUtil.SetColors(White, Red);
                    WriteLine($"ERROR: Action not set for screen '{screen}'.");
                }
            WriteLine();
            WriteLine("Press any key to continue");
        }

        public bool TryUpdate(ConsoleKeyInfo key, out TScreen screen)
        {
            screen = nextScreen;
            return true;
        }
    }

    class MenuItem<TScreen>
    {
        TScreen screen;

        public MenuItem()
        {
            IsTermination = true;
        }

        public bool IsTermination { get; private set; }
        public char Key { get; set; }
        public string Text { get; set; }

        public TScreen Screen
        {
            get { return screen; }
            set
            {
                screen = value;
                IsTermination = false;
            }
        }
    }

    interface IScreen<TScreen>
    {
        Task Display();

        bool TryUpdate(ConsoleKeyInfo key, out TScreen screen);
    }

    static class ConsoleUtil
    {
        public static void SetColors(ConsoleColor foreground, ConsoleColor background)
        {
            ForegroundColor = foreground;
            BackgroundColor = background;
        }
    }

    class ConsoleColorReset : IDisposable
    {
        ConsoleColor background;
        ConsoleColor foreground;

        public ConsoleColorReset()
        {
            background = BackgroundColor;
            foreground = ForegroundColor;
        }

        public void Dispose()
        {
            ConsoleUtil.SetColors(foreground, background);
        }
    }
}