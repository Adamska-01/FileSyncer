namespace SaveDataSync.Data
{
	public readonly struct FileLocation
	{
		public required string CloudPath { get; init; }

		public required string PhysicalPath { get; init; }
	}
}