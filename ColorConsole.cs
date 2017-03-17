using System;

namespace xvalidatr {
    static class ColorConsole {

        static ConsoleColor errorColor = ConsoleColor.Red;
        static ConsoleColor warningColor = ConsoleColor.Yellow;
        static ConsoleColor successColor = ConsoleColor.Green;
        static ConsoleColor actionColor = ConsoleColor.DarkGreen;
        static ConsoleColor aboutColor = ConsoleColor.DarkGreen;
        static ConsoleColor bright = ConsoleColor.White;

        private static String Center(string str) {
            return str.PadLeft((Console.WindowWidth + str.Length) / 2);
        }

        private static void printColor(string text, ConsoleColor color) {
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
            printColor(text, errorColor);
        }

        public static void WriteError(string text) {
            writeColor(text, errorColor);
        }

        public static void PrintWarning(string text) {
            printColor(text, warningColor);
        }

        public static void WriteWarning(string text) {
            writeColor(text, warningColor);
        }

        public static void PrintSuccess(string text) {
            printColor(text, successColor);
        }

        public static void PrintAbout(string text) {
            printColor(Center(text), aboutColor);
        }

        public static void PrintAction(string text) {
            Console.WriteLine();
            printColor(text, actionColor);
        }

        public static void PrintBright(string text) {
            printColor(text, ConsoleColor.White);
        }

        public static void WriteBright(string text) {
            writeColor(text, bright);
        }

    }
}
