using System.Text.RegularExpressions;


namespace SharedLibrary.RClone.Generic
{
	using Interfaces;


	/// <summary>
	/// Represents a rule that matches input against a pattern and provides a corresponding response.
	/// </summary>
	public class ResponseRule : IResponseRule
	{
		private readonly Regex regex;
		
		private readonly Func<string> responseGenerator;


		public string Message { get; }


		public ResponseRule(string pattern, string message, string response)
			: this(pattern, message, () => response) 
		{ 
		}

		public ResponseRule(string pattern, string message, Func<string> responseGenerator)
		{
			regex = new Regex(pattern, RegexOptions.Compiled);
			Message = message;
			this.responseGenerator = responseGenerator;
		}


		bool IResponseRule.IsMatch(string input) 
			=> regex.IsMatch(input);
		
		string IResponseRule.GetResponse() 
			=> responseGenerator();
	}
}
