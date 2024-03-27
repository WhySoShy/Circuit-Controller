using Circuit_Controller.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Circuit_Controller.Entities;

namespace Circuit_Controller;

/// <summary>
/// Every child that wants to be affected by a circuit change, should inherit this.
/// </summary>
public class CircuitComponentBase : ComponentBase
{
    #region Dependency injection

    [Inject] private ICircuitController Circuit { get; set; } = default!;
    [Inject] private CircuitHandler ScopedCircuitHandler { get; set; } = default!;

    // Instead of casting our CircuitHandler when accessing it, we can call this ScopedCircuit that casts it for us.
    internal CircuitScopedController ScopedCircuit { get => ScopedCircuitHandler as CircuitScopedController; }

    #endregion

    protected bool _componentState = true;

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (!firstRender)
            return;


        // Add the child (this) into our current circuit.
        Circuit.AddComponent(
                ScopedCircuit.CircuitId,
                new CircuitComponent(OnUpdate),
                // Create anonym method, that changes the state of the current component.
                (bool componentState) => _componentState = componentState
            );

    }


    #region Overrideable methods

    /// <summary>
    /// Overrideable method, that can be modified for each component, remember to call the 
    /// <code>base.OnUpdate()</code> or <code>InvokeAsync(StateHasChanged)</code>
    /// <para>
    ///     I recommend you not to <c>await</c> it, beacuse that could potentionaly pause all of your tabs.
    ///     Beacuse it has to wait on each component to finish rendering.
    /// </para>
    /// </summary>
    protected virtual void OnUpdate() => InvokeAsync(StateHasChanged);

    /// <summary>
    /// <para>
    ///     Determines wether the component should be rendered.
    /// </para>
    /// If overrided, remember to return <seealso cref="_componentState"/>
    /// </summary>
    protected override bool ShouldRender() => _componentState;

    #endregion
}
