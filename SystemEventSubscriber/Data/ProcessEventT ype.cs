namespace SystemEventSubscriber.Data
{
	/// <summary>
	/// Specifies the types of Windows Management Instrumentation (WMI) Event.
	/// </summary>
	public enum ProcessEventType
	{
		/// <summary>
		/// When a process starts
		/// </summary>
		START_TRACE,

		/// <summary>
		/// When a process stops
		/// </summary>
		STOP_TRACE
	}
}