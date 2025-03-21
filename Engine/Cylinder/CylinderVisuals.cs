using Godot;
[Tool, GlobalClass]
public partial class CylinderVisuals : Node3D
{
    [Export] Cylinder main;
    [Export] private MeshInstance3D gasInsideCylinder;
    [Export] private MeshInstance3D piston;
    public void UpdateMeshes()
    {
        if (Engine.IsEditorHint())
        {
            piston.Scale = new(main.bore, main.pistonHeight, main.bore);

        }


        var height = main.stroke + main.additionalUpwardHeight - main.stroke * main.pistonPosition;
        gasInsideCylinder.Position = new(0, main.stroke + main.additionalUpwardHeight - height / 2f, 0);
        gasInsideCylinder.Scale = new(main.bore, height, main.bore);

        var material = (ShaderMaterial)gasInsideCylinder.GetSurfaceOverrideMaterial(0);
        material.SetShaderParameter("strokeIndex", (int)main.CurrentStrokeType);
    }
}
