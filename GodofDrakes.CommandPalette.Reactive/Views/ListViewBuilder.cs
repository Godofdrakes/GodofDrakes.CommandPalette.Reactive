using System.Linq.Expressions;
using System.Reactive.Linq;
using DynamicData;
using GodofDrakes.CommandPalette.Reactive.Interfaces;
using GodofDrakes.CommandPalette.Reactive.ViewModels;
using Microsoft.CommandPalette.Extensions;
using ReactiveUI;

namespace GodofDrakes.CommandPalette.Reactive.Views;

internal class ListViewBuilder
{
	private static IObservable<IChangeSet<IListItem, IListItem>> ConnectViewModel( ListViewModel? viewModel )
	{
		return viewModel?.Connect() ?? Observable.Never<IChangeSet<IListItem, IListItem>>();
	}

	private readonly IObservable<ListViewModel?> _viewModel;

	public static ListViewBuilder Create<TView, TViewModel>( TView view,
		Expression<Func<TView, TViewModel?>> viewModel )
		where TViewModel : ListViewModel
	{
		ArgumentNullException.ThrowIfNull( view );
		ArgumentNullException.ThrowIfNull( viewModel );

		return new ListViewBuilder( view.WhenAnyValue( viewModel ) );
	}

	private ListViewBuilder( IObservable<ListViewModel?> viewModel )
	{
		ArgumentNullException.ThrowIfNull( viewModel );

		_viewModel = viewModel;
	}

	public IObservable<IChangeSet<IListItem, IListItem>> ListItems
	{
		get => _viewModel.Select( ConnectViewModel ).Switch();
	}

	public IObservable<bool> IsLoading
	{
		get => _viewModel
			.Select( vm =>
			{
				if ( vm is INotifyLoading source )
				{
					return source.WhenAnyValue( x => x.IsLoading );
				}

				return Observable.Never<bool>().Prepend( false );
			} )
			.Switch();
	}
}