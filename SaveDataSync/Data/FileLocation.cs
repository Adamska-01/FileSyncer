namespace SaveDataSync.Data
{
	/// <summary>
	/// Represents the location of a file both in the cloud and on the physical storage.
	/// </summary>
	public readonly struct FileLocation
	{
		/// <summary>
		/// The path to the file in cloud storage.
		/// </summary>
		public required string CloudPath { get; init; }

		/// <summary>
		/// The physical path to the file on the local storage.
		/// </summary>
		public required string PhysicalPath { get; init; }
	}
}