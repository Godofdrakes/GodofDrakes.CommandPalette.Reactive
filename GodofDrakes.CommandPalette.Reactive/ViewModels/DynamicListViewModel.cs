using ReactiveUI;

namespace GodofDrakes.CommandPalette.Reactive.ViewModels;

public abstract class DynamicListViewModel : ListViewModel
{
	private string _searchText = string.Empty;

	public string SearchText
	{
		get => _searchText;
		set => this.RaiseAndSetIfChanged( ref _searchText, value );
	}
}