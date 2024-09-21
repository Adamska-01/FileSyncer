using SaveDataSync.Providers.Factory;


namespace SharedLibrary
{
	public class FilePathsLoader
	{
		public static T LoadFilePaths<T>()
		{
			var saveFilePaths = Path.Combine(Directory.GetCurrentDirectory(), "StaticData/FilePaths.json");

			return Factory.GetProduct<JsonDeserializerFactory<T>, T, string>(saveFilePaths);
		}
	}
}