using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using virens.Utilities;

namespace virens.Requests
{
	public class RequestOptions
	{
		public string Repository = null;
		public string Pat = null;
		public List<string> Branch = new List<string> ();

		public void Validate ()
		{
			if (String.IsNullOrEmpty (Repository))
				Errors.Die ("Unable to read repository location");

			if (String.IsNullOrEmpty (Pat)) {
				var defaultPat = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.UserProfile), ".github-status-pat");
				if (File.Exists (defaultPat)) {
					Pat = File.ReadAllLines (defaultPat).First ();
				}
			}
			if (String.IsNullOrEmpty (Pat)) {
				Errors.Die ("Unable to read GitHub PAT token");
			}

			if (Branch.Count == 0) {
				Branch.Add ("master");
			}
		}
	}
}