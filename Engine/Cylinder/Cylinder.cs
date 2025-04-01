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
    public AirFlow airFlow;
    public EngineController engine;
    public Crankshaft crankshaft;


    [Export] CylinderVisuals visuals;

    [Export(PropertyHint.Range, "0,100,")] public uint cylinderIndex = 0;
    [Export] public float angleOffset;
    [Export(PropertyHint.Range, "0,1,")] public float pistonPosition;
    [Export] private float currentTorque;
    [Export] private StrokeType currentStrokeType;


    public float CurrentAngleDegrees => angleOffset + crankshaft.shaftAngleDeg;
    public StrokeType CurrentStrokeType => (StrokeType)(Mathf.FloorToInt((CurrentAngleDegrees + 180) / 180f) % 4);
    public float CurrentGasVolume => engine.bore * (engine.strokeLength * (1 - pistonPosition) + engine.additionalUpwardHeight);



    public override void _Process(double delta)
    {
        if (crankshaft == null)
        {
            GD.Print("cramlshaft == null");
            return;
        }

        if (Engine.IsEditorHint())
        {
            Position = crankshaft.visuals.GetRelativeCylinderPlacement(cylinderIndex);
            currentStrokeType = CurrentStrokeType;
        }
        pistonPosition = (crankshaft.GetPistonPositionAtAngle(CurrentAngleDegrees) - Position.Y) / engine.strokeLength;

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
                gasMasInsideCylinder += airFlow.CalculateMassFlowOfAir(deltaTime);

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
        work = averagePressure * volumeChange - (pressureChange * engine.pressureChangeFrictionModifier);

        //TODO:
        float heatFormCombustion = 0;

        float internalEnergyChange = heatFormCombustion - work;
        float specificHeat = SpecificHeat.GetCurrentSpecificHeat(this);
        float temperatureChange = internalEnergyChange / (gasMasInsideCylinder * specificHeat);

        lastVolume = currentVolume;
        lastPressure = currentPressure;
    }


    public float gasMasInsideCylinder = 0;  // Kg
    public float gasTemperatureInsideCylinder = 10000; //Kelvins

    const float AtmospherePressure = 101325f; //Pa
    public float currentPressure;


    public void Start()
    {
        combustion.main = this;
        visuals.engine = engine;
        visuals.main = this;
        visuals.UpdateMeshes();
    }
}
