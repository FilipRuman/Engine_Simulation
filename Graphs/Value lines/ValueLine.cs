using Godot;
using System.Collections.Generic;
[Tool]
public partial class ValueLine : Control {
    [Export] public Label topLabel;
    [Export] public Label bottomLabel;

    [Export] public Panel line;
    [Export] public bool horizontal;

    public void SetLineSize(float size) {
        if (line == null)
            return;

        if (horizontal)
            line.Size = new(size, line.Size.Y);
        else
            line.Size = new(line.Size.X, size);
    }

    public void SetText(string text) {
        topLabel.Text = text;
        bottomLabel.Text = text;

    }
}
