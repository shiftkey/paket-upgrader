using System;

namespace PaketUpgrader
{
    class Options
    {
        public Options()
        {
            Repositories = Array.Empty<string>();
        }

        public string Token { get; set; }
        public string[] Repositories { get; set; }
        public string Account { get; set; }
        public bool IncludeForks { get; internal set; }
        public bool SubmitPullRequests { get; internal set; }
        public bool Debug { get; internal set; }
    }
}
