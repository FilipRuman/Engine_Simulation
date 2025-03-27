using Godot;
using System.Collections.Generic;
[Tool]
public partial class Charts : Node
{
    [Export] private Control spawnPoint;
    [Export] private Vector2 canvasScale;
    [Export] private int maxPointsCount;
    private List<float> points = new();
    [Export] private PackedScene pointScene;
    public override void _Ready()
    {
        base._Ready();
    }
    public void AddPointToChart(float torque)
    {
        points.Add(torque);
        if (points.Count > maxPointsCount)
            points.RemoveAt(0);
    }

    [Export] float drawOffset;
    float currentTime;
    public override void _Process(double delta)
    {
        currentTime += (float)delta;
        if (currentTime < drawOffset)
        {
            base._Process(delta);
            return;
        }
        currentTime = 0;
        DrawAllPoints();
        base._Process(delta);
    }

    public void DrawAllPoints()
    {
        foreach (Node node in spawnPoint.GetChildren())
        {
            node.QueueFree();
        }
        for (int x = 0; x < points.Count; x++)
        {
            var node = (Control)pointScene.Instantiate();
            spawnPoint.AddChild(node);
            node.Position = new Vector2((float)x, points[x] / 1000f) * canvasScale;
        }


    }

}
