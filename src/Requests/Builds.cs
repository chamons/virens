using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Octokit;

using virens.Utilities;

namespace virens.Requests
{
	public class Builds
	{
		GitHubClient Client;
		RequestOptions Options;
		string Owner;
		string Area;

		public Builds (RequestOptions options)
		{
			Options = options;
			Client = new GitHubClient (new ProductHeaderValue ("chamons.virens"));
			Client.Credentials = new Credentials (Options.Pat);
			(Owner, Area) = ParseLocation (Options.Repository);
		}

		public static Builds Create (RequestOptions options) => new Builds (options);

		public async Task AssertLimits ()
		{
			var limits = await Client.Miscellaneous.GetRateLimits ();
			int coreLimit = limits.Resources.Core.Remaining;
			int searchLimit = limits.Resources.Search.Remaining;
			if (coreLimit < 1 || searchLimit < 1)
				Errors.Die ($"Rate Limit Hit: {coreLimit} {searchLimit}");
		}

		(string Owner, string Area) ParseLocation (string location)
		{
			var bits = location.Split ('/');
			if (bits.Length != 2)
				Errors.Die ("--repository formatted incorrectly");
			return (bits[0], bits[1]);
		}

		public async Task<List<BuildInfo>> FindGreenBuilds ()
		{
			var builds = new List<BuildInfo> ();
			foreach (var branchName in Options.Branch) {
				var branch = await Client.Repository.Branch.Get (Owner, Area, branchName);

				var commit = branch.Commit.Sha;
				int count = 0;
				while (true) {
					bool passed = await CheckBuild (commit);
					if (passed) {
						builds.Add (new BuildInfo (branchName, count));
						break;
					}
					count++;

					var commitInfo = await Client.Repository.Commit.Get (Owner, Area, commit);
					// HACK - We only follow the first parent in merge commits
					commit = commitInfo.Parents.First ().Sha;
				}
			}
			return builds;
		}

		async Task<bool> CheckBuild (string sha)
		{
			var status = await Client.Repository.Status.GetCombined (Owner, Area, sha);
			return status.Statuses.All (x =>
				x.State.TryParse (out CommitState state) && state == CommitState.Success);
		}
	}
}