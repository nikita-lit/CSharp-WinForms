namespace WinForms
{
    internal static class Program
    {
        public static string[] Args { get; private set; }

        /// <summary>
        ///  The main entry posint for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Args = args;

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MathQuiz());
        }

        public static string GetDirectory() => AppDomain.CurrentDomain.BaseDirectory;
    }
}