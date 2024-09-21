using SaveDataSync.Data;


namespace SaveDataSync.Providers
{
	public class FileSynchronizerService
	{
		public IEnumerable<FileSyncLog> SyncFolder(string sourceFolder, string destFolder)
		{
			var logs = new List<FileSyncLog>();
			var sourceDir = new DirectoryInfo(sourceFolder);
			var destDir = new DirectoryInfo(destFolder);

			try
			{
				if (!destDir.Exists)
				{
					Directory.CreateDirectory(destDir.FullName);
				}

				// Process files
				foreach (FileInfo file in sourceDir.GetFiles())
				{
					var tempPath = Path.Combine(destDir.FullName, file.Name);
					var status = string.Empty;

					if (File.Exists(tempPath) && File.GetLastWriteTime(tempPath) >= file.LastWriteTime)
					{
						status = "Skipped (Up-to-date)";
					}
					else
					{
						file.CopyTo(tempPath, true);
						status = "Copied";
					}

					logs.Add(new FileSyncLog(file.Name, file.LastWriteTime, status, tempPath));
				}

				// Recursively process subdirectories
				foreach (DirectoryInfo subdir in sourceDir.GetDirectories())
				{
					logs.AddRange(SyncFolder(subdir.FullName, Path.Combine(destDir.FullName, subdir.Name)));
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
