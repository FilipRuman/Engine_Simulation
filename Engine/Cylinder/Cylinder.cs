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
    [Export] AirFlow airFlow;
    [Export] CylinderVisuals visuals;

    [Export] public Crankshaft crankshaft;
    [Export(PropertyHint.Range, "0,100,")] public uint cylinderIndex = 0;
    [Export] public float angleOffset;
    [ExportGroup("Current values")]
    [Export(PropertyHint.Range, "0,1,")] public float pistonPosition;
    [Export] private float currentTorque;
    [Export] private StrokeType currentStorkeType;
    [ExportGroup("engine size (cm^3)")]
    /// m
    [Export] public float bore;
    ///m
    [Export] public float strokeLength;
    /// m
    [Export] public float additionalUpwardHeight;
    /// m^3
    [Export] private float engineDisplacement;


    [ExportGroup("piston settings")]
    [Export] public float pistonHeight;

    public float CurrentAngleDegrees => angleOffset + crankshaft.shaftAngleDeg;
    public StrokeType CurrentStrokeType => (StrokeType)(Mathf.FloorToInt((CurrentAngleDegrees + 180) / 180f) % 4);
    public float CurrentGasVolume => bore * (strokeLength * (1 - pistonPosition) + additionalUpwardHeight);



    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint())
        {
            currentTorque = GetCurrentTorque();
            strokeLength = crankshaft.GetStroke();
            Position = crankshaft.GetRelativeCylinderPlacement(cylinderIndex);
            CalculateDisplacement();
            currentTorque = CalculateTorque(1);
            currentStorkeType = CurrentStrokeType;
        }

        pistonPosition = (crankshaft.GetPistonPositionAtAngle(CurrentAngleDegrees) - Position.Y) / strokeLength;
        visuals.UpdateMeshes();

        base._Process(delta);
    }
    private bool fuelIsBurned;
    private bool exhaustedGas;

    public void UpdateCurrentConditionsInsideCylinder(float deltaTime)
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
                gasMasInsideCylinder = airFlow.CalculateMassFlowOfAir(deltaTime);

                break;
            case StrokeType.Compression:

                break;


        }
    }



    public float gasMasInsideCylinder = 1000;  // grams
    public float gasTemperatureInsideCylinder = 10000; //Kelvins
    private float GetCurrentForce()
    {
        float mas = gasMasInsideCylinder;
        float temperature = gasTemperatureInsideCylinder;
        float volume = CurrentGasVolume;
        float absolutePressure = mas * Combustion.GasConstant * temperature / volume;

        float radius = bore / 2f;
        float area = Mathf.Pi * radius * radius;
        float force = area * absolutePressure;

        return force;
    }
    public float GetCurrentTorque()
    {
        return CalculateTorque(GetCurrentForce());
    }

    private void CalculateDisplacement()
    {
        var radius = bore / 2;
        engineDisplacement = Mathf.Pi * radius * radius * (strokeLength + additionalUpwardHeight);
    }



    float CalculateTorque(float linearForce)
    {
        //TODO: Use one from this paper's first part https://crimsonpublishers.com/eme/pdf/EME.000582.pdf         
        return crankshaft.crankPinLength * linearForce * Mathf.Sin(Mathf.DegToRad(CurrentAngleDegrees));
    }


    public override void _Ready()
    {
        combustion.main = this;
        base._Ready();
    }
}
