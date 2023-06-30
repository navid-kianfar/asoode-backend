namespace Asoode.Shared.Abstraction.Dtos.General;

public record SelectableItem<T>
{
    public object Payload { get; set; }

    // public bool? Disabled { get; set; }
    // public string Icon { get; set; }
    // public string Image { get; set; }
    // public bool? Selected { get; set; }
    public string Text { get; set; }
    public T Value { get; set; }
}