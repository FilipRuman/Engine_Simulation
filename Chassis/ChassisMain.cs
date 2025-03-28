using Godot;
public partial class ChassisMain : Node
{
    [Export] Crankshaft crankshaft;
    [Export] EngineController engine;
    [Export] private float mass; // kg
    [Export] private float linearVelocity; // km/h

    public int gear;
    [Export] float[] gearRatios;
    [Export] float whealRadious;
    [Export] float dragModifeir;
    [Export] Curve dragBasedOnVelocity;

    [ExportGroup("Ui")]

    [Export] Label gearText;
    [Export] Label linearVelocityText;
    [Export] Label dragText;

    const float msToKm = 3.6f;
    public override void _PhysicsProcess(double delta)
    {
        if (Engine.IsEditorHint())
            return;

        float currentGearRatio = 1 / gearRatios[gear];
        float engineTorque = engine.HandlePhysicsAndCalculateTorque((float)delta);

        float forceAtTheWheals = (engineTorque * currentGearRatio) / whealRadious;
        const int drivenWheals = 4;
        float totalEngineForce = forceAtTheWheals * (float)drivenWheals;
        float netForce = totalEngineForce - dragBasedOnVelocity.SampleBaked(linearVelocity) * dragModifeir;
        float acceleration = netForce / mass;

        linearVelocity += (acceleration * (float)delta) * msToKm;
        linearVelocity = Mathf.Max(0, linearVelocity);

        crankshaft.UpdateCrankshaftStatsBasedOnDrivetrain(linearVelocity, whealRadious, currentGearRatio, (float)delta);

        base._PhysicsProcess(delta);
    }
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("nextGear") && gear != gearRatios.Length - 1)
            gear++;
        if (Input.IsActionJustPressed("previousGear") && gear != 0)
            gear--;

        gearText.Text = $"gear: {gear}";
        linearVelocityText.Text = $"velocity: {Mathf.RoundToInt(linearVelocity)}";
        dragText.Text = $"drag force: {Mathf.RoundToInt(dragBasedOnVelocity.SampleBaked(linearVelocity) * dragModifeir)}";

        base._Process(delta);
    }


}
