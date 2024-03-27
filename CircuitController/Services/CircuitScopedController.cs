using Microsoft.AspNetCore.Components.Server.Circuits;

namespace Circuit_Controller.Services;


public class CircuitScopedController : CircuitHandler, IScopedCircuitController
{
    public string CircuitId { get; set; } = string.Empty;

    public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        CircuitId = circuit.Id;
        return base.OnCircuitOpenedAsync(circuit, cancellationToken);
    }
}

public interface IScopedCircuitController
{
    string CircuitId { get; set; }
}