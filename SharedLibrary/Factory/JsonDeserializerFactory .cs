using System.Text.Json;


namespace SaveDataSync.Providers.Factory
{
	using Interfaces;


	public class JsonDeserializerFactory<TReturn> : IFactory<TReturn, string>
	{
		TReturn IFactory<TReturn, string>.CreateProduct(string jsonPath)
		{
			var paths = File.ReadAllText(jsonPath);
			var savePaths = JsonSerializer.Deserialize<TReturn>(paths) ?? throw new Exception("An error occurred while deserializing Paths.json");

			return savePaths;
		}
	}
}
