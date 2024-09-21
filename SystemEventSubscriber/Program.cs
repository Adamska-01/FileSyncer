using Microsoft.Win32.TaskScheduler;
using SharedLibrary;
using System.Diagnostics;
using System.Management;
using SystemEventSubscriber.Data;
using SystemEventSubscriber.Provider.Extensions;


class Program
{
	// Event to block the program from exiting
	private static ManualResetEvent quitEvent = new ManualResetEvent(false);

	static void Main(string[] args)
	{
		string taskName = "SystemEventSubscriber";
		var taskFolderPath = @"\SaveDataSync";

		// Get the current program's executable path
		var exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
		// Create a new task service instance
		using (var taskService = new TaskService())
		{
			// Check if the task with the same name already exists
			var existingTask = taskService.FindTask(taskName);

			if (existingTask == null)
			{
				// If task doesn't exist, create a new task definition
				var taskDef = taskService.NewTask();
				taskDef.RegistrationInfo.Description = "SystemEventSubscriber to sync save data for games that don't support steam cloud";

				// Set the trigger (to run at system startup)
				taskDef.Triggers.Add(new BootTrigger());

				// Define the action (run the current executable)
				taskDef.Actions.Add(new ExecAction(exePath, null, null));

				// Set the task to run with highest privileges (administrator rights)
				taskDef.Principal.UserId = "SYSTEM"; // Run as SYSTEM user
				taskDef.Principal.LogonType = TaskLogonType.ServiceAccount;
				taskDef.Principal.RunLevel = TaskRunLevel.Highest;
				// Register the task in the Task Scheduler root folder
				// Check if the folder exists
				TaskFolder folder;
				try
				{
					folder = taskService.GetFolder(taskFolderPath);
				}
				catch (FileNotFoundException)
				{
					// Folder doesn't exist, create it
					folder = taskService.RootFolder.CreateFolder(taskFolderPath, new TaskSecurity());
				}

				folder.RegisterTaskDefinition(taskName, taskDef);
			}
		}


		var savePaths = FilePathsLoader.LoadFilePaths<ProcessDetails[]>();

		var startQuery = GetProcessQuery(savePaths, ProcessEventType.START_TRACE);
		var startWatcher = new ManagementEventWatcher(startQuery);
		startWatcher.EventArrived += new EventArrivedEventHandler(GameClosedHandler);

		var stopQuery = GetProcessQuery(savePaths, ProcessEventType.STOP_TRACE);
		ManagementEventWatcher endWatcher = new ManagementEventWatcher(stopQuery);
		endWatcher.EventArrived += new EventArrivedEventHandler(GameClosedHandler);
		
		startWatcher.Start();
		endWatcher.Start();

		quitEvent.WaitOne();

		startWatcher.Stop();
		endWatcher.Stop();
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
