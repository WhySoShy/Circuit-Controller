namespace Circuit_Controller.Entities;

public class CircuitComponent
{
    public CircuitComponent() { }
    public CircuitComponent(Action updateEvent) => UpdateEvent = updateEvent;

    /// <summary>
    /// ID of the current component, can be edited to always being the same (dont recommend).
    /// </summary>
    public Guid ID { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Method to call when you want to update the component.
    /// </summary>
    public Action? UpdateEvent { get; set; }

    public DateTime Created { get; } = DateTime.Now;
}
