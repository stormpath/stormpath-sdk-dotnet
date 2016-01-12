// <copyright file="Helpers.cs" company="Stormpath, Inc.">
// Copyright (c) 2016 Stormpath, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using System.Threading;

namespace Stormpath.Demo
{
    public static class Helpers
    {
        public static bool SpacebarToContinue(CancellationToken cancelToken)
        {
            if (cancelToken.IsCancellationRequested)
            {
                return false;
            }

            Console.Write($"{Strings.NL}Press spacebar to continue");
            var key = Console.ReadKey(true);

            if (cancelToken.IsCancellationRequested)
            {
                return false;
            }

            ClearCurrentLine(Console.CursorTop);
            Console.SetCursorPosition(0, Console.CursorTop - 1);

            return key.KeyChar == ' ';
        }

        public static string TrimWithEllipse(string input, int maxLength)
        {
            if (input.Length <= maxLength)
            {
                return input;
            }

            return input.Substring(0, maxLength - 3) + "...";
        }

        public static void ClearCurrentLine(int line)
        {
            Console.SetCursorPosition(0, line);
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write(" ");
            }

            Console.SetCursorPosition(0, line);
        }
    }
}
