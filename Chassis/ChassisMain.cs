using Godot;
public partial class ChassisMain : Node {

    [Export] Crankshaft crankshaft;
    [Export] EngineController engine;
    [Export] private float mass; // kg
    [Export] public float linearVelocity; // km/h
    [Export] private float breakeTorque;
    public float brakePosition;

    public int gear;
    [Export] private float[] gearRatios;
    [Export] private float whealRadious;
    [Export] private float dragModifier;
    [Export] private float starterSpeed;
    [Export] Curve dragBasedOnVelocity;

    public float currentDragForce;
    const float msToKm = 3.6f;
    public override void _PhysicsProcess(double delta) {
        if (Engine.IsEditorHint())
            return;


        float currentGearRatio = 1 / gearRatios[gear];
        float engineTorque = engine.HandlePhysicsAndCalculateTorque((float)delta);

        float forceAtTheWheals = (engineTorque * currentGearRatio) / whealRadious;
        const int drivenWheals = 4;
        float totalEngineForce = forceAtTheWheals * (float)drivenWheals;

        currentDragForce = dragBasedOnVelocity.SampleBaked(linearVelocity) * dragModifier;

        float breakeForce = breakeTorque * brakePosition;
        float netForce = totalEngineForce - breakeForce - currentDragForce;
        float acceleration = netForce / mass;
        //TODO: change that to keybinding


        linearVelocity += (acceleration * (float)delta) * msToKm;

        linearVelocity = Mathf.Max(0, linearVelocity);
        if (crankshaft.visuals.starterButton.ButtonPressed) {
            linearVelocity = starterSpeed;
        }
        crankshaft.UpdateCrankshaftStatsBasedOnDrivetrain(linearVelocity, whealRadious, currentGearRatio, (float)delta);

        base._PhysicsProcess(delta);
    }
    public override void _Process(double delta) {
        HandleInput();

        base._Process(delta);
    }
    [Export] string nextGearActionName = "nextGear";
    [Export] string previousGearActionName = "previousGear";
    [Export] string starterActionName = "starter";

    [Export] string throttleActionName = "throttle";
    [Export] string brakeActionName = "breake";
    private void HandleInput() {

        if (nextGearActionName != "" && Input.IsActionJustPressed(nextGearActionName) && gear != gearRatios.Length - 1)
            gear++;
        if (previousGearActionName != "" && Input.IsActionJustPressed(previousGearActionName) && gear != 0)
            gear--;
        if (starterActionName != "" && Input.IsActionJustPressed(starterActionName) && linearVelocity < starterSpeed + 10)
            crankshaft.visuals.starterButton.ButtonPressed = !crankshaft.visuals.starterButton.ButtonPressed;

        if (throttleActionName != "")
            engine.throttle = Input.GetActionStrength(throttleActionName);

        if (brakeActionName != "")
            brakePosition = Input.GetActionStrength(brakeActionName);

    }



}
