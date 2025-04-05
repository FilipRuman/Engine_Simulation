using System;
using System.Collections.Generic;
using Godot;
[Tool]
public partial class Crankshaft : Node3D
{
    [ExportGroup("References")]
    [Export] public CrankshaftVisuals visuals;


    [Export] public EngineController engine;
    [ExportGroup("Angle relatedStuff")]

    [Export] public float shaftAngleDeg = 0;
    [Export] public float angularVelocityDeg = 1;

    public float RevolutionsPerSecond => Mathf.DegToRad(angularVelocityDeg) / Mathf.Tau;

    [ExportGroup("piston connection settings")]
    [Export] private float rodLengthCm = 0;
    [Export] private float crankPinLengthCm = 0;

    public float rodLength => rodLengthCm / 100f;
    public float crankPinLength => crankPinLengthCm / 100f;

    public void UpdateCrankshaftStatsBasedOnDrivetrain(float linearVelocity, float whealRadius, float gearRatio, float delta)
    {
        float angularVelocityRad = linearVelocity / whealRadius * gearRatio;
        angularVelocityDeg = Mathf.RadToDeg(angularVelocityRad);
        shaftAngleDeg += angularVelocityDeg * delta;
    }

    [Export] bool statisticsSomething = true;
    public override void _Process(double delta)
    {
        if (statisticsSomething)
            HandleStatisticsSmoothing();

        base._Process(delta);
    }
    public override void _Ready()
    {

        if (visuals == null)
            return;

        visuals.main = this;
        visuals.engine = engine;
        visuals.SpawnCrankPins();
        base._Ready();
    }

    public int currentSmoothingFrame = 0;

    private void HandleStatisticsSmoothing()
    {
        currentSmoothingFrame++;
        if (currentSmoothingFrame < averageSmoothingFrames)
            return;
        currentSmoothingFrame = 0;


        visuals.averageTemperature = CalculateAverageTemeperature();
        visuals.averageTorque = CalculateAverageTorque();
    }

    [Export] public int averageSmoothingFrames;
    public List<float> torques = new();
    public List<float> temperatures = new();

    private float CalculateAverageTemeperature()
    {
        float sum = 0;
        foreach (float temperature in temperatures)
        {
            sum += temperature;
        }
        int count = temperatures.Count;
        temperatures.Clear();
        return sum / (float)count;
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
