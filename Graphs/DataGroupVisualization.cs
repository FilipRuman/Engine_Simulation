using Godot;
using System;
[Tool]
public partial class DataGroupVisualization : Node {
    [Export] TextureRect textureRect;
    [Export] Label label;

    public void Setup(Color color, string text) {
        label.Text = text;
        textureRect.Modulate = color;
    }
}
