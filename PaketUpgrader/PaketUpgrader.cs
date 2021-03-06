﻿using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PaketUpgrader
{


    public class PaketUpgrader
    {
        private GitHubClient client;

        private List<string> supportedPaketVersions = new List<string>
        {
            "b98e000b232408fe0a21730e88f89755f0d7a68c", // 5.142 official release
            "2b166849ff7703ca2569b2279bd2b6309ab61065", // reported good by @BlythMeister
            "cdd55e9c248b33768917cb5081606f461483a78c" // reported good by @BlythMeister
        };

        public PaketUpgrader(GitHubClient client)
        {
            this.client = client;
        }

        public async Task Run(Repository repository, bool submitPullRequest, bool debug = false)
        {
            var owner = repository.Owner.Login;
            var name = repository.Name;

            var result = await Validate(repository);
            if (result == PaketUpgrade.PaketNotFound)
            {
                if (debug)
                {
                    Console.WriteLine($"Ignoring {repository.FullName} as Paket wasn't found in the repository");
                }
                return;
            }

            if (result == PaketUpgrade.UpToDate)
            {
                Console.WriteLine($"{repository.FullName} appears to be up-to-date");
                return;
            }

            await PerformUpgrade(owner, name, submitPullRequest);
        }

        private async Task<PaketUpgrade> Validate(Repository repository)
        {
            try
            {
                var contents = await Wrap.RateLimiting(client, c => c.Repository.Content.GetAllContentsByRef(repository.Id, ".paket", repository.DefaultBranch));
                var executables = contents.Where(c => c.Path.EndsWith(".exe"));
                foreach (var executable in executables)
                {
                    if (executable.Path.EndsWith("paket.exe") && !supportedPaketVersions.Contains(executable.Sha))
                    {
                        return PaketUpgrade.UpgradeNeeded;
                    }

                    if (executable.Path.EndsWith("paket.bootstrapper.exe") && !supportedPaketVersions.Contains(executable.Sha))
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
                Console.WriteLine($"{owner}/{name} has an open pull request #{openPullRequest.Number}");
                return;
            }

            if (!submitPullRequest)
            {
                Console.WriteLine($"{owner}/{name} requires update, but ignored");
                return;
            }

            var repository = await FindRepositoryToSubmitPullRequestFrom(owner, name);
            if (repository == null)
            {
                Console.WriteLine($" - Couldn't find a repository to use for upgrading {owner}/{name}");
                return;
            }

            var reference = await CreateNewReferenceWithPatch(repository);
            if (reference == null)
            {
                Console.WriteLine($" - Unable to create a new reference in th erepository {repository.FullName}");
                return;
            }

            var pullRequest = await CreatePullRequest(owner, name, repository, reference);
            Console.WriteLine($" - {owner}/{name} now has PR {pullRequest.Number} opened");
        }

        private async Task<PullRequest> CreatePullRequest(string owner, string name, Repository repository, Reference reference)
        {
            var branch = reference.Ref.Replace("refs/heads/", "");

            var headRef = repository.Fork ? $"{repository.Owner.Login}:{branch}" : branch;

            var newPullRequest = new NewPullRequest("Update paket to address TLS deprecation", headRef, repository.DefaultBranch);
            newPullRequest.Body = @":wave: GitHub disabled TLS 1.0 and TLS 1.1 on February 22nd, which affected Paket and needs to be updated to 5.142 or later.

You can read more about this on the [GitHub Engineering blog](https://githubengineering.com/crypto-removal-notice/).

The update to Paket is explained here: https://github.com/fsprojects/Paket/pull/3066

The work to update Paket in the wild is occurring here: https://github.com/fsprojects/Paket/issues/3068";

            var pullRequest = await Wrap.RateLimiting(client, c => c.PullRequest.Create(owner, name, newPullRequest));
            return pullRequest;
        }

        async Task<PullRequest> HasOpenPullRequest(string owner, string name)
        {
            var pullRequests = await Wrap.RateLimiting(client, c => c.PullRequest.GetAllForRepository(owner, name, new ApiOptions() { PageSize = 100 }));

            foreach (var pullRequest in pullRequests)
            {
                var files = await Wrap.RateLimiting(client, c => c.PullRequest.Files(owner, name, pullRequest.Number));
                var updatesPaketToLatestVersion = files.FirstOrDefault(f =>
                    f.FileName == ".paket/paket.exe" && supportedPaketVersions.Contains(f.Sha)
                    || f.FileName == ".paket/paket.bootstrapper.exe" && supportedPaketVersions.Contains(f.Sha));

                if (updatesPaketToLatestVersion != null)
                {
                    return pullRequest;
                }
            }

            return null;
        }

        async Task<Repository> FindRepositoryToSubmitPullRequestFrom(string owner, string name)
        {
            var repository = await Wrap.RateLimiting(client, c => c.Repository.Get(owner, name));
            if (repository.Permissions.Push)
            {
                return repository;
            }

            var request = new RepositoryRequest()
            {
                Type = RepositoryType.Owner
            };

            var ownedRepos = await Wrap.RateLimiting(client, c => c.Repository.GetAllForCurrent(request, new ApiOptions() { PageSize = 100 }));
            var forks = ownedRepos.Where(r => r.Fork);
            var matchingNames = forks.Where(r => r.Name == name);
            var foundFork = matchingNames.FirstOrDefault();

            // TODO: foundFork.Parent is null, and that might be a problem down the track

            if (foundFork != null)
            {
                return foundFork;
            }

            return await Wrap.RateLimiting(client, c => c.Repository.Forks.Create(owner, name, new NewRepositoryFork()));
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
            var newBlob = await Wrap.RateLimiting(client, c => c.Git.Blob.Create(repository.Id, blob));

            // we create the new reference for the PR branch
            var defaultRef = $"heads/{repository.DefaultBranch}";
            var defaultBranch = await Wrap.RateLimiting(client, c => c.Git.Reference.Get(repository.Id, defaultRef));
            var initialSha = defaultBranch.Object.Sha;

            var newRef = $"heads/bootstrapper";
            var newReference = await Wrap.RateLimiting(client, c => c.Git.Reference.Create(repository.Id, new NewReference(newRef, initialSha)));

            var currentTree = await Wrap.RateLimiting(client, c => c.Git.Tree.Get(repository.Id, initialSha));

            // update the paket subdirectory to assign the new blob to whatever executable
            var paketTreeNode = currentTree.Tree.FirstOrDefault(t => t.Path == ".paket");
            var paketTree = await Wrap.RateLimiting(client, c => c.Git.Tree.Get(repository.Id, paketTreeNode.Sha));

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

            var updatedPaketTree = await Wrap.RateLimiting(client, c => c.Git.Tree.Create(repository.Id, newPaketTree));

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

            var updatedRootTree = await Wrap.RateLimiting(client, c => c.Git.Tree.Create(repository.Id, newRootTree));

            // create a new commit using the updated tree
            var newCommit = new NewCommit($"Updated {executable.Path} to address TLS 1.0 and 1.1 deprecation", updatedRootTree.Sha, initialSha);
            var commit = await Wrap.RateLimiting(client, c => c.Git.Commit.Create(repository.Id, newCommit));

            // then update the bootstrapper ref to this new commit
            var updatedReference = await Wrap.RateLimiting(client, c => c.Git.Reference.Update(repository.Id, newRef, new ReferenceUpdate(commit.Sha)));

            return updatedReference;
        }
    }
}
