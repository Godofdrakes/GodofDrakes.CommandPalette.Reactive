using System.Linq.Expressions;
using System.Reactive.Linq;
using Windows.Foundation;
using Microsoft.CommandPalette.Extensions;
using ReactiveUI;

namespace GodofDrakes.CommandPalette.Reactive.ObservableForProperty;

public sealed partial class INotifyPropChangedObservableForProperty : ICreatesObservableForProperty
{
	public int GetAffinityForObject( Type type, string propertyName, bool beforeChanged = false )
	{
		if ( type.IsAssignableTo( typeof(INotifyPropChanged) ) )
		{
			return 2;
		}

		return 0;
	}

	public IObservable<IObservedChange<object?, object?>> GetNotificationForProperty(
		object sender,
		Expression? expression,
		string propertyName,
		bool beforeChanged = false,
		bool suppressWarnings = false )
	{
		if ( sender is not INotifyPropChanged source )
		{
			var message = "Sender must implement INotifyPropChanged";

			throw new ArgumentException( message, nameof(sender) );
		}

		ArgumentException.ThrowIfNullOrWhiteSpace( propertyName );
		ArgumentNullException.ThrowIfNull( expression );

		if ( expression.NodeType is not ExpressionType.MemberAccess )
		{
			var message = "INotifyPropChanged only supports property access";

			throw new NotSupportedException( message );
		}

		if ( beforeChanged )
		{
			var message = "INotifyPropChanged does not support before change events. Type: {Type}";

			throw new NotSupportedException( message );
		}

		var propChanged = Observable
			.FromEvent<TypedEventHandler<object?, IPropChangedEventArgs>, string?>(
				eventHandler =>
				{
					return Handler;

					void Handler( object? _, IPropChangedEventArgs args )
					{
						eventHandler( args.PropertyName );
					}
				},
				handler => source.PropChanged += handler,
				handler => source.PropChanged -= handler );

		return propChanged
			.Where( name => string.IsNullOrWhiteSpace( name ) || name.Equals( propertyName, StringComparison.Ordinal ) )
			.Select( _ => new ObservedChange<object?, object?>( sender, expression, null ) );
	}
}