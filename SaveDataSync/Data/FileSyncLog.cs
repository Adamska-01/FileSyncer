namespace SaveDataSync.Data
{
	public class FileSyncLog
	{
		public string FileName { get; init; }

		public DateTime LastModified { get; init; }
		
		public string SyncStatus { get; init; }
		
		public string DestinationPath { get; init; }


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