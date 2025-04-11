using System;
using System.Collections.Generic;
using Godot;
[Tool]
public partial class Crankshaft : Node3D {

    [Export] public CrankshaftVisuals visuals;
    [Export] public EngineController engine;


    public float shaftAngleDeg = 0;
    public float angularVelocityDeg = 1;
    public float RevolutionsPerSecond => Mathf.DegToRad(angularVelocityDeg) / Mathf.Tau;

    [ExportGroup("piston connection settings")]
    [Export] private float rodLengthCm = 0;
    [Export] private float crankPinLengthCm = 0;

    public float rodLength => rodLengthCm / 100f;
    public float crankPinLength => crankPinLengthCm / 100f;

    public void UpdateCrankshaftStatsBasedOnDrivetrain(float linearVelocity, float whealRadius, float gearRatio, float delta) {
        float angularVelocityRad = linearVelocity / whealRadius * gearRatio;
        angularVelocityDeg = Mathf.RadToDeg(angularVelocityRad);
        shaftAngleDeg += angularVelocityDeg * delta;
    }

    public override void _Ready() {

        if (visuals == null)
            return;

        visuals.main = this;
        visuals.engine = engine;
        visuals.SpawnCrankPins();
        base._Ready();
    }

    //https://en.wikipedia.org/wiki/Piston_motion_equations
    public float GetPistonPositionAtAngle(float angleInDegrees) {
        var angleInRads = Mathf.DegToRad(angleInDegrees);
        return crankPinLength * Mathf.Cos(angleInRads) + Mathf.Sqrt(rodLength * rodLength - crankPinLength * crankPinLength * Mathf.Sin(angleInRads) * Mathf.Sin(angleInRads));
    }


    public float GetStroke() {
        return GetTopDeadCentreHeight() - GetBottomDeadCentreHeight();
    }
    public float GetBottomDeadCentreHeight() {
        return GetPistonPositionAtAngle(90);
    }
    public float GetTopDeadCentreHeight() {
        return GetPistonPositionAtAngle(0);
    }

}
