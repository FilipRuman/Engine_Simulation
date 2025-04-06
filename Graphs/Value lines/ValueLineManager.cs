using Godot;
using System.Collections.Generic;
[Tool]
public partial class ValueLineManager : Node {
    [Export] ValueLineDirection horizontal;
    [Export] ValueLineDirection vertical;
    [Export] GraphMain main;

    [Export] private float lineThickness;
    public void ReSpawnAllLines() {
        horizontal.ClearLines();
        vertical.ClearLines();
        var direction = horizontal;
        for (uint i = 0; i < direction.linesCount; i++) {
            bool end = i == 0 || i == direction.linesCount - 1;
            SetupValueLine(i, horizontalDirection: true, end, direction);
        }

        direction = vertical;
        for (uint i = 0; i < direction.linesCount; i++) {
            bool end = i == 0 || i == direction.linesCount - 1;
            SetupValueLine(i, horizontalDirection: false, end, direction);
        }
    }

    private void SetupValueLine(uint index, bool horizontalDirection, bool end, ValueLineDirection direction) {

        var line = SpawnValueLine(horizontalDirection, end, direction);
        line.SetLineSize(lineThickness);


        float Value = main.ValueBasedOnPositionPercentage(1f / (direction.linesCount - 1) * index, horizontalDirection);

        line.SetText(((int)Value).ToString());

    }
    private ValueLine SpawnValueLine(bool horizontalDirection, bool end, ValueLineDirection direction) {

        var prefab = end ? direction.endLine : direction.line;

        var valueLine = (ValueLine)prefab.Instantiate();
        direction.layout.AddChild(valueLine);
        direction.linesList.Add(valueLine);
        return valueLine;
    }

}
