using Godot;
using System;
using System.Collections.Generic;
public partial class EngineUI : Node {
    public EngineController engine;
    public Crankshaft crankshaft;

    [Export] private GraphMain graph;
    [Export] private EngineSoundController soundController;
    [Export] Slider throttleSlider;
    [Export] Label angularVelocityText;
    [Export] Label totalTorque;
    [Export] Label averageGasTemperature;

    [Export] Label exhaustFumesRatioBeforeCombustion;
    [Export] Label rpm;
    [Export] Label horsePower;
    [Export] Label temperature;
    [Export] Label gameFps;

    [Export] public uint graphPowerDataGroupIndex;
    [Export] public uint graphTorqueDataGroupIndex;
    public void AddCurrentPhysicsStatsForGraph() {
        graph.AddDataToEnd(engine.currentHorsePower, graphPowerDataGroupIndex);
        graph.AddDataToEnd(engine.currentTorque, graphTorqueDataGroupIndex);

    }

    public override void _Process(double delta) {


        UpdateTextUI();
        base._Process(delta);
    }

    [Export] string throttleActionName = "throttle";
    private void HandleInput() {

        if (throttleActionName != "")
            engine.throttle = Input.GetActionStrength(throttleActionName);


    }
    private void UpdateTextUI() {
        // i just use one of cylinders so i don't heave to do any weird averaging, ratios should be similar in all cylinders
        exhaustFumesRatioBeforeCombustion.Text = $"Exhaust fumes ratio in gas mixture before combustion {Math.Round(engine.cylinders[0].CurrentCombustionFumesAirRatio, 2)}";
        gameFps.Text = $"FPS {Engine.GetFramesPerSecond()}";
        throttleSlider.Value = engine.throttle;
        temperature.Text = $"Temperature of 1'st cylinder's wall: {Mathf.RoundToInt(engine.heatHandler.cylinderWallTemperature - 273)}C";

        angularVelocityText.Text = $"Angular velocity: {Mathf.RoundToInt(crankshaft.angularVelocityDeg)}";
        rpm.Text = $"RPM: {Mathf.RoundToInt(crankshaft.RevolutionsPerSecond * 60f)}";

        soundController.throttle = engine.overRPM ? 0 : engine.throttle;
        soundController.rpm = crankshaft.RevolutionsPerSecond * 60f;

        horsePower.Text = $"Horse power: {(int)crankshaft.engine.currentHorsePower}";
        totalTorque.Text = $"Torque: {(int)engine.averageTorque}";
        averageGasTemperature.Text = $"Average gas temperature: {(int)engine.cylinders[0].gasTemperatureInsideCylinder}";

    }

}
