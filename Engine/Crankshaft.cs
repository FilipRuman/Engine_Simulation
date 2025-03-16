using System;
using Godot;
[Tool]
public partial class Crankshaft : Node3D
{
    [Export] private MeshInstance3D mesh;
    /// from top dead center
    [Export] public float crankAngle = 0;
    [Export] public float crankshaftLength = 10f;
    [Export] public float cylindersPadding = 5f;
    [Export(PropertyHint.Range, "1,100,")] public uint cylindersCount = 1;

    [ExportGroup("piston connection settings")]
    [Export] public float rodLength = 0;
    [Export] public float crankRadius = 0;
    [Export] public float pistonPinPosition = 0;

    public override void _Process(double delta)
    {
        mesh.Scale = new(1, crankshaftLength, 1);
        mesh.RotationDegrees = new(90, crankAngle, 0);
        mesh.Position = new Vector3(0, 0, crankshaftLength / 2f);
        base._Process(delta);
    }


    //https://en.wikipedia.org/wiki/Piston_motion_equations
    public float GetPistonPositionAtAngle(float angle)
    {
        return crankRadius * Mathf.Cos(angle) + Mathf.Sqrt(rodLength * rodLength - crankRadius * crankRadius * Mathf.Sin(angle) * Mathf.Sin(angle));
    }
    public float GetStroke()
    {
        return GetTopDeadCentreHeight() - GetBottomDeadCentreHeight();
    }
    public float GetBottomDeadCentreHeight()
    {
        return GetPistonPositionAtAngle(90);
    }
    public float GetTopDeadCentreHeight()
    {
        return GetPistonPositionAtAngle(0);
    }

    public Vector3 GetRelativeCylinderPlacement(uint cylinderIndex)
    {
        return new(0, GetBottomDeadCentreHeight(), ((crankshaftLength - cylindersPadding * 2) / cylinderIndex) + cylindersPadding);
    }
}
