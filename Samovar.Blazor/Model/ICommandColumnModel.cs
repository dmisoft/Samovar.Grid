using System.Reactive.Subjects;

namespace Samovar.Blazor
{
    public interface ICommandColumnModel
        : IColumnModel
    {
		public ISubject<bool> NewButtonVisible { get; }

		public ISubject<bool> EditButtonVisible { get; }

		public ISubject<bool> DeleteButtonVisible { get; }
		public ISubject<string> Title { get; }


		//ISubject<bool> NewButtonVisible { get; }

		//ISubject<bool> ClearFilterButtonVisible { get; }

		//ISubject<bool> EditButtonVisible { get; }

		//ISubject<bool> DeleteButtonVisible { get; }

		//ITemplateRenderer<object> CellTemplate { get; }

		//ITemplateRenderer HeaderCellTemplate { get; }

		//ITemplateRenderer HeaderFilterCellTemplate { get; }
	}
}
