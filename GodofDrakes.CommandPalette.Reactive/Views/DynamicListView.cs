using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using GodofDrakes.CommandPalette.Reactive.ViewModels;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using ReactiveUI;

namespace GodofDrakes.CommandPalette.Reactive.Views;

public abstract partial class DynamicListView : DynamicListPage
{
	internal DynamicListView() { }

	internal static IObservable<IChangeSet<IListItem, IListItem>> Connect( DynamicListViewModel? viewModel )
	{
		return viewModel?.Connect() ?? Observable.Never<IChangeSet<IListItem, IListItem>>();
	}
}

// A DynamicListView driven by a view model
public partial class DynamicListView<T> : DynamicListView
	where T : DynamicListViewModel
{
	private readonly ReadOnlyObservableCollection<IListItem> _listItems;

	// Not IViewFor since CmdPal doesn't support any kind of activation messages
	private T? _viewModel;

	public DynamicListView()
	{
		this.WhenAnyValue( x => x.ViewModel )
			.Select( Connect )
			.Switch()
			.Bind( out _listItems )
			.Do( _ => RaiseItemsChanged() )
			.Subscribe();
	}

	public T? ViewModel
	{
		get => _viewModel;
		set
		{
			if ( object.Equals( _viewModel, value ) )
			{
				return;
			}

			_viewModel = value;

			OnPropertyChanged( nameof(ViewModel) );
		}
	}

	public override void UpdateSearchText( string oldSearch, string newSearch )
	{
		if ( string.Equals( oldSearch, newSearch, StringComparison.Ordinal ) )
		{
			return;
		}

		if ( ViewModel is not null )
		{
			ViewModel.SearchText = newSearch;
		}
	}

	public sealed override IListItem[] GetItems()
	{
		return _listItems.ToArray();
	}
}