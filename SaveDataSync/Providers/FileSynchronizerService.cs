namespace SaveDataSync.Providers
{
	using Data;
	
	
	public class FileSynchronizerService
	{
		public IEnumerable<FileSyncLog> SyncFolders(string path1, string path2)
		{
			var path1Exists = Directory.Exists(path1);
			var path2Exists = Directory.Exists(path2);

			// Check if at least one path exists
			if (!path1Exists && !path2Exists)
				yield break;

			// Ensure both paths exist by creating them if needed
			if (!path1Exists)
			{
				Directory.CreateDirectory(path1);
			}

			if (!path2Exists)
			{
				Directory.CreateDirectory(path2);
			}

			// Sync files in the current directories and their subdirectories
			foreach (var log in SyncDirectory(path1, path2))
			{
				yield return log;
			}

			foreach (var log in SyncDirectory(path2, path1))
			{
				yield return log;
			}

			Console.WriteLine("Sync complete.");
		}


		private IEnumerable<FileSyncLog> SyncDirectory(string sourceDir, string targetDir)
		{
			// Get all files from the source directory
			foreach (var sourceFilePath in Directory.GetFiles(sourceDir))
			{
				var fileName = Path.GetFileName(sourceFilePath);
				var targetFilePath = Path.Combine(targetDir, fileName);
				var lastModified = File.GetLastWriteTime(sourceFilePath);

				// If the file exists in both directories, compare the last write time
				if (File.Exists(targetFilePath))
				{
					var targetLastModified = File.GetLastWriteTime(targetFilePath);

					// Copy the more recent file to the other folder
					if (lastModified > targetLastModified)
					{
						File.Copy(sourceFilePath, targetFilePath, true);
						yield return new FileSyncLog(fileName, lastModified, "Updated", targetFilePath);
					}
					else if (targetLastModified > lastModified)
					{
						File.Copy(targetFilePath, sourceFilePath, true);
						yield return new FileSyncLog(Path.GetFileName(targetFilePath), targetLastModified, "Updated", sourceDir);
					}
				}
				else
				{
					// File only exists in the source directory, copy it to the target directory
					File.Copy(sourceFilePath, targetFilePath, true);
					yield return new FileSyncLog(fileName, lastModified, "Copied", targetFilePath);
				}
			}

			// Get all subdirectories in the source directory
			var sourceSubDirs = Directory.GetDirectories(sourceDir);
			foreach (var sourceSubDir in sourceSubDirs)
			{
				var subDirName = Path.GetFileName(sourceSubDir);
				var targetSubDir = Path.Combine(targetDir, subDirName);

				// Ensure the target subdirectory exists
				if (!Directory.Exists(targetSubDir))
				{
					Directory.CreateDirectory(targetSubDir);
				}

				// Recursively sync the subdirectories
				foreach (var log in SyncDirectory(sourceSubDir, targetSubDir))
				{
					yield return log;
				}
			}
		}
	}
}
