using System;
using System.Collections.Generic;
using Godot;
[Tool]
public partial class Crankshaft : Node3D
{

    [ExportGroup("References")]
    [Export] public CrankshaftVisuals visuals;
    [Export] public AirFlow airFlow;
    [Export] public Cylinder[] cylinders;

    [ExportGroup("Angle relatedStuff")]

    [Export] public float shaftAngleDeg = 0;
    [Export] public float angularVelocityDeg = 1;

    public float RevolutionsPerSecond => Mathf.DegToRad(angularVelocityDeg) / Mathf.Tau;

    [ExportGroup("piston connection settings")]
    [Export] private float rodLengthCm = 0;
    [Export] private float crankPinLengthCm = 0;

    public float rodLength => rodLengthCm / 100f;
    public float crankPinLength => crankPinLength / 100f;

    [ExportGroup("Drag and torque")]
    [Export(PropertyHint.Range, "0,1,")] public float throttle;
    [Export] private float starterTorque;
    [Export] private float mechanicalDragModifier;
    [Export] public float pressureChangeFrictionModifier = 3;

    public void UpdateCrankshaftStatsBasedOnDrivetrain(float linearVelocity, float whealRadius, float gearRatio, float delta)
    {
        float angularVelocityRad = linearVelocity / whealRadius * gearRatio;
        angularVelocityDeg = Mathf.RadToDeg(angularVelocityRad);
        shaftAngleDeg += angularVelocityDeg * delta;
    }

    public override void _Process(double delta)
    {
        HandleStatisticsSmoothing();

        base._Process(delta);
    }

    public int currentSmoothingFrame = 0;
    public float averageTorque;
    private void HandleStatisticsSmoothing()
    {
        currentSmoothingFrame++;
        if (currentSmoothingFrame < averageSmoothingFrames)
            return;
        currentSmoothingFrame = 0;

        visuals.averageTorque = CalculateAverageTorque();
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
        //TODO: change that to keybinding
        if (visuals.starterButton.ButtonPressed)
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


    public override void _Ready()
    {
        airFlow.sampleCylinder = cylinders[0];
        foreach (Cylinder cylinder in cylinders)
        {
            cylinder.currentPressure = 0;
        }
        base._Ready();
    }
    //https://en.wikipedia.org/wiki/Piston_motion_equations
    public float GetPistonPositionAtAngle(float angleInDegrees)
    {
        var angleInRads = Mathf.DegToRad(angleInDegrees);
        return crankPinLength * Mathf.Cos(angleInRads) + Mathf.Sqrt(rodLength * rodLength - crankPinLength * crankPinLength * Mathf.Sin(angleInRads) * Mathf.Sin(angleInRads));
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

}
