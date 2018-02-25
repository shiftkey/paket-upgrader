using Octokit;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Mono.Options;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace PaketUpgrader
{

    class Program
    {
        static string[] TryParseFile(string relativePath)
        {
            var path = Path.GetFullPath(relativePath);
            if (!File.Exists(path))
            {
                throw new OptionException($"File at path '{path}' does not exist", "inputFile");
            }

            return File.ReadAllLines(path);
        }

        static void EmitHelpMessage(OptionSet options)
        {
            Console.WriteLine("Usage: paket-upgrader.exe [OPTIONS]");
            Console.WriteLine("Scan a number of GitHub repositories to determine whether they are running an old version of Paket.");
            Console.WriteLine("Specify an input file to read, or an account to scan for repositories to inspect.");
            Console.WriteLine();

            Console.WriteLine("Options:");
            options.WriteOptionDescriptions(Console.Out);
            return;
        }

        static void Main(string[] args)
        {
            var shouldShowHelp = false;

            var o = new Options();

            var options = new OptionSet {
                { "t|token=", "a personal access token with public_repo scope to be used to interact with the GitHub API", n => o.Token = n },
                { "f|file=", "a file containing a list of 'owner/repo' values, one on each line", n => o.Repositories = TryParseFile(n) },
                { "u|user=", "a user or organization on GitHub to scan for problem repositories", n => o.Account = n },
                { "include-forks", "also include forked repositories when scanning for repositories to update", (bool n) => o.IncludeForks = true },
                { "submit-pull-request", "actually submit a PR to each of the repositories which are using an old version of Paket", (bool n) => o.SubmitPullRequests = true },
                { "h|help", "show this message and exit", h => shouldShowHelp = h != null },
            };

            List<string> extra;
            try
            {
                extra = options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("paket-upgrader: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `paket-upgrader --help' for more information.");
                return;
            }

            if (shouldShowHelp)
            {
                // show some app description message
                Console.WriteLine("Usage: paket-upgrader.exe [OPTIONS]");
                Console.WriteLine("Scan a number of GitHub repositories to determine whether they are running an old version of Paket.");
                Console.WriteLine("Specify an input file to read, or an account to scan for repositories to inspect.");
                Console.WriteLine();

                // output the options
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            if (string.IsNullOrWhiteSpace(o.Token))
            {
                o.Token = Environment.GetEnvironmentVariable("GITHUB_ACCESS_TOKEN");
            }

            if (string.IsNullOrWhiteSpace(o.Token))
            {
                Console.WriteLine("You must provide a 'token' argument or set the GITHUB_ACCESS_TOKEN environment variable to use this program");
                Environment.Exit(-1);
                return;
            }

            var client = new GitHubClient(new ProductHeaderValue("paket-ugprade-scanner"))
            {
                Credentials = new Credentials(o.Token)
            };

            var upgrader = new PaketUpgrader(client);

            if (o.Repositories.Any())
            {
                Console.WriteLine($"Found {o.Repositories.Length} entries in file");
                Console.WriteLine();

                try
                {
                    var lookup = o.Repositories.Select(async r =>
                    {
                        var parts = r.Split('/');
                        var owner = parts[0];
                        var name = parts[1];
                        try
                        {
                            return await client.Repository.Get(owner, name);
                        }
                        catch
                        {
#if DEBUG
                            if (!Debugger.IsAttached)
                            {
                                Debugger.Launch();
                            }
#endif
                            return null;
                        }
                    });

                    var allRepos = Task.WhenAll(lookup).Result;

                    var repos = allRepos
                        .Where(r => r != null)
                        .Select(r =>
                        {
                            if (!r.Fork || o.IncludeForks)
                            {
                                return r;
                            }
                            else
                            {
                                return null;
                            }
                        })
                        .Where(r => r != null);

                    var tasks = repos.Select(r => upgrader.Run(r, o.SubmitPullRequests)).ToArray();

                    Task.WaitAll(tasks);

                }
                catch (Exception ex)
                {
#if DEBUG
                    if (!Debugger.IsAttached)
                    {
                        Debugger.Launch();
                    }
#else
                    throw ex;
#endif
                }
            }
            else if (!string.IsNullOrWhiteSpace(o.Account))
            {
                var user = client.User.Get(o.Account).Result;
                var type = user.Type == AccountType.User ? "user" : "organization";

                var allRepos = client.Repository.GetAllForUser(o.Account, new ApiOptions { PageSize = 100 }).Result;


                var repos = allRepos.Select(r =>
                {
                    if (!r.Fork || o.IncludeForks)
                    {
                        return r;
                    }
                    else
                    {
                        return null;
                    }
                }).Where(r => r != null).ToArray();

                Console.WriteLine($"Found {repos.Length} repositories under the {o.Account} {type}");
                Console.WriteLine();

                var tasks = repos.Select(r => upgrader.Run(r, o.SubmitPullRequests)).ToArray();

                Task.WaitAll(tasks);
            }
            else
            {
                EmitHelpMessage(options);
            }
        }
    }
}
