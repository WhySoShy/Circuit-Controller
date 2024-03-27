using Circuit_Controller.Services;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.Extensions.DependencyInjection;

namespace Circuit_Controller.Util;

public static class Builder
{
    public static IServiceCollection AddCircuitController(this IServiceCollection service)
    {
        // Create a new instance of the shell controller.
        var shellInstance = new CircuitShellController();

        // Refer to the created instance, for a seperate singleton with a known reference to each other.
        service.AddSingleton<CircuitHandler>(shellInstance);
        service.AddSingleton<ICircuitController>(shellInstance);

        // Add our scoped controller to the services, we use it to get the current circuit of the circuit we are working on.
        service.AddScoped<CircuitHandler>(provider => new CircuitScopedController());
        // Get the already created CircuitHandler from the current scoped services and cast to IScopedCircuitController.
        service.AddScoped<IScopedCircuitController>(provider => provider.GetRequiredService<CircuitHandler>() as IScopedCircuitController);

        return service;
    }
}
