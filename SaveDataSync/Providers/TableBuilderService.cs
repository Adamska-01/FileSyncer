using Alba.CsConsoleFormat;


namespace SaveDataSync.Providers
{
	public class TableBuilderService
	{
		private readonly Grid grid;

		private readonly LineThickness borderThickness;


		public TableBuilderService(LineThickness borderThickness, ConsoleColor borderColor)
		{
			this.borderThickness = borderThickness;

			grid = new Grid
			{
				Stroke = borderThickness,
				StrokeColor = borderColor
			};
		}


		public TableBuilderService AddColumn(int width, int maxWidth = -1)
		{
			grid.Columns.Add(new Column
			{
				Width = GridLength.Auto,
				MinWidth = width,
				MaxWidth = maxWidth > 0 ? maxWidth : int.MaxValue
			});

			return this;
		}

		public void AddRow(ConsoleColor textColor, Align textlignment, params string[] values)
		{
			foreach (var value in values)
			{
				grid.Children.Add(new Cell(value)
				{
					Stroke = borderThickness,
					Color = textColor,
					Align = textlignment
				});
			}
		}

		public Document Build()
		{
			var document = new Document();
			document.Children.Add(grid);

			return document;
		}
	}
}