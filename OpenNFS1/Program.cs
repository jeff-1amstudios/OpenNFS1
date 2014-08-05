#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#endregion

namespace OpenNFS1
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            using (var game = new Game1())
                game.Run();
        }

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			File.WriteAllText("exception.txt", e.ExceptionObject.ToString());
		}
    }
}
