namespace SaveDataSync.Providers.Factory.Interfaces
{
	/// <summary>
	/// Interface for factory services that create products without requiring an initialiser.
	/// </summary>
	/// <typeparam name="T">The type of object the factory creates.</typeparam>
	public interface IFactory<T>
	{
		/// <summary>
		/// Creates a <typeparamref name="T"/> product.
		/// </summary>
		/// <returns>The created <typeparamref name="T"/> product.</returns>
		T CreateProduct();
	}


	/// <summary>
	/// Interface for factory services that create products, requiring a <typeparamref name="U"/> initialiser to create them.
	/// </summary>
	/// <typeparam name="T">The type of object the factory creates.</typeparam>
	/// <typeparam name="U">The type of the initialiser used to create the object.</typeparam>
	public interface IFactory<out T, in U>
	{
		/// <summary>
		/// Creates a <typeparamref name="T"/> product using the specified <typeparamref name="U"/> <paramref name="initialiser"/>.
		/// </summary>
		/// <param name="initialiser">The <typeparamref name="U"/> <paramref name="initialiser"/> to be used to create the <typeparamref name="T"/> product.</param>
		/// <returns>The created <typeparamref name="T"/> product.</returns>
		T CreateProduct(U initialiser);
	}
}