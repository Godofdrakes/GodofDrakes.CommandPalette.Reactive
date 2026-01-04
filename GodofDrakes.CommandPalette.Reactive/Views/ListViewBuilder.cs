using System.Linq.Expressions;
using System.Reactive.Linq;
using DynamicData;
using GodofDrakes.CommandPalette.Reactive.Interfaces;
using GodofDrakes.CommandPalette.Reactive.ViewModels;
using Microsoft.CommandPalette.Extensions;
using ReactiveUI;

namespace GodofDrakes.CommandPalette.Reactive.Views;

internal static class ListViewBuilder
{
	public static ListViewBuilder<TViewModel> Create<TView, TViewModel>(
		TView view,
		Expression<Func<TView, TViewModel?>> viewModel )
		where TViewModel : ListViewModel
	{
		ArgumentNullException.ThrowIfNull( view );
		ArgumentNullException.ThrowIfNull( viewModel );

		return new ListViewBuilder<TViewModel>( view.WhenAnyValue( viewModel ) );
	}
}

internal class ListViewBuilder<TViewModel>
	where TViewModel : ListViewModel
{
	private readonly IObservable<TViewModel?> _viewModel;

	internal ListViewBuilder( IObservable<TViewModel?> viewModel )
	{
		ArgumentNullException.ThrowIfNull( viewModel );

		_viewModel = viewModel;
	}

	public IObservable<IChangeSet<IListItem, string>> ListItems
	{
		get
		{
			return _viewModel.Select( ListItems ).Switch();

			static IObservable<IChangeSet<IListItem, string>> ListItems( TViewModel? viewModel )
			{
				if ( viewModel is not null )
				{
					return viewModel.Connect();
				}

				return Observable.Never<IChangeSet<IListItem, string>>();
			}
		}
	}

	public IObservable<bool> IsLoading
	{
		get
		{
			return _viewModel.Select( IsLoading ).Switch();

			static IObservable<bool> IsLoading( TViewModel? vm )
			{
				if ( vm is INotifyLoading source )
				{
					return source.WhenAnyValue( x => x.IsLoading );
				}

				return Observable.Never<bool>().Prepend( false );
			}
		}
	}
}