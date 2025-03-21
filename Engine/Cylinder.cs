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

    [Export] private MeshInstance3D gasInsideCylinder;
    [Export] private MeshInstance3D piston;
    [Export] private Crankshaft crankshaft;
    [Export(PropertyHint.Range, "0,100,")] public uint cylinderIndex = 0;
    [Export] public float angleOffset;
    [ExportGroup("Current values")]
    [Export(PropertyHint.Range, "0,1,")] private float pistonPosition;
    [Export] private float currentTorque;
    [Export] private StrokeType currentStorkeType;
    [ExportGroup("engine size (cm^3)")]
    /// m
    [Export] private float bore;
    ///m
    [Export] private float stroke;
    /// m
    [Export] private float additionalUpwardHeight;
    /// m^3
    [Export] private float engineDisplacement;


    [ExportGroup("piston settings")]
    [Export] private float pistonHeight;

    public float CurrentAngleDegrees => angleOffset + crankshaft.shaftAngleDeg;
    public StrokeType CurrentStrokeType => (StrokeType)(Mathf.FloorToInt((CurrentAngleDegrees + 180) / 180f) % 4);
    public float CurrentGasVolume => bore * (stroke * (1 - pistonPosition) + additionalUpwardHeight);
    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint())
        {
            currentTorque = GetCurrentTorque();
            stroke = crankshaft.GetStroke();
            Position = crankshaft.GetRelativeCylinderPlacement(cylinderIndex);
            CalculateDisplacement();
            currentTorque = CalculateTorque(1);
            currentStorkeType = CurrentStrokeType;
        }

        UpdateMeshes();

        base._Process(delta);
    }

    //TEMP:
    [ExportGroup("Combustion settings")]
    [Export] float combustionAirDensityModifier = 1;
    [Export] float combustionTemperature = 1;

    //let's say that the gasoline is C8H18 - Octane  https://en.wikipedia.org/wiki/Octane
    // you shouldn't do half's of mol but it doesn't really matter
    // so reaction of burning it as gas goes like this:  12.5(O2) + C8H18 -> 8(CO2) + 9(H2O)
    // so per 1 mole of gas we need 12.5 moles of oxygen
    // molar mass of Octane - 114.23 g/mol
    // molar mass of oxygen - 32 g/mol
    // so ideally for 1g of oxygen we need 0,285575g of Octane

    const float AirOxygenRatio = .21f;

    const float AirNitrogenRatio = .79f;

    const float OxygenFuelRatio = .285575f;
    const float HeatFormOctaneCombustion = 44.427f; // kj/g

    //mas change:
    // Substrates mas: 12.5 * 32(O2) + 114.23(C8H18) = 514.23g
    // Products mas: 8*44(CO2)+ 9*(34) = 658

    const float burnMasChange = 1.2795830659f;

    private static float IdealOctaneBurnGramsFromAirGrams(float airG, out float oxygenG)
    {
        oxygenG = airG * AirOxygenRatio;
        return oxygenG * OxygenFuelRatio;
    }


    //WARN: I'm rounding those values so calculations might be less accurate but for now I don't care

    // So lets say that we have 100g of Air
    // Only 21g of it is oxygen and 79g is nitrogen 
    // we add 5.997 Of fuel
    // then we burn about 27g of oxygen and fuel and we get: about  1.27 times that
    // so 100 -21 + (21 + 6) * 1.28 = 113,545g is total mass of exhaust fumes in cylinder   
    //
    // and we have (21 + 6) * 1.28 = 34.545g of mixture of CO2 and H2O
    // ratio of CO2 = ( 8*44 ) / 658  = 0.535; and that's 18,482g 
    // so we have 16,063g of H2O


    const float ExhaustFumesN2Ratio = 0.696f; //79 /113.545g
    const float ExhaustFumesCO2Ratio = 0.163f; //18.482 /113.545g
    const float ExhaustFumesH2ORatio = 0.141f;//16.063 /113.545g
    /// <summary> 
    /// WARN: this is not the most accurate because I assume that the burning process happens in 1 frame but that's not true at all
    ///
    /// calculates how much fuel you can burn using IdealOctaneBurnGramsFromAirGrams()
    ///  then calculates how temperature && mas will change and applies it
    /// </summary>
    private void BurnCurrentAir()
    {
        var fuelMassG = IdealOctaneBurnGramsFromAirGrams(gasMasInsideCylinder, out float oxygenG);
        gasMasInsideCylinder += fuelMassG;

        var gasMasChange = (burnMasChange - 1) * (fuelMassG + oxygenG);
        gasMasInsideCylinder += gasMasChange;

        var N2Mass = gasMasInsideCylinder * ExhaustFumesN2Ratio;
        var CO2Mass = gasMasInsideCylinder * ExhaustFumesCO2Ratio;
        var H2OMass = gasMasInsideCylinder * ExhaustFumesH2ORatio;

        var heatKJ = HeatFormOctaneCombustion * fuelMassG;
        var specificHeat = SpecificHeat.GetSpecificHeatOfExhaustFumes(gasTemperatureInsideCylinder, gasMasInsideCylinder, CO2Mass, H2OMass, N2Mass);
        var temperatureChange = heatKJ / (gasMasInsideCylinder / 1000 * specificHeat); // it doesn't matter it this is in Kelvins or Celsius i think.....

    }

    public bool fuelIsBurned = false;
    public bool exhaustedGas = false;

    public void UpdateCurrentConditionsInsideCylinder()
    {
        var strokeType = CurrentStrokeType;
        if (strokeType == StrokeType.Combustion && !fuelIsBurned)
        {
            fuelIsBurned = true;
            exhaustedGas = false;
            BurnCurrentAir();
        } // TODO: Make amount of exhaust gasses and air change over time
        else if (strokeType == StrokeType.Exhaust && !exhaustedGas)
        {
            exhaustedGas = true;
            fuelIsBurned = false;
            gasTemperatureInsideCylinder = ambientAirTemperature;

            // WARN: This is not ideal because cylinder is moving and not whole cylinder will be filled with air
            // Also some of the 
            var maxVolume = bore * (stroke + additionalUpwardHeight);
            gasMasInsideCylinder = ambientAirDensity * maxVolume;
        }
    }



    const float GasConstant = 8.314f;
    const float ambientAirDensity = 1.225f; // kg/m3
    const float ambientAirTemperature = 288.15f; // Kelvins

    float gasMasInsideCylinder = 1000;  // grams
    float gasTemperatureInsideCylinder = 10000; //Kelvins
    private float GetCurrentForce()
    {
        float mas = gasMasInsideCylinder;
        float temperature = gasTemperatureInsideCylinder;
        float volume = CurrentGasVolume;
        float pressure = mas * GasConstant * temperature / volume;
        float radius = bore / 2f;
        float area = Mathf.Pi * radius * radius;
        float force = area * pressure;

        return force;
    }
    public float GetCurrentTorque()
    {
        return CalculateTorque(GetCurrentForce());
    }

    private void CalculateDisplacement()
    {
        var radius = bore / 2;
        engineDisplacement = Mathf.Pi * radius * radius * (stroke + additionalUpwardHeight);
    }


    private void UpdateMeshes()
    {
        if (Engine.IsEditorHint())
        {
            piston.Scale = new(bore, pistonHeight, bore);

        }

        pistonPosition = (crankshaft.GetPistonPositionAtAngle(CurrentAngleDegrees) - Position.Y) / stroke;
        piston.Position = new(0, stroke * pistonPosition - pistonHeight / 2f, 0);

        var height = stroke + additionalUpwardHeight - stroke * pistonPosition;
        gasInsideCylinder.Position = new(0, stroke + additionalUpwardHeight - height / 2f, 0);
        gasInsideCylinder.Scale = new(bore, height, bore);

        var material = (ShaderMaterial)gasInsideCylinder.GetSurfaceOverrideMaterial(0);
        material.SetShaderParameter("strokeIndex", (int)CurrentStrokeType);
    }

    float CalculateTorque(float linearForce)
    {
        //TODO: Use one from this paper's first part https://crimsonpublishers.com/eme/pdf/EME.000582.pdf         
        return crankshaft.crankPinLength * linearForce * Mathf.Sin(Mathf.DegToRad(CurrentAngleDegrees));
    }


    public override void _Ready()
    {
        base._Ready();
    }
}
