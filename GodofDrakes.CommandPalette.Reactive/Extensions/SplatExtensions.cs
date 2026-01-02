using GodofDrakes.CommandPalette.Reactive.ObservableForProperty;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.Microsoft.Extensions.Logging;

namespace GodofDrakes.CommandPalette.Reactive.Extensions;

public static class SplatExtensions
{
	public static IHostBuilder ConfigureSplat( this IHostBuilder hostBuilder )
	{
		hostBuilder.ConfigureServices( ( context, services ) =>
		{
			// Make splat use this service collection for setup
			services.UseMicrosoftDependencyResolver();

			// Necessary for this.WhenAnyValue to work with INotifyPropChanged
			services.AddTransient<ICreatesObservableForProperty, INotifyPropChangedObservableForProperty>();

			// Re-init splat and rxui
			var resolver = Locator.CurrentMutable;
			resolver.InitializeSplat();
			resolver.InitializeReactiveUI();

			// Make sure splat logging works
			services.AddLogging( loggingBuilder =>
			{
				loggingBuilder.AddSplat();
			} );
		} );

		return hostBuilder;
	}

	public static IHost InitializeSplat( this IHost host )
	{
		// Make splat use this service provider for execution.
		// We have to do this a second time since the service object changes between setup and run.
		host.Services.UseMicrosoftDependencyResolver();

		return host;
	}
}