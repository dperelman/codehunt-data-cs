using System;
using System.IO;
using System.Collections.Generic;

using codehunt.datarelease;
using codehunt.rest;

namespace codehunt.example
{
    class Program
    {
        public static void Main(string[] args)
        {
            // In order to get access to the Code Hunt REST API, please
            //   request a client_id and client_secret from codehunt@microsoft.com
            string clientId = null;
            string clientSecret = null;

            CodeHuntClient client;
            if (clientId != null && clientSecret != null)
            {
                client = new CodeHuntClient(clientId, clientSecret);
            }
            else
            {
                client = null;
            }

            Data data = new codehunt.datarelease.Data(new DirectoryInfo(
                                Path.Combine("..", "..", "..", "..", "..", @"Code Hunt data release 1")));

            foreach (Level level in data.Levels)
            {
                // The friendly name for the level used in the data release
                Console.Out.WriteLine(level);
                // The Code Hunt API name for the level
                Console.Out.WriteLine(level.ChallengeId);
                // The reference solution for the level
                Console.Out.WriteLine(level.ChallengeText);
                try
                {
                    // Use Microsoft.CodeAnalysis ("Roslyn") to parse challenge code and remove its comments/extra whitespace.
                    Console.Out.WriteLine(level.Parse().GetRoot().RemoveComments());
                }
                catch (Exception e)
                {
                    // Microsoft.CodeAnalysis doesn't seem to work on Mono
                    Console.Error.WriteLine(e);
                }
            }

            foreach (User user in data.Users)
            {
                Console.Out.WriteLine(user);
                // user-reported experience level, 1-3:
                //   1="Beginner", 2="Intermediate", 3="Advanced"
                Console.Out.WriteLine(user.Experience);
                foreach (Level level in data.Levels)
                {
                    IEnumerable<Attempt> attempts = user.EnumerateAttemptsFor(level);
                    // attempts will be null if the user did not attempt this level
                    if (attempts != null)
                    {
                        foreach (Attempt attempt in attempts)
                        {
                            Console.Out.WriteLine(attempt.AttemptFile);
                            Console.Out.WriteLine(attempt.AttemptNum);
                            Console.Out.WriteLine(attempt.Won);
                            Console.Out.WriteLine(attempt.Rating);
                            Console.Out.WriteLine(attempt.Timestamp);
                            Console.Out.WriteLine(attempt.Language);
                            Console.Out.WriteLine(attempt.Text);

                            // Get a client_id/client_secret from codehunt@microsoft.com
                            //   to query the Code Hunt REST API.
                            if (client != null)
                            {
                                if (attempt.Language == Language.Java)
                                {
                                    // Translate from Java to C#
                                    Translation t = client.translate(attempt);
                                    Console.Out.WriteLine(t);

                                    if (t.Success)
                                    {
                                        Console.Out.WriteLine(t.Text);
                                    }
                                    else
                                    {
                                        Console.Out.WriteLine(t.Errors);
                                    }
                                }
                                // Perform "exploration": this is what the "Capture Code"
                                //   button does in the game. The response will be an error
                                //   or a set of test cases.
                                Exploration exp = client.explore(attempt);
                                Console.Out.WriteLine(exp);
                                if (attempt.Won != exp.HasWon)
                                {
                                    Console.Out.WriteLine("This shouldn't happen.");
                                }
                                if (exp.AttemptCompiles)
                                {
                                    foreach (TestCase testCase in exp.TestCases)
                                    {
                                        Console.Out.WriteLine(testCase);
                                    }
                                }
                                else
                                {
                                    // kind of failure
                                    Console.Out.WriteLine(exp.Kind);
                                    // error messages
                                    foreach (var error in exp.Errors)
                                    {
                                        Console.Out.WriteLine(error);
                                    }
                                    if (exp.Kind == "CompilationError")
                                    {
                                        // compilation error messages are more structured,
                                        //   they are also summarized in exp.errors
                                        foreach (var error in exp.CompilationErrors)
                                        {
                                            Console.Out.WriteLine(error);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
