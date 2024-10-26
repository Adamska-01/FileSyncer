namespace SharedLibrary.RClone.Generic.Interfaces
{
	/// <summary>
	/// Defines a contract for response rules that can match an input and generate a response.
	/// </summary>
	public interface IResponseRule
	{
		/// <summary>
		/// Gets the message associated with the response rule.
		/// </summary>
		string Message { get; }


		/// <summary>
		/// Determines whether the specified input matches the rule's pattern.
		/// </summary>
		/// <param name="input">The input string to check against the pattern.</param>
		/// <returns>
		/// <c>true</c> if the input matches the pattern; otherwise, <c>false</c>.
		/// </returns>
		bool IsMatch(string input);

		/// <summary>
		/// Generates a response based on the rule's response generator.
		/// </summary>
		/// <returns>A response string.</returns>
		string GetResponse();
	}
}
