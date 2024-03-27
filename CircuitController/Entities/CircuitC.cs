namespace Circuit_Controller.Entities;

public class CircuitC
{
    /// <summary>
    /// ID of the current circuit - given by blazor.
    /// </summary>
    public string? ID { get; internal set; }

    /// <summary>
    /// Method to call if you want to update the whole circuit.
    /// </summary>
    public Action? UpdateCircuit { get; internal set; }

    /// <summary>
    /// Determines inactivity.
    /// </summary>
    public bool IsIdle { get; internal set; }
    public DateTime Created { get; } = DateTime.Now;

    /// <summary>
    /// Running JS interops to change the last activity. <br /> 
    /// Used unix timestamp for more acurate readings.
    /// <para>
    ///     This is being used to change the state of unused circuits if too many circuits are available.
    /// </para>
    /// </summary>
    public long LastActivity { get; internal set; }

    public List<CircuitComponent> Components { get; internal set; } = new();

}