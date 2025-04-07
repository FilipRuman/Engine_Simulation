using Godot;
public partial class ChassisVisuals : Node {

    [Export] ChassisMain main;
    [Export] Label gearText;
    [Export] Label linearVelocityText;
    [Export] Label dragText;

    public override void _Process(double delta) {
        if (gearText != null)
            gearText.Text = $"gear: {main.gear}";

        if (linearVelocityText != null)
            linearVelocityText.Text = $"velocity: {Mathf.RoundToInt(main.linearVelocity)}";

        if (dragText != null)
            dragText.Text = $"drag force: {Mathf.RoundToInt(main.currentDragForce)}";


        base._Process(delta);
    }
}
