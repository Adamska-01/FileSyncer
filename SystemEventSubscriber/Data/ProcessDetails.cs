namespace SystemEventSubscriber.Data
{
	/// <summary>
	/// Represents details of a process, including the name of the process.
	/// </summary>
	public readonly struct ProcessDetails
	{
		/// <summary>
		/// Gets or sets the name of the process.
		/// </summary>
		public required string ProcessName { get; init; }
	}
}