using Godot;
[Tool]
public partial class CylinderGraphics : Node3D
{

    [Export] private MeshInstance3D gasInsideCylinder;
    [Export] private MeshInstance3D piston;

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
            CalculateEngineDisplacement();
        }
        UpdateMeshes();

        base._Process(delta);
    }

    private void CalculateEngineDisplacement()
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

        piston.Position = new(0, stroke * pistonPosition - pistonHeight / 2f, 0);

        var height = stroke + additionalUpwardHeight - stroke * pistonPosition;
        gasInsideCylinder.Position = new(0, stroke + additionalUpwardHeight - height / 2f, 0);
        gasInsideCylinder.Scale = new(bore, height, bore);
    }

    public override void _Ready()
    {
        base._Ready();
    }
}
