using System;

namespace xvalidatr {
    static class ColorConsole {

        static readonly ConsoleColor errorColor = ConsoleColor.Red;
        static readonly ConsoleColor warningColor = ConsoleColor.Yellow;
        static readonly ConsoleColor successColor = ConsoleColor.Green;
        static readonly ConsoleColor actionColor = ConsoleColor.DarkGreen;
        static readonly ConsoleColor aboutColor = ConsoleColor.DarkGreen;
        static readonly ConsoleColor bright = ConsoleColor.White;

        private static String Center(string str) {
            return str.PadLeft((Console.WindowWidth + str.Length) / 2);
        }

        private static void PrintColor(string text, ConsoleColor color) {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        private static void writeColor(string text, ConsoleColor color) {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }

        public static void PrintError(string text) {
            PrintColor(text, errorColor);
        }

        public static void WriteError(string text) {
            writeColor(text, errorColor);
        }

        public static void PrintWarning(string text) {
            PrintColor(text, warningColor);
        }

        public static void WriteWarning(string text) {
            writeColor(text, warningColor);
        }

        public static void PrintSuccess(string text) {
            PrintColor(text, successColor);
        }

        public static void PrintAbout(string text) {
            PrintColor(Center(text), aboutColor);
        }

        public static void PrintAction(string text) {
            Console.WriteLine();
            PrintColor(text, actionColor);
        }

        public static void PrintBright(string text) {
            PrintColor(text, ConsoleColor.White);
        }

        public static void WriteBright(string text) {
            writeColor(text, bright);
        }

    }
}
