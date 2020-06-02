using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Mono.Options;

using virens.Requests;

namespace virens
{
	class EntryPoint
	{
		static async Task Main (string[] args)
		{
			var options = new RequestOptions ();
			bool showHelp = false;

			OptionSet os = new OptionSet ()
			{
				{ "h|?|help", "Displays the help", v => showHelp = true },
				{ "r|repository=", "Github repository to consider.", r => options.Repository = r },
				{ "t|token=", "Token to use to communicate with Github via OctoKit", t => options.Pat = t },
				{ "b|branch=", "A branch to review builds. Can be specified multiple times. (Default master)", b => options.Branch.Add (b) },
			};

			try {
				IList<string> unprocessed = os.Parse (args);
			} catch (Exception e) {
				Console.Error.WriteLine ("Could not parse the command line arguments: {0}", e.Message);
				return;
			}

			if (showHelp) {
				ShowHelp (os);
				return;
			}

			options.Validate ();

			var buildInfos = await Builds.Create (options).FindGreenBuilds ();
			foreach (var build in buildInfos.OrderBy (x => x.Branch)) {
				Console.WriteLine ($"{build.Branch} - {build.Distance}");
			}
		}

		static void ShowHelp (OptionSet os)
		{
			Console.WriteLine ("vrens [options] path");
			os.WriteOptionDescriptions (Console.Out);
		}
	}
}
