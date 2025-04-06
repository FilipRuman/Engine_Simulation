using Godot;
using System.Collections.Generic;
[GlobalClass, Tool]
public partial class DataGroup : Resource {
    [Export] public Color color;
    [Export] public string name;
    public List<float> data = new();
    public List<Control> points = new();
}
