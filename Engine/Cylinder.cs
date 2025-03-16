using Godot;
[Tool, GlobalClass]
public partial class Cylinder : Node3D
{
    [Export] private MeshInstance3D gasInsideCylinder;
    [Export] private MeshInstance3D piston;
    [Export] private Crankshaft crankshaft;
    [Export(PropertyHint.Range, "0,100,")] public uint cylinderIndex = 0;
    [Export] public float angleOffset;
    [ExportGroup("")]
    [Export(PropertyHint.Range, "0,1,")] private float pistonPosition;
    [ExportGroup("engine size (cm^3)")]
    [Export] private float bore;
    [Export] private float stroke;
    [Export] private float additionalUpwardHeight;
    [Export] private float engineDisplacement;

    [ExportGroup("piston settings")]
    [Export] private float pistonHeight;

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint())
        {
            stroke = crankshaft.GetStroke();
            Position = crankshaft.GetRelativeCylinderPlacement(cylinderIndex);
            CalculateDisplacement();
        }
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
        pistonPosition = (crankshaft.GetPistonPositionAtAngle(crankshaft.shaftAngle + angleOffset) - Position.Y) / stroke;
        piston.Position = new(0, stroke * pistonPosition - pistonHeight / 2f, 0);

        var height = stroke + additionalUpwardHeight - stroke * pistonPosition;
        gasInsideCylinder.Position = new(0, stroke + additionalUpwardHeight - height / 2f, 0);
        gasInsideCylinder.Scale = new(bore, height, bore);
    }
    public float CalcualteTorque(float linearForce)
    {
        //TODO
        return 0;
    }


    public override void _Ready()
    {
        base._Ready();
    }
}
