using Octokit;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Mono.Options;
using System.IO;
using System.Linq;

namespace PaketUpgrader
{
    public enum PaketUpgrade
    {
        UpgradeNeeded,
        NothingToDo,
        RepositoryIsFork
    }

    class Options
    {
        public string Token { get; set; }
        public string[] Repositories { get; set; }
        public string Account { get; set; }
        public bool IncludeForks { get; internal set; }
        public bool SubmitPullRequests { get; internal set; }
    }

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
                Console.WriteLine("Specify either an input file to read, or an account to scan for repositories to inspect.");
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

            Run(o).Wait();

            Console.ReadKey();
        }

        static async Task Run(Options options)
        {
            var client = new GitHubClient(new ProductHeaderValue("paket-ugprade-scanner"))
            {
                Credentials = new Credentials(options.Token)
            };

            var upgrader = new PaketUpgrader(client);


            if (options.Repositories.Any())
            {
                foreach (var project in options.Repositories)
                {
                    await upgrader.Run(project, options.IncludeForks, options.SubmitPullRequests);
                }
            }
            else if (!string.IsNullOrWhiteSpace(options.Account))
            {
                var repos = await client.Repository.GetAllForUser(options.Account);

                var projects = repos.Select(r => r.FullName).ToArray();
                foreach (var project in projects)
                {
                    await upgrader.Run(project, options.IncludeForks, options.SubmitPullRequests);
                }
            }
            else
            {
                Console.WriteLine("No work specified, exiting...");
            }
        }
    }
}
