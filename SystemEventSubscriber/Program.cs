using Microsoft.Win32.TaskScheduler;
using SharedLibrary;
using System.Diagnostics;
using System.Management;
using SystemEventSubscriber.Data;
using SystemEventSubscriber.Provider.Extensions;


class Program
{
	private const string TASK_NAME = "SystemEventSubscriber";

	private const string TASK_FOLDER_PATH = @"\SaveDataSync";


	private static ProcessDetails[] monitoredPaths = Array.Empty<ProcessDetails>();

	private static ManagementEventWatcher? startWatcher;

	private static ManagementEventWatcher? endWatcher;

	private static FileSystemWatcher? configWatcher;


	static void Main(string[] args)
	{
		CreateScheduledTask();

		LoadMonitoredPaths();

		SetupEventWatchers(monitoredPaths);

		SetupConfigFileWatcher();

		var quitEvent = new ManualResetEvent(false);
		quitEvent.WaitOne();

		CleanupWatchers();
	}


	static void CreateScheduledTask()
	{
		var exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;

		using var taskService = new TaskService();

		var folder = taskService.GetFolder(TASK_FOLDER_PATH) ?? taskService.RootFolder.CreateFolder(TASK_FOLDER_PATH, new TaskSecurity());

		var existingTask = folder.Tasks.FirstOrDefault(t => t.Name == TASK_NAME);

		if (existingTask == null)
		{
			var taskDef = taskService.NewTask();
			taskDef.RegistrationInfo.Description = "SystemEventSubscriber to sync save data for games that don't support steam cloud";

			taskDef.Triggers.Add(new BootTrigger());
			taskDef.Actions.Add(new ExecAction(exePath, null, null));

			taskDef.Principal.UserId = "SYSTEM";
			taskDef.Principal.LogonType = TaskLogonType.ServiceAccount;
			taskDef.Principal.RunLevel = TaskRunLevel.Highest;

			folder.RegisterTaskDefinition(TASK_NAME, taskDef);

			Console.WriteLine($"Task '{TASK_NAME}' created.");
		}
		else
		{
			Console.WriteLine($"Task '{TASK_NAME}' already exists.");
		}
	}

	static void LoadMonitoredPaths()
	{
		// Example: Load from JSON or other source
		monitoredPaths = FilePathsLoader.LoadFilePaths<ProcessDetails[]>();

		Console.WriteLine("Loaded monitored paths.");
	}

	static void SetupEventWatchers(ProcessDetails[] paths)
	{
		// Clean up existing watchers if any
		CleanupWatchers();

		var startQuery = GetProcessQuery(paths, ProcessEventType.START_TRACE);
		startWatcher = new ManagementEventWatcher(startQuery);
		startWatcher.EventArrived += GameClosedHandler;

		var stopQuery = GetProcessQuery(paths, ProcessEventType.STOP_TRACE);
		endWatcher = new ManagementEventWatcher(stopQuery);
		endWatcher.EventArrived += GameClosedHandler;
		
		startWatcher.Start();
		endWatcher.Start();

		Console.WriteLine("Event watchers started.");
	}

	static void CleanupWatchers()
	{
		if (startWatcher != null)
		{
			startWatcher.Stop();
			startWatcher.EventArrived -= GameClosedHandler;
			startWatcher.Dispose();
			startWatcher = null;
		}
		if (endWatcher != null)
		{
			endWatcher.Stop();
			endWatcher.EventArrived -= GameClosedHandler;
			endWatcher.Dispose();
			endWatcher = null;
		}
	}

	static void SetupConfigFileWatcher()
	{
		string configFile = "monitoredPaths.json";  // Adjust to your config file path

		configWatcher = new FileSystemWatcher(Path.GetDirectoryName(configFile))
		{
			Filter = Path.GetFileName(configFile),
			NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName
		};

		configWatcher.Changed += OnConfigFileChanged;
		configWatcher.Created += OnConfigFileChanged;
		configWatcher.Renamed += OnConfigFileChanged;
		configWatcher.EnableRaisingEvents = true;

		Console.WriteLine("Config file watcher enabled.");
	}

	private static void OnConfigFileChanged(object sender, FileSystemEventArgs e)
	{
		// Small delay to allow file write to finish
		Thread.Sleep(100);

		try
		{
			Console.WriteLine("Config file changed. Reloading monitored paths...");
			LoadMonitoredPaths();

			SetupEventWatchers(monitoredPaths);

			Console.WriteLine("Reload complete.");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error reloading config: {ex.Message}");
		}
	}


	static string GetProcessQuery(ProcessDetails[] processDetails, ProcessEventType eventType)
	{
		var conditions = processDetails
			.Select(process => $"ProcessName LIKE '%{Path.GetFileNameWithoutExtension(process.ProcessName)}%'");

		return $"SELECT * FROM {eventType.ToWMIEvent()} WHERE " + string.Join(" OR ", conditions) + "\n";
	}

	static void GameClosedHandler(object sender, EventArrivedEventArgs e)
	{
		var oneDrivePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "OneDrive");

		var StartInfo = new ProcessStartInfo
		{
			FileName = Path.Combine(oneDrivePath, "SaveDataSync/Builds/SaveDataSync/SaveDataSync.exe"),
			CreateNoWindow = false,
			RedirectStandardOutput = false
		};

		Process.Start(StartInfo);
	}
}