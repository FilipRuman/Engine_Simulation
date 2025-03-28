using Godot;
[Tool, GlobalClass]
public partial class Cylinder : Node3D
{
    public enum StrokeType
    {
        Exhaust,
        Intake,
        Compression,
        Combustion
    }

    public Combustion combustion = new();
    [Export] CylinderVisuals visuals;

    [Export] public Crankshaft crankshaft;
    [Export(PropertyHint.Range, "0,100,")] public uint cylinderIndex = 0;
    [Export] public float angleOffset;
    [ExportGroup("Current values")]
    [Export(PropertyHint.Range, "0,1,")] public float pistonPosition;
    [Export] private float currentTorque;
    [Export] private StrokeType currentStrokeType;
    [ExportGroup("engine size (m^3)")]
    [Export] private float boreDm;
    [Export] private float displacementDm;

    public float bore => boreDm / 1000f;
    /// m^3
    [Export] public float strokeLength;
    /// m^3
    [Export] public float additionalUpwardHeight;
    /// m^3
    float displacement;

    [ExportGroup("piston settings")]
    [Export] public float pistonHeight;

    public float CurrentAngleDegrees => angleOffset + crankshaft.shaftAngleDeg;
    public StrokeType CurrentStrokeType => (StrokeType)(Mathf.FloorToInt((CurrentAngleDegrees + 180) / 180f) % 4);
    public float CurrentGasVolume => bore * (strokeLength * (1 - pistonPosition) + additionalUpwardHeight);



    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint())
        {
            strokeLength = crankshaft.GetStrokeM();
            Position = crankshaft.GetRelativeCylinderPlacement(cylinderIndex);
            CalculateDisplacement();
            currentStrokeType = CurrentStrokeType;
        }

        pistonPosition = (crankshaft.floatDm(CurrentAngleDegrees) - Position.Y) / strokeLength;
        visuals.UpdateMeshes();

        base._Process(delta);
    }
    private bool fuelIsBurned;
    private bool exhaustedGas;

    public void UpdateCurrentConditionsInsideCylinder(float deltaTime, float deltaAngleDegrees, out float torque)
    {
        switch (CurrentStrokeType)
        {
            case StrokeType.Combustion:
                if (!fuelIsBurned)
                {
                    fuelIsBurned = true;
                    exhaustedGas = false;
                    combustion.BurnCurrentAir();
                }
                break;
            case StrokeType.Exhaust:
                if (!exhaustedGas)
                {
                    //TODO: Make this better
                    exhaustedGas = true;
                    fuelIsBurned = false;

                    gasTemperatureInsideCylinder = Combustion.ambientAirTemperature;
                    gasMasInsideCylinder = 0;
                }
                break;
            case StrokeType.Intake:
                gasMasInsideCylinder += crankshaft.airFlow.CalculateMassFlowOfAir(deltaTime);

                break;
            case StrokeType.Compression:

                break;
        }
        //https://en.wikipedia.org/wiki/First_law_of_thermodynamics
        ApplyFirstLawOfThremodynamics(out float work);
        if (deltaAngleDegrees == 0)
            torque = 0;
        else
            torque = work / Mathf.DegToRad(deltaAngleDegrees);
    }



    private float lastVolume;
    private float lastPressure;
    private void ApplyFirstLawOfThremodynamics(out float work)
    {

        float currentVolume = CurrentGasVolume;
        currentPressure = gasMasInsideCylinder * Combustion.GasConstant * gasTemperatureInsideCylinder / currentVolume;

        float pressureChange = currentPressure - lastPressure;
        float averagePressure = (currentPressure + lastPressure) / 2f;
        float volumeChange = currentVolume - lastVolume;
        work = averagePressure * volumeChange - (pressureChange * crankshaft.pressureChangeFrictionModifier);

        //TODO:
        float heatFormCombustion = 0;

        float internalEnergyChange = heatFormCombustion - work;
        float specificHeat = SpecificHeat.GetCurrentSpecificHeat(this);
        float temperatureChange = internalEnergyChange / (gasMasInsideCylinder * specificHeat);

        lastVolume = currentVolume;
        lastPressure = currentPressure;
    }


    public float gasMasInsideCylinder = 1000;  // grams
    public float gasTemperatureInsideCylinder = 10000; //Kelvins

    const float AtmospherePressure = 101325f; //Pa
    public float currentPressure;

    private void CalculateDisplacement()
    {
        var radius = bore / 2;
        displacement = Mathf.Pi * radius * radius * (strokeLength + additionalUpwardHeight);
        displacementDm = displacement / 10000f;
    }

    public override void _Ready()
    {
        combustion.main = this;
        crankshaft.airFlow.sampleCylinder = this;
        base._Ready();
    }
}
