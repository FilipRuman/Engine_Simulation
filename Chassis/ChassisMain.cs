using Godot;
public partial class ChassisMain : Node
{
    [Export] Crankshaft crankshaft;
    [Export] EngineController engine;
    [Export] private float mass; // kg
    [Export] private float linearVelocity; // km/h
    [Export] private float breakeTorque;
    public float brakePosition;

    public int gear;
    [Export] float[] gearRatios;
    [Export] float whealRadious;
    [Export] float dragModifeir;
    [Export] private float starterSpeed;
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
        float dragForce = dragBasedOnVelocity.SampleBaked(linearVelocity) * dragModifeir;
        float breakeForce = breakeTorque * brakePosition;
        float netForce = totalEngineForce - breakeForce - dragForce;
        float acceleration = netForce / mass;
        //TODO: change that to keybinding


        linearVelocity += (acceleration * (float)delta) * msToKm;

        linearVelocity = Mathf.Max(0, linearVelocity);
        if (crankshaft.visuals.starterButton.ButtonPressed)
        {
            linearVelocity = starterSpeed;
        }
        crankshaft.UpdateCrankshaftStatsBasedOnDrivetrain(linearVelocity, whealRadious, currentGearRatio, (float)delta);

        base._PhysicsProcess(delta);
    }
    public override void _Process(double delta)
    {
        HandleInput();
        gearText.Text = $"gear: {gear}";
        linearVelocityText.Text = $"velocity: {Mathf.RoundToInt(linearVelocity)}";
        dragText.Text = $"drag force: {Mathf.RoundToInt(dragBasedOnVelocity.SampleBaked(linearVelocity) * dragModifeir)}";

        base._Process(delta);
    }
    [Export] string nextGearActionName = "nextGear";
    [Export] string previousGearActionName = "previousGear";
    [Export] string starterActionName = "starter";

    [Export] string throttleActionName = "throttle";
    [Export] string breakeActionName = "breake";
    private void HandleInput()
    {

        if (nextGearActionName != "" && Input.IsActionJustPressed(nextGearActionName) && gear != gearRatios.Length - 1)
            gear++;
        if (previousGearActionName != "" && Input.IsActionJustPressed(previousGearActionName) && gear != 0)
            gear--;
        if (starterActionName != "" && Input.IsActionJustPressed(starterActionName) && linearVelocity < starterSpeed + 10)
            crankshaft.visuals.starterButton.ButtonPressed = !crankshaft.visuals.starterButton.ButtonPressed;

        if (throttleActionName != "")
            engine.throttle = Input.GetActionStrength(throttleActionName);

        if (breakeActionName != "")
            brakePosition = Input.GetActionStrength(breakeActionName);

    }



}
