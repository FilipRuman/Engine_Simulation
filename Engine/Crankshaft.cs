using System;
using System.Collections.Generic;
using Godot;
[Tool]
public partial class Crankshaft : Node3D
{

    [ExportGroup("References")]
    [Export] private MeshInstance3D mesh;
    [Export] public AirFlow airFlow;

    [Export] private Node3D crankPinSpawnPoint;
    [Export] private PackedScene crankPinPrefab;
    [Export] private EngineSoundController soundController;
    [Export] private Charts charts;
    /// from top dead center

    [ExportGroup("Angle relatedStuff")]
    //TODO: Calculate this
    [Export] public float momentOfInertia;

    [Export] public float shaftAngleDeg = 0;
    [Export] public float angularVelocityDeg = 1;

    public float RevolutionsPerSecond => Mathf.DegToRad(angularVelocityDeg) / Mathf.Tau;

    [ExportGroup("cylinder settings")]
    [Export] public float crankshaftLength = 10f;
    [Export] public float cylindersPadding = 5f;
    [Export] public Cylinder[] cylinders;

    [ExportGroup("piston connection settings")]
    [Export] public float rodLengthDm = 0;
    [Export] public float crankPinLengthDm = 0;


    [ExportGroup("Ui")]
    [Export] Slider throttleSlider;
    [Export] Label angularVelocityText;
    [Export] Label pressureInsideCylinder1;
    [Export] Label totalTorque;
    [Export] Label rpm;
    [Export] CheckButton starterButton;
    [Export] Label gameFps;



    [ExportGroup("Drag and torque")]
    [Export(PropertyHint.Range, "0,1,")] public float throttle;
    [Export] private float starterTorque;
    [Export] private float mechanicalDragModifier;
    [Export] public float pressureChangeFrictionModifier = 3;

    // public override void _PhysicsProcess(double delta)
    // {
    //     HandlePhysicsCalculateTorque((float)delta);
    //
    //     base._PhysicsProcess(delta);
    // }
    public void UpdateCrankshaftStatsBasedOnDrivetrain(float linearVelocity, float whealRadius, float gearRatio, float delta)
    {
        float angularVelocityRad = linearVelocity / whealRadius * gearRatio;
        angularVelocityDeg = Mathf.RadToDeg(angularVelocityRad);
        shaftAngleDeg += angularVelocityDeg * delta;
    }

    public override void _Process(double delta)
    {
        gameFps.Text = $"FPS {Engine.GetFramesPerSecond()}";
        rpm.Text = $"RPM: {Mathf.RoundToInt(RevolutionsPerSecond * 60f)}";
        throttle = (float)throttleSlider.Value;
        angularVelocityText.Text = $"Angular velocity: {Mathf.RoundToInt(angularVelocityDeg)}";
        pressureInsideCylinder1.Text = $"Pressure :\n{cylinders[0].currentPressure} \n {cylinders[1].currentPressure} \n {cylinders[2].currentPressure} \n {cylinders[3].currentPressure}";

        if (Engine.IsEditorHint())
            SpawnCrankPins();

        HandleStatisticsSmoothing();

        UpdateVisuals();
        soundController.throttle = throttle;
        soundController.rpm = RevolutionsPerSecond * 60f;


        base._Process(delta);
    }

    public int currentSmoothingFrame = 0;
    private void HandleStatisticsSmoothing()
    {
        currentSmoothingFrame++;
        if (currentSmoothingFrame < averageSmoothingFrames)
            return;
        currentSmoothingFrame = 0;
        float averageTorque = CalculateAverageTorque();
        totalTorque.Text = $"torque: {(int)averageTorque}";
        charts.AddPointToChart(averageTorque);
    }

    [Export] public int averageSmoothingFrames;
    List<float> torques = new();

    float lastAngleDeg;

    public float HandlePhysicsAndCalculateTorque(float delta)
    {
        //abs so engine doesn't run backwards
        float deltaAngle = Mathf.Abs(shaftAngleDeg - lastAngleDeg);
        lastAngleDeg = shaftAngleDeg;
        float torque = 0;
        foreach (Cylinder cylinder in cylinders)
        {
            cylinder.UpdateCurrentConditionsInsideCylinder(delta, deltaAngle, out float addTorque);
            torque += addTorque;
        }
        torque -= mechanicalDragModifier * delta * angularVelocityDeg;
        if (starterButton.ButtonPressed)
            torque += delta * starterTorque;

        torques.Add(torque);
        return torque;
    }
    private float CalculateAverageTorque()
    {
        float sum = 0;
        foreach (float torque in torques)
        {
            sum += torque;
        }
        int count = torques.Count;
        torques.Clear();
        return sum / (float)count;
    }

    private void UpdateVisuals()
    {

        mesh.Scale = new(1, crankshaftLength, 1);
        mesh.RotationDegrees = new(90, shaftAngleDeg, 0);
        crankPinSpawnPoint.RotationDegrees = new(0, 0, shaftAngleDeg);
        mesh.Position = new Vector3(0, 0, crankshaftLength / 2f);
    }


    public override void _Ready()
    {
        airFlow.sampleCylinder = cylinders[0];
        foreach (Cylinder cylinder in cylinders)
        {
            cylinder.currentPressure = 0;
        }
        SpawnCrankPins();
        base._Ready();
    }

    public void SpawnCrankPins()
    {
        crankPinSpawnPoint.RotationDegrees = new(0, 0, 0);
        foreach (Node3D node in crankPinSpawnPoint.GetChildren())
        {
            node.QueueFree();
        }



        foreach (Cylinder cylinder in cylinders)
        {

            var node = crankPinPrefab.Instantiate(PackedScene.GenEditState.Instance);
            crankPinSpawnPoint.AddChild(node);
            var node3d = (Node3D)node;

            node3d.RotationDegrees = new(0, 0, cylinder.angleOffset);
            node3d.Scale = new(node3d.Scale.X, crankPinLengthDm, node3d.Scale.Z);
            node3d.Position = Vector3.Back * GetRelativeCylinderPlacement(cylinder.cylinderIndex).Z;
        }

    }
    //https://en.wikipedia.org/wiki/Piston_motion_equations
    public float GetPistonPositionAtAngle(float angleInDegrees)
    {
        var angleInRads = Mathf.DegToRad(angleInDegrees);
        return crankPinLengthDm * Mathf.Cos(angleInRads) + Mathf.Sqrt(rodLengthDm * rodLengthDm - crankPinLengthDm * crankPinLengthDm * Mathf.Sin(angleInRads) * Mathf.Sin(angleInRads));
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
