using Godot;
using System;
[Tool]
public partial class EngineSoundController : Node
{
    [Export] AudioStreamPlayer player;
    [Export] NoiseAndSin waveGen;
    [Export(PropertyHint.Range, "0,1,")] public float throttle;

    [Export] public float rpm;
    // [Export] Curve2D
    [Export] public Vector2 pitchMinMax;
    [Export] public Vector2 volumeMinMax;
    public override void _Process(double delta)
    {
        float hz = rpm / 60f;
        float combustionHz = hz / 4f;
        // waveGen.frequency = combustionHz;

        if (!player.Playing && rpm != 0)
        {
            GD.Print("Start");
            player.StreamPaused = false;
        }
        if (player.Playing && rpm == 0)
            player.StreamPaused = true;

        player.VolumeDb = Mathf.Lerp(volumeMinMax.X, volumeMinMax.Y, throttle);

        player.PitchScale = combustionHz / 100f;

        base._Process(delta);
    }

}
