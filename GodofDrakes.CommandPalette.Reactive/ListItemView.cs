using System.Reactive.Disposables;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace GodofDrakes.CommandPalette.Reactive;

public partial class ListItemView : ListItem, IDisposable
{
	private readonly CompositeDisposable _onDispose = [];

	public static ListItemView Create( Func<ListItemView, IDisposable?> configure )
	{
		ArgumentNullException.ThrowIfNull( configure );

		var view = new ListItemView();

		var disposable = configure( view );

		if ( disposable is not null )
		{
			view._onDispose.Add( disposable );
		}

		return view;
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