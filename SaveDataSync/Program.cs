using Alba.CsConsoleFormat;
using SaveDataSync.Data;
using SaveDataSync.Providers;
using SharedLibrary;


class Program
{
	static void Main()
	{
		var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		var oneDrivePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "OneDrive");

		var savePaths = FilePathsLoader.LoadFilePaths<FileLocation[]>();

		var tableBuilder = new TableBuilderService(LineThickness.Double, ConsoleColor.DarkYellow)
			.AddColumn(25)
			.AddColumn(20, 20)
			.AddColumn(20, 20)
			.AddColumn(40);

		tableBuilder.AddRow(
			ConsoleColor.Red, 
			Align.Center, 
			"File/Directory Name", 
			"Last Modified", 
			"Status", 
			"Destination Path");

		var fileSynchronizer = new FileSynchronizerService();
		var logs = new List<FileSyncLog>();

		foreach (var path in savePaths)
		{
			try
			{
				var sourcePath = Path.Combine(oneDrivePath, path.CloudPath);
				var destinationPath = Path.Combine(appDataPath, path.PhysicalPath);

				logs.AddRange(fileSynchronizer.SyncFolders(sourcePath, destinationPath));
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occurred while copying files: {ex.Message}");
			}
		}

		foreach (var log in logs)
		{
			tableBuilder.AddRow(
				ConsoleColor.White,
				Align.Left,
				log.FileName,
				log.LastModified.ToString("dd/MM/yyyy HH:mm:ss"),
				log.SyncStatus,
				log.DestinationPath);
		}

		var tableDocument = tableBuilder.Build();
		ConsoleRenderer.RenderDocument(tableDocument);

		// Add an explicit termination point
		Console.WriteLine("\nPress any key to exit...");
		Console.ReadKey();
	}
}