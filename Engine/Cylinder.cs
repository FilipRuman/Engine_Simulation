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
    public void UpdateCurrentConditionsInsideCylinder()
    {

        // TODO:
        gasMas = ambientAirDensity * (CurrentStrokeType == StrokeType.Combustion ? combustionAirDensityModifier : 1);
        airTemperature = CurrentStrokeType == StrokeType.Combustion ? combustionTemperature : ambientAirTemperature;

    }



    const float GasConstant = 8.314f;
    const float ambientAirDensity = 1.225f; // kg/m3
    const float ambientAirTemperature = 288.15f; // Kelvins

    float gasMas = 1000; // kg/m3
    float airTemperature = 10000; //Kelvins
    private float GetCurrentForce()
    {
        float mas = gasMas;
        float temperature = airTemperature;
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
