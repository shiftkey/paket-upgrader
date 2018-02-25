using Octokit;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace PaketUpgrader
{
    public class PaketUpgrader
    {
        private GitHubClient client;

        private string updatedPaketSha = "b98e000b232408fe0a21730e88f89755f0d7a68c";

        public PaketUpgrader(GitHubClient client)
        {
            this.client = client;
        }

        public async Task Run(Repository repository, bool submitPullRequest)
        {
            var owner = repository.Owner.Login;
            var name = repository.Name;

            var result = await Validate(repository);
            if (result == PaketUpgrade.PaketNotFound)
            {
                Console.WriteLine($"Skipping {repository.FullName} as Paket wasn't found");
                return;
            }

            if (result == PaketUpgrade.UpToDate)
            {
                Console.WriteLine($"Skipping {repository.FullName} as it appears to be up-to-date");
                return;
            }

            await PerformUpgrade(owner, name, submitPullRequest);
        }

        private async Task<PaketUpgrade> Validate(Repository repository)
        {
            var defaultBranch = repository.DefaultBranch;

            try
            {
                var contents = await client.Repository.Content.GetAllContentsByRef(repository.Id, ".paket", defaultBranch);
                var executables = contents.Where(c => c.Path.EndsWith(".exe"));
                foreach (var executable in executables)
                {
                    if (executable.Path.EndsWith("paket.exe") && executable.Sha != updatedPaketSha)
                    {
                        return PaketUpgrade.UpgradeNeeded;
                    }

                    if (executable.Path.EndsWith("paket.bootstrapper.exe") && executable.Sha != updatedPaketSha)
                    {
                        return PaketUpgrade.UpgradeNeeded;
                    }
                }
            }
            catch
            {
                return PaketUpgrade.PaketNotFound;
            }


            return PaketUpgrade.UpToDate;
        }

        async Task PerformUpgrade(string owner, string name, bool submitPullRequest)
        {
            var openPullRequest = await HasOpenPullRequest(owner, name);
            if (openPullRequest != null)
            {
                Console.WriteLine($"{owner}/{name} has an open pull request #{openPullRequest.Number}, skipping...");
                return;
            }

            if (!submitPullRequest)
            {
                Console.WriteLine($"Skipping {owner}/{name}, pass --submit-pull-request to generate the pull request.");
                return;
            }

            Console.WriteLine($"Submitting a pull request to {owner}/{name}...");

            var repository = await FindRepositoryToSubmitPullRequestFrom(owner, name);
            if (repository == null)
            {
                Console.WriteLine($"Couldn't find a repository to use for upgrading. WTF?");
                return;
            }

            var reference = await CreateNewReferenceWithPatch(repository);
            if (reference != null)
            {
                var branch = reference.Ref.Replace("refs/heads/", "");

                var headRef = repository.Fork ? $"{repository.Owner.Login}:{branch}" : branch;

                var newPullRequest = new NewPullRequest("Update paket to address TLS deprecation", headRef, repository.DefaultBranch);
                newPullRequest.Body = @":wave: GitHub disabled TLS 1.0 and TLS 1.1 on February 22nd, which affected Paket.

You can read more about this on the [GitHub Engineering blog](https://githubengineering.com/crypto-removal-notice/).

The update to Paket is explained here: https://github.com/fsprojects/Paket/pull/3066

The work to update Paket in the wild is occurring here: https://github.com/fsprojects/Paket/issues/3068";

                var pullRequest = await client.PullRequest.Create(owner, name, newPullRequest);
            }

        }

        async Task<PullRequest> HasOpenPullRequest(string owner, string name)
        {
            var pullRequests = await client.PullRequest.GetAllForRepository(owner, name, new ApiOptions() { PageSize = 100 });

            foreach (var pullRequest in pullRequests)
            {
                var files = await client.PullRequest.Files(owner, name, pullRequest.Number);
                var updatesPaketToLatestVersion = files.FirstOrDefault(f =>
                    f.FileName == ".paket/paket.exe" && f.Sha == updatedPaketSha || f.FileName == ".paket/paket.bootstrapper.exe" && f.Sha == updatedPaketSha);

                if (updatesPaketToLatestVersion != null)
                {
                    return pullRequest;
                }
            }

            return null;
        }

        async Task<Repository> FindRepositoryToSubmitPullRequestFrom(string owner, string name)
        {
            var repository = await client.Repository.Get(owner, name);
            if (repository.Permissions.Push)
            {
                return repository;
            }

            var request = new RepositoryRequest()
            {
                Type = RepositoryType.Owner
            };

            var ownedRepos = await client.Repository.GetAllForCurrent(request, new ApiOptions() { PageSize = 100 });
            var forks = ownedRepos.Where(r => r.Fork);
            var matchingNames = forks.Where(r => r.Name == name);
            var foundFork = matchingNames.FirstOrDefault();

            // TODO: foundFork.Parent is null, and that might be a problem down the track

            if (foundFork != null)
            {
                return foundFork;
            }

            return await client.Repository.Forks.Create(owner, name, new NewRepositoryFork());
        }

        async Task<string> GetNewExecutableBase64()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames();
            var resourceName = resourceNames[0];

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var bytes = memoryStream.ToArray();
                return Convert.ToBase64String(bytes);
            }
        }

        async Task<Reference> CreateNewReferenceWithPatch(Repository repository)
        {
            // first, create a new blob in the repository which is the new file contents
            var blob = new NewBlob()
            {
                Content = await GetNewExecutableBase64(),
                Encoding = EncodingType.Base64
            };
            var newBlob = await client.Git.Blob.Create(repository.Id, blob);

            // we create the new reference for the PR branch
            var defaultRef = $"heads/{repository.DefaultBranch}";
            var defaultBranch = await client.Git.Reference.Get(repository.Id, defaultRef);
            var initialSha = defaultBranch.Object.Sha;

            var newRef = $"heads/bootstrapper";
            var newReference = await client.Git.Reference.Create(repository.Id, new NewReference(newRef, initialSha));

            var currentTree = await client.Git.Tree.Get(repository.Id, initialSha);

            // update the paket subdirectory to assign the new blob to whatever executable
            var paketTreeNode = currentTree.Tree.FirstOrDefault(t => t.Path == ".paket");
            var paketTree = await client.Git.Tree.Get(repository.Id, paketTreeNode.Sha);

            var executables = paketTree.Tree.Where(t => t.Path.EndsWith(".exe"));

            if (executables.Count() == 0)
            {
                Console.WriteLine($"TODO: oh gosh, we're not able to find executables in the .paket directory");
                return null;
            }

            var executable = executables.ElementAt(0);

            var newPaketTree = new NewTree
            {
                BaseTree = paketTree.Sha,
            };
            newPaketTree.Tree.Add(new NewTreeItem()
            {
                Mode = executable.Mode,
                Sha = newBlob.Sha,
                Path = executable.Path,
                Type = executable.Type.Value
            });

            var updatedPaketTree = await client.Git.Tree.Create(repository.Id, newPaketTree);

            // update the root tree to use this new .paket directory
            var newRootTree = new NewTree
            {
                BaseTree = currentTree.Sha
            };
            newRootTree.Tree.Add(new NewTreeItem()
            {
                Mode = paketTreeNode.Mode,
                Sha = updatedPaketTree.Sha,
                Path = paketTreeNode.Path,
                Type = paketTreeNode.Type.Value
            });

            var updatedRootTree = await client.Git.Tree.Create(repository.Id, newRootTree);

            // create a new commit using the updated tree
            var newCommit = new NewCommit($"Updated {executable.Path} to address TLS 1.0 and 1.1 deprecation", updatedRootTree.Sha, initialSha);
            var commit = await client.Git.Commit.Create(repository.Id, newCommit);

            // then update the bootstrapper ref to this new commit
            var updatedReference = await client.Git.Reference.Update(repository.Id, newRef, new ReferenceUpdate(commit.Sha));

            return updatedReference;
        }
    }
}
