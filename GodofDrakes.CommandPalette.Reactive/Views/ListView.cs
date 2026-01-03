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

// A ListPage driven by a view model
public partial class ListView<T> : ListPage, IDisposable
	where T : ListViewModel
{
	private readonly CompositeDisposable _onDispose = [];

	private readonly ReadOnlyObservableCollection<IListItem> _listItems;

	// Not IViewFor since CmdPal doesn't support any kind of activation messages
	private T? _viewModel;

	public ListView()
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