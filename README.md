# paket-upgrader

This is a Windows command line tool for scanning for out-of-date Paket
installations and auto-generating a PR to update these. This uses the GitHub
API to do this, avoiding the need to clone the repository locally.

```shellsession
g>.\paket-upgrader.exe
Usage: paket-upgrader.exe [OPTIONS]
Scan a number of GitHub repositories to determine whether they are running an old version of Paket.
Specify an input file to read, or an account to scan for repositories to inspect.

Options:
  -t, --token=VALUE          a personal access token with public_repo scope to
                               be used to interact with the GitHub API
  -f, --file=VALUE           a file containing a list of 'owner/repo' values,
                               one on each line
  -u, --user=VALUE           a user or organization on GitHub to scan for
                               problem repositories
      --include-forks        also include forked repositories when scanning for
                               repositories to update
      --submit-pull-request  actually submit a PR to each of the repositories
                               which are using an old version of Paket
  -h, --help                 show this message and exit
```