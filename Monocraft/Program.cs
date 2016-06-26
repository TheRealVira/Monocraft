#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: Monocraft
// Project: Monocraft
// Filename: Program.cs
// Date - created: 2016.06.26 - 19:21
// Date - current: 2016.06.26 - 20:15

#endregion

#region Usings

using System;

#endregion

namespace Monocraft
{
    /// <summary>
    ///     The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new Game1())
                game.Run();
        }
    }
}