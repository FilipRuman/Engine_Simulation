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
    [Export] private StrokeType currentStorkeTyoe;
    [ExportGroup("engine size (cm^3)")]
    [Export] private float bore;
    [Export] private float stroke;
    [Export] private float additionalUpwardHeight;
    [Export] private float engineDisplacement;


    [ExportGroup("piston settings")]
    [Export] private float pistonHeight;

    public float CurrentAngleDegrees => angleOffset + crankshaft.shaftAngle;
    public StrokeType GetCurrentStrokeType()
    {
        return (StrokeType)(Mathf.FloorToInt((CurrentAngleDegrees + 180) / 180f) % 4);
    }
    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint())
        {
            stroke = crankshaft.GetStroke();
            Position = crankshaft.GetRelativeCylinderPlacement(cylinderIndex);
            CalculateDisplacement();
        }
        currentTorque = CalculateTorque(1);
        currentStorkeTyoe = GetCurrentStrokeType();

        UpdateMeshes();

        base._Process(delta);
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
        material.SetShaderParameter("strokeIndex", (int)GetCurrentStrokeType());
    }
    public float CalculateTorque(float linearForce)
    {
        //https://en.wikipedia.org/wiki/Torque#Definition_and_relation_to_other_physical_quantities

        return crankshaft.crankPinLength * linearForce * Mathf.Sin(Mathf.DegToRad(CurrentAngleDegrees));
    }


    public override void _Ready()
    {
        base._Ready();
    }
}
