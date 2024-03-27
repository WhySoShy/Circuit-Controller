using Circuit_Controller.Entities;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace Circuit_Controller.Services;

internal sealed class CircuitShellController : CircuitHandler, ICircuitController
{
    public List<CircuitC> Circuits { get; private set; } = new();

    /// <summary>
    /// List that contains all of the circuits if too many was loaded.
    /// </summary>
    public List<CircuitC> DeactivatedCircuits { get; private set; } = new();


    /// <summary>
    /// Adds a component to the list of circuits as a child.
    /// </summary>
    /// <param name="circuitID"></param>
    /// <param name="component"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    public void AddComponent(string circuitID, CircuitComponent component, Action<bool> stateChanger)
    {
        CircuitC? foundCircuit = Circuits.FirstOrDefault(x => x.ID == circuitID);

        // If the circuit does not exist, throw an exception beacuse that should not happend.
        if (foundCircuit is null)
            throw new KeyNotFoundException();

        foundCircuit.Components.Add(component);
    }

    /// <summary>
    /// Change the state of a specific circuit.
    /// </summary>
    /// <param name="circuitId"></param>
    /// <param name="circuitState"></param>
    /// <exception cref="NullReferenceException">Throws exception if it could not find the circuit.</exception>
    public void SetCircuitState(string circuitId, bool circuitState)
    {
        CircuitC? foundCircuit = Circuits.FirstOrDefault(x => x.ID == circuitId);

        if (foundCircuit is null)
            throw new NullReferenceException($"Could not find circuit with id: {circuitId}");

        foundCircuit.IsIdle = circuitState;
        foundCircuit.LastActivity = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }


    #region Circuit Create / Destruction

    // New circuit was created / new tab was created.
    public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        // Ensure that the circuit isn't added twice, should not be possible but just for ensurance.
        if (!Circuits.Any(x => x.ID == circuit.Id))
            Circuits.Add(new() { ID = circuit.Id });

        return base.OnCircuitOpenedAsync(circuit, cancellationToken);
    }

    // Existing circuit was destroyed / tab was closed.
    public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        // Find the circuit.
        CircuitC? foundCircuit = Circuits.FirstOrDefault(x => x.ID == circuit.Id);

        // Remove the found circuit if found.
        if (foundCircuit is not null)
            Circuits.Remove(foundCircuit);

        return base.OnCircuitClosedAsync(circuit, cancellationToken);
    }

    #endregion

    #region Debug features

    public void DisplayAllCircuits()
    {
        Console.Clear();

        foreach (CircuitC circuit in Circuits)
        {
            Console.WriteLine("Parent component: " + circuit.ID);
            Console.WriteLine($"- {circuit.Created}");
            Console.WriteLine($"- {circuit.IsIdle}");
            foreach (CircuitComponent component in circuit.Components)
            {
                Console.WriteLine($" > {component.ID} - {component.Created}");
            }
        }
    }
    public void DisplayActivityInOrder()
    {
        Console.Clear();
        foreach (CircuitC circuit in Circuits.OrderByDescending(x => x.LastActivity))
        {
            Console.WriteLine("Parent component: " + circuit.ID);
            Console.WriteLine($"- {circuit.Created}");
            Console.WriteLine($"- {circuit.IsIdle}");
            foreach (CircuitComponent component in circuit.Components)
            {
                Console.WriteLine($" > {component.ID} - {component.Created}");
            }
        }
    }

    #endregion

    #region Invoke

    /// <summary>
    /// Invoke all <seealso cref="CircuitComponentBase"/>.
    /// </summary>
    /// <returns>Number of components rerendered</returns>
    public int Invoke()
    {
        int componentsRendered = 0;

        // Save a sorted list, so we reduce the ammount of searches done.
        IEnumerable<CircuitC> sortecCircuits = Circuits.Where(x => !x.IsIdle && x.Components.Count > 0 && x.ID != string.Empty);

        // Check if there are any active circuits
        if (sortecCircuits.Count() <= 0)
            return 0;

        foreach (CircuitComponent component in sortecCircuits.SelectMany(x => x.Components))
        {
            // Check if the UpdateEvent has been set.
            if (component.UpdateEvent is null)
                continue;

            // Invoke the method acociated with the event.
            component.UpdateEvent.Invoke();
            componentsRendered++;
        }

        Console.WriteLine($"Updated {componentsRendered} components");

        return componentsRendered;
    }
    /// <summary>
    /// Invoke all <seealso cref="CircuitComponentBase"/> inside a circuit.
    /// </summary>
    /// <param name="circuitId"></param>
    /// <returns></returns>
    public int Invoke(string circuitId)
    {
        int componentsRendered = 0;

        CircuitC? foundCircuit = Circuits.FirstOrDefault(x => x.ID == circuitId && !x.IsIdle && x.Components.Count > 0);

        if (foundCircuit is null)
            return 0;

        foreach (CircuitComponent component in foundCircuit.Components)
        {
            if (component.UpdateEvent is null)
                continue;

            component.UpdateEvent.Invoke();
            componentsRendered++;
        }

        Console.WriteLine($"Updated {componentsRendered} components");

        return componentsRendered;
    }

    #endregion
}

public interface ICircuitController
{
    void DisplayAllCircuits();
    void DisplayActivityInOrder();
    void AddComponent(string circuitID, CircuitComponent component, Action<bool> stateChanger);
    void SetCircuitState(string circuitId, bool circuitState);

    int Invoke();
    int Invoke(string circuitId);
}