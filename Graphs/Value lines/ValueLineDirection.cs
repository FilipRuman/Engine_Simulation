using Godot;
using System.Collections.Generic;
[Tool]
public partial class ValueLineDirection : Node {

    [Export] public Control layout;
    [Export] public PackedScene line;
    [Export] public PackedScene endLine;

    [Export] public uint linesCount;

    public List<ValueLine> linesList = new();


    public void ClearLines() {
        foreach (ValueLine valueLine in layout.GetChildren()) {
            valueLine.QueueFree();
        }
        linesList.Clear();

    }
}



