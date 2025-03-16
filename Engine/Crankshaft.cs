using System;
using System.Collections.Generic;
using Godot;
[Tool]
public partial class Crankshaft : Node3D
{
    [Export] private MeshInstance3D mesh;

    [Export] private bool reSpawnCrankPins;
    [Export] private Node3D crankPinSpawnPoint;
    [Export] private PackedScene crankPinPrefab;
    /// from top dead center
    [Export] public float shaftAngle = 0;
    [Export] public float crankshaftLength = 10f;
    [Export] public float cylindersPadding = 5f;
    [Export] public Cylinder[] cylinders;

    [ExportGroup("piston connection settings")]
    [Export] public float rodLength = 0;
    [Export] public float crankPinLength = 0;

    public override void _Process(double delta)
    {
        //TODO: make that only in editor
        SpawnCrankPins();

        mesh.Scale = new(1, crankshaftLength, 1);
        mesh.RotationDegrees = new(90, shaftAngle, 0);
        crankPinSpawnPoint.RotationDegrees = new(90, shaftAngle, 0);
        mesh.Position = new Vector3(0, 0, crankshaftLength / 2f);
        base._Process(delta);
    }
    public override void _Ready()
    {
        SpawnCrankPins();
        base._Ready();
    }

    public void SpawnCrankPins()
    {
        crankPinSpawnPoint.RotationDegrees = new(90, 0, 0);
        foreach (Node3D node in crankPinSpawnPoint.GetChildren())
        {
            node.QueueFree();
        }


        foreach (Cylinder cylinder in cylinders)
        {

            var node = crankPinPrefab.Instantiate(PackedScene.GenEditState.Instance);
            crankPinSpawnPoint.AddChild(node);
            var node3d = (Node3D)node;

            node3d.RotationDegrees = new(-cylinder.angleOffset, 90, 90);
            node3d.Scale = new(node3d.Scale.X, crankPinLength, node3d.Scale.Z);
            node3d.GlobalPosition = Vector3.Back * GetRelativeCylinderPlacement(cylinder.cylinderIndex).Z + node3d.Quaternion * Vector3.Right * crankPinLength / 2f;
        }

    }
    //https://en.wikipedia.org/wiki/Piston_motion_equations
    public float GetPistonPositionAtAngle(float angle)
    {
        angle = Mathf.DegToRad(angle);
        return crankPinLength * Mathf.Cos(angle) + Mathf.Sqrt(rodLength * rodLength - crankPinLength * crankPinLength * Mathf.Sin(angle) * Mathf.Sin(angle));
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
        var lengthPerCylinder = (crankshaftLength - cylindersPadding * 2) / (float)(cylinders.Length - 1);
        return new(0, GetBottomDeadCentreHeight(), lengthPerCylinder * cylinderIndex + cylindersPadding);
    }
}
