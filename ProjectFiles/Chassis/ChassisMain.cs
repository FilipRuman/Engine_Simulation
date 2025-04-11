using Godot;
[Tool]
public partial class ChassisMain : Node {
    [Export] Crankshaft crankshaft;
    [Export] public EngineController engine;
    [Export] public float mass; // kg
    [Export] public float linearVelocity; // km/h
    [Export] private float breakeTorque;
    public float brakePosition;

    public int gear;
    [Export] public float[] gearRatios;
    [Export] private float whealRadious;
    [Export] private float dragModifier;
    [Export] public float starterSpeed;
    [Export] Curve dragBasedOnVelocity;

    public float currentDragForce;
    const float msToKm = 3.6f;

    public bool starterButtonPressed;



    public override void _PhysicsProcess(double delta) {
        if (Engine.IsEditorHint())
            return;

        engine.HandlePhysics((float)delta);
        engine.PhysicsProcessDataForLaterUI();

        float totalEngineForce = ModifyTorqueByDrivetrainRatio(engine.currentTorque);
        currentDragForce = dragBasedOnVelocity.SampleBaked(linearVelocity) * dragModifier;

        float breakeForce = breakeTorque * brakePosition;
        float netForce = totalEngineForce - breakeForce - currentDragForce;
        float acceleration = netForce / mass;


        linearVelocity += (acceleration * (float)delta) * msToKm;

        linearVelocity = Mathf.Max(0, linearVelocity);

        if (starterButtonPressed)
            linearVelocity = starterSpeed;

        crankshaft.UpdateCrankshaftStatsBasedOnDrivetrain(linearVelocity, whealRadious, CurrentGearRatio, (float)delta);

        base._PhysicsProcess(delta);
    }



    public float CurrentGearRatio => 1 / gearRatios[gear];
    private float ModifyTorqueByDrivetrainRatio(float engineTorque) {

        float forceAtTheWheals = (engineTorque * CurrentGearRatio) / whealRadious;
        const int drivenWheals = 4;
        return forceAtTheWheals * (float)drivenWheals;
    }
}
