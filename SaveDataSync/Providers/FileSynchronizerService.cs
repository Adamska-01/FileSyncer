using SaveDataSync.Data;


namespace SaveDataSync.Providers
{
	public class FileSynchronizerService
	{
		public IEnumerable<FileSyncLog> SyncFolder(string cloudFolder, string physicalFolder)
		{
			var logs = new List<FileSyncLog>();
			var cloudDir = new DirectoryInfo(cloudFolder);
			var physicalDir = new DirectoryInfo(physicalFolder);

			try
			{
				if (!physicalDir.Exists)
				{
					Directory.CreateDirectory(physicalDir.FullName);
				}

				// Sync files from cloud folder to physical folder
				foreach (var cloudFile in cloudDir.GetFiles())
				{
					var physicalFilePath = Path.Combine(physicalDir.FullName, cloudFile.Name);
					var physicalFile = new FileInfo(physicalFilePath);

					var status = string.Empty;
					var destinationPath = string.Empty;

					if (physicalFile.Exists)
					{
						// Check if physical file is newer than the cloud file
						if (physicalFile.LastWriteTime > cloudFile.LastWriteTime)
						{
							var cloudFilePath = Path.Combine(cloudDir.FullName, cloudFile.Name);
							physicalFile.CopyTo(cloudFilePath, overwrite: true);
							status = "Copied to Cloud";
							destinationPath = cloudFilePath;
						}
						else
						{
							status = "Already up to date";
							destinationPath = "N/A";
						}
					}
					else
					{
						// If physical file does not exist, copy cloud file to physical folder
						cloudFile.CopyTo(physicalFilePath, overwrite: true);
						status = "Copied to Physical";
						destinationPath = physicalFilePath;
					}

					logs.Add(new FileSyncLog(cloudFile.Name, cloudFile.LastWriteTime, status, destinationPath));
				}

				// Recursively sync subdirectories
				foreach (var subdir in cloudDir.GetDirectories())
				{
					logs.AddRange(SyncFolder(subdir.FullName, Path.Combine(physicalDir.FullName, subdir.Name)));
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				Console.WriteLine($"Access denied: {ex.Message}");
			}
			catch (IOException ex)
			{
				Console.WriteLine($"I/O error: {ex.Message}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Unexpected error: {ex.Message}");
			}

			return logs;
		}
	}
}
