namespace SaveDataSync.Providers.Factory
{
	using Interfaces;


	public static class Factory
	{
		/// <summary>
		/// Creates the product : <typeparamref name="U"/> by first creating the <see cref="IFactory{T}"/> : <typeparamref name="T"/> /> 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="U"></typeparam>
		/// <param name="initialiser"></param>
		/// <returns></returns>
		public static U GetProduct<T, U>() where T : IFactory<U>, new()
			=> new T().CreateProduct();

		/// <summary>
		/// Creates the product : <typeparamref name="U"/> by first creating the <see cref="IFactory{T, U}"/> : <typeparamref name="T"/> passing the <paramref name="initialiser"/> : <typeparamref name="V"/> as an aurgument to <see cref="IFactory{T, U}.CreateProduct(U)"/> 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="U"></typeparam>
		/// <typeparam name="V"></typeparam>
		/// <param name="initialiser"></param>
		/// <returns></returns>
		public static U GetProduct<T, U, V>(V initialiser) where T : IFactory<U, V>, new()
			=> new T().CreateProduct(initialiser);
	}
}