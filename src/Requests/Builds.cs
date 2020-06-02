using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Octokit;

using virens.Utilities;

namespace virens.Requests
{
	public class Issues
	{
		GitHubClient Client;
		RequestOptions Options;
		string Owner;
		string Area;

		public Issues (RequestOptions options)
		{
			Options = options;
			Client = new GitHubClient (new ProductHeaderValue ("chamons.virens"));
			Client.Credentials = new Credentials (Options.Pat);
			(Owner, Area) = ParseLocation (Options.Repository);
		}

		public static Issues Create (RequestOptions options) => new Issues (options);

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

		public async Task Find ()
		{
		}
	}
}