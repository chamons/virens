
namespace virens.Requests
{
	public class BuildInfo
	{
		public string Branch;
		public int Distance;

		public BuildInfo (string branch, int distance)
		{
			Branch = branch;
			Distance = distance;
		}
	}
}