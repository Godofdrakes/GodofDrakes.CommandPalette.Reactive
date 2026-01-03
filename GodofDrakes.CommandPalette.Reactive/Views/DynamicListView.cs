using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using DynamicData;
using GodofDrakes.CommandPalette.Reactive.ViewModels;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using ReactiveUI;

namespace GodofDrakes.CommandPalette.Reactive.Views;

// A DynamicListView driven by a view model
public partial class DynamicListView<T> : DynamicListPage, IDisposable
	where T : DynamicListViewModel
{
	private readonly CompositeDisposable _onDispose = [];

	private readonly ReadOnlyObservableCollection<IListItem> _listItems;

	// Not IViewFor since CmdPal doesn't support any kind of activation messages
	private T? _viewModel;

	public DynamicListView()
	{
		var builder = ListViewBuilder.Create( this, view => view.ViewModel );

		builder.ListItems
			.Bind( out _listItems )
			.Do( _ => RaiseItemsChanged() )
			.Subscribe()
			.DisposeWith( _onDispose );

		builder.IsLoading
			.BindTo( this, view => view.IsLoading )
			.DisposeWith( _onDispose );
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

	protected virtual void Dispose( bool disposing )
	{
		if ( disposing )
		{
			_onDispose.Dispose();
		}
	}

	public void Dispose()
	{
		Dispose( true );
		GC.SuppressFinalize( this );
	}
}