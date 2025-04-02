using System;
using System.Collections.Generic;
using Godot;
[Tool]
public partial class CrankshaftVisuals : Node
{

    public Crankshaft main;
    public EngineController engine;

    [Export] private MeshInstance3D chankshaftMesh;
    [Export] private Node3D crankPinSpawnPoint;
    [Export] private PackedScene crankPinPrefab;
    [Export] private EngineSoundController soundController;

    [Export] public float crankshaftLength = 10f;
    [Export] public float cylindersPadding = 5f;


    [ExportGroup("Ui")]
    [Export] Slider throttleSlider;
    [Export] Label angularVelocityText;
    [Export] Label totalTorque;
    [Export] Label averageGasTemperature;

    [Export] Label exhaustFumesRatioBeforeCombustion;
    [Export] Label rpm;
    [Export] Label temperature;
    [Export] public CheckButton starterButton;
    [Export] Label gameFps;


    public float averageTemperature;
    public float averageTorque;
    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint())
            SpawnCrankPins();


        UpdateTextUI();
        UpdateCrankShaftAndPinsMeshes();
        base._Process(delta);
    }

    public void SpawnCrankPins()
    {
        crankPinSpawnPoint.RotationDegrees = new(0, 0, 0);
        foreach (Node3D node in crankPinSpawnPoint.GetChildren())
        {
            node.QueueFree();
        }

        foreach (Cylinder cylinder in engine.cylinders)
        {

            var node = crankPinPrefab.Instantiate(PackedScene.GenEditState.Instance);
            crankPinSpawnPoint.AddChild(node);
            var node3d = (Node3D)node;

            node3d.RotationDegrees = new(0, 0, cylinder.angleOffset);
            node3d.Scale = new(node3d.Scale.X, main.crankPinLength * engine.visualsScale, node3d.Scale.Z);
            node3d.Position = Vector3.Back * GetRelativeCylinderPlacement(cylinder.cylinderIndex).Z;
        }

    }

    public Vector3 GetRelativeCylinderPlacement(uint cylinderIndex)
    {
        var lengthPerCylinder = (crankshaftLength - cylindersPadding * 2) / (float)(engine.cylinders.Length - 1);
        return new(0, main.GetBottomDeadCentreHeight(), lengthPerCylinder * cylinderIndex + cylindersPadding);
    }

    private void UpdateTextUI()
    {
        // i just use one of cylinders so i don't heave to do any weird averaging, ratios should be similar in all cylinders
        exhaustFumesRatioBeforeCombustion.Text = $"exhaust fumes ratio in gas mixture before combustion {Math.Round(main.engine.cylinders[0].CurrentCombustionFumesAirRatio, 2)}";
        gameFps.Text = $"FPS {Engine.GetFramesPerSecond()}";
        rpm.Text = $"RPM: {Mathf.RoundToInt(main.RevolutionsPerSecond * 60f)}";
        throttleSlider.Value = engine.throttle;
        temperature.Text = $"Temperature: {Mathf.RoundToInt(engine.heatHandler.cylinderWallTemperature - 273)}C";

        angularVelocityText.Text = $"Angular velocity: {Mathf.RoundToInt(main.angularVelocityDeg)}";

        soundController.throttle = engine.overRPM ? 0 : engine.throttle;
        soundController.rpm = main.RevolutionsPerSecond * 60f;

        totalTorque.Text = $"torque: {(int)averageTorque}";
        averageGasTemperature.Text = $"average gas temperature: {(int)engine.cylinders[0].gasTemperatureInsideCylinder}";

    }
    private void UpdateCrankShaftAndPinsMeshes()
    {
        chankshaftMesh.Scale = new(1, crankshaftLength, 1);
        chankshaftMesh.RotationDegrees = new(90, main.shaftAngleDeg, 0);
        crankPinSpawnPoint.RotationDegrees = new(0, 0, main.shaftAngleDeg);
        chankshaftMesh.Position = new Vector3(0, 0, crankshaftLength / 2f);
    }
}
