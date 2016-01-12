// <copyright file="Program.cs" company="Stormpath, Inc.">
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
using System.Threading.Tasks;

namespace Stormpath.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Wire up the console cancel event (Ctrl+C) to cancel async tasks
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (source, evt) =>
            {
                evt.Cancel = true;
                cts.Cancel();
            };

            Console.WriteLine("Welcome to the Stormpath.SDK demo suite!");

            while (true)
            {
                Console.WriteLine($"Available commands:{Strings.NL}");
                Console.WriteLine("{0,-10} {1}", "basic", "Simple account creation and login");
                Console.WriteLine("{0,-10} {1}", "email", "Email verification");
                Console.WriteLine("{0,-10} {1}", "quit", "Exit the demo");
                Console.Write($"{Strings.NL}> ");

                var input = Console.ReadLine();
                Thread.Sleep(50); // let the CancellationToken "catch up"

                if (cts.Token.IsCancellationRequested)
                {
                    return;
                }

                if (input.Equals("quit", StringComparison.CurrentCultureIgnoreCase))
                {
                    return;
                }

                switch (input)
                {
                    case "basic":
                        RunAsync<BasicDemo>(cts.Token).GetAwaiter().GetResult();
                        break;
                    case "email":
                        RunAsync<EmailVerificationDemo>(cts.Token).GetAwaiter().GetResult();
                        break;
                    default:
                        Console.WriteLine($"Unknown command '{input}'");
                        break;
                }
            }
        }

        public static async Task RunAsync<T>(CancellationToken cancellationToken)
            where T : class, IDemo, new()
        {
            var demo = new T();

            try
            {
                await demo.RunAsync(cancellationToken);
                Console.WriteLine("Demo finished.");
            }
            finally
            {
                Console.WriteLine("Cleaning up...");
                await demo.CleanupAsync();
                Console.WriteLine("Cleaned up.");
            }
        }
    }
}
