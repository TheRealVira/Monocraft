#region License

// Copyright (c) 2016, Vira
// All rights reserved.
// Solution: Monocraft
// Project: Monocraft
// Filename: Program.cs
// Date - created: 2016.06.18 - 18:57
// Date - current: 2016.06.25 - 18:38

#endregion

#region Usings

using System;

#endregion

namespace Monocraft
{
#if WINDOWS || LINUX
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
#endif
}