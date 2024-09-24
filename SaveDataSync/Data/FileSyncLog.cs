namespace SaveDataSync.Data
{
	/// <summary>
	/// Represents a log entry for a file synchronization process, tracking file details and sync status.
	/// </summary>
	public class FileSyncLog
	{
		/// <summary>
		/// The name of the file that was synchronized.
		/// </summary>
		public string FileName { get; init; }

		/// <summary>
		/// The date and time when the file was last modified.
		/// </summary>
		public DateTime LastModified { get; init; }

		/// <summary>
		/// The status of the file synchronization, such as "Success" or "Failed".
		/// </summary>
		public string SyncStatus { get; init; }

		/// <summary>
		/// The destination path where the file was synchronized to.
		/// </summary>
		public string DestinationPath { get; init; }


		/// <summary>
		/// Initializes a new instance of the <see cref="FileSyncLog"/> class.
		/// </summary>
		/// <param name="fileName">The name of the file being synchronized.</param>
		/// <param name="lastModified">The date and time when the file was last modified.</param>
		/// <param name="syncStatus">The synchronization status of the file.</param>
		/// <param name="destinationPath">The destination path where the file was synchronized.</param>
		public FileSyncLog(
			string fileName,
			DateTime lastModified,
			string syncStatus,
			string destinationPath)
		{
			FileName = fileName;
			LastModified = lastModified;
			SyncStatus = syncStatus;
			DestinationPath = destinationPath;
		}
	}
}