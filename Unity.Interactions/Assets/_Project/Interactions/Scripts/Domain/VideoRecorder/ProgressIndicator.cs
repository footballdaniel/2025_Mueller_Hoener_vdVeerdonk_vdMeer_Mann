using System;

namespace Interactions.Domain.VideoRecorder
{
	public class ProgressIndicator : IProgress<int>
	{
		static ProgressIndicator()
		{
			_statement = new ProgressStatement();
		}

		// Singleton instance
		static readonly ProgressIndicator _instance = new();
		static readonly ProgressStatement _statement;

		// Event to notify subscribers of progress changes
		public event Action<ProgressStatement> ProgressChanged;

		// Public property to access the instance
		public static ProgressIndicator Instance => _instance;

		// Additional properties
		public int MaxValue { get; set; }
		public string Title { get; set; }
		public string Task { get; set; }
		public int CurrentValue { get; private set; }


		public void Report(int value)
		{
			_statement.Value = value;
			ProgressChanged?.Invoke(_statement);
		}

		public void Display(string exporting, string frameExport, int maxValue)
		{
			_statement.Title = exporting;
			_statement.Task = frameExport;
			_statement.MaxValue = maxValue;
		}
	}

	public class ProgressStatement
	{
		public int MaxValue { get; set; }
		public string Title { get; set; }
		public string Task { get; set; }
		public int Value { get; set; }
	}
}