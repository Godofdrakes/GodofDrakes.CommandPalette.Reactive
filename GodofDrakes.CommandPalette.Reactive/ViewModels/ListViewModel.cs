using System.Reactive.Disposables;
using DynamicData;
using Microsoft.CommandPalette.Extensions;
using ReactiveUI;

namespace GodofDrakes.CommandPalette.Reactive.ViewModels;

public abstract class ListViewModel : ReactiveObject
{
	public abstract IObservable<IChangeSet<IListItem, IListItem>> Connect();
}

public sealed class ReadOnlyListViewModel : ListViewModel
{
	private readonly IListItem[] _listItems = [];

	public IReadOnlyCollection<IListItem> ListItems
	{
		get => _listItems;
		init => _listItems = value.ToArray();
	}

	public override IObservable<IChangeSet<IListItem, IListItem>> Connect()
	{
		return ObservableChangeSet.Create( CreateChangeSet, ( IListItem listItem ) => listItem );
	}

	private IDisposable CreateChangeSet( ISourceCache<IListItem, IListItem> cache )
	{
		// Seed the initial list and then never update
		cache.AddOrUpdate( ListItems );

		return Disposable.Empty;
	}
}