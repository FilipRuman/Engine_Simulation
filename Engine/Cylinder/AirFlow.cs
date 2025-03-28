using System;
using Godot;
[Tool]
public partial class AirFlow : Node
{
    public EngineController engine;
    public Crankshaft crankshaft;

    [Export] private float MinValveLift;
    [Export] private float MaxValveLift;
    [Export] private float throttleDiameter;
    private float ValveLift => Mathf.Lerp(MinValveLift, MaxValveLift, engine.throttle);
    // also i think it is the same as curtain area Ac
    private float CurrentEffectiveFlowArea => Mathf.Pi * throttleDiameter * ValveLift;

    // i don't think there is any way to calculate it
    [Export(PropertyHint.Range, "0.3,0.6,")] private float intakeVelocityModifier = .5f;

    private float AverageIntakeVelocity => 2 * engine.strokeLength * crankshaft.RevolutionsPerSecond * intakeVelocityModifier;


    [Export(PropertyHint.Range, "0.4,0.8,")] private float flowEfficiency = .5f;
    private float VolumetricFlowRate => CurrentEffectiveFlowArea * AverageIntakeVelocity * flowEfficiency;

    public float CalculateMassFlowOfAir(float deltaTime)
    {
        float gasDensity = Combustion.ambientAirDensity;
        return gasDensity * VolumetricFlowRate * deltaTime;
    }

    // public float CalculateMassFlowOfGas(float specificGasConstant, float temperature, float deltaTime, float absoultePressure)
    // {
    //     float gasDensity = absoultePressure / (temperature * specificGasConstant);
    //     return gasDensity * VolumetricFlowRate * deltaTime;
    // }
}
