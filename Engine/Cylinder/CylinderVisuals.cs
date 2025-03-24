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


        piston.Position = new(0, main.strokeLength * main.pistonPosition - main.pistonHeight / 2f, 0);

        var height = main.strokeLength + main.additionalUpwardHeight - main.strokeLength * main.pistonPosition;
        gasInsideCylinder.Position = new(0, main.strokeLength + main.additionalUpwardHeight - height / 2f, 0);
        gasInsideCylinder.Scale = new(main.bore, height, main.bore);

        var material = (ShaderMaterial)gasInsideCylinder.GetSurfaceOverrideMaterial(0);
        material.SetShaderParameter("pressure", main.currentPressure);
    }
}
