using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using GodofDrakes.CommandPalette.Reactive.ViewModels;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using ReactiveUI;

namespace GodofDrakes.CommandPalette.Reactive.Views;

public partial class ListView : ListPage
{
	internal ListView() { }

	internal static IObservable<IChangeSet<IListItem, IListItem>> Connect( ListViewModel? viewModel )
	{
		return viewModel?.Connect() ?? Observable.Never<IChangeSet<IListItem, IListItem>>();
	}
}

// A ListPage driven by a view model
public partial class ListView<T> : ListView
	where T : ListViewModel
{
	private readonly ReadOnlyObservableCollection<IListItem> _listItems;

	// Not IViewFor since CmdPal doesn't support any kind of activation messages
	private T? _viewModel;

	public ListView()
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

	public sealed override IListItem[] GetItems()
	{
		return _listItems.ToArray();
	}
}