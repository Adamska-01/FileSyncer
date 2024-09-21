using SystemEventSubscriber.Data;


namespace SystemEventSubscriber.Provider.Extensions
{
	public static class ProcessEventTypeExtensions
	{
		public static string ToWMIEvent(this ProcessEventType processEvent) => processEvent switch
		{
			ProcessEventType.START_TRACE => "Win32_ProcessStartTrace",
			
			ProcessEventType.STOP_TRACE => "Win32_ProcessStopTrace",
			
			_ => throw new ArgumentException("The \"ProcessEventType\" is not supported.", processEvent.ToString())
		};
	}
}