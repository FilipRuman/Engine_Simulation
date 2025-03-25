using Godot;
using System;

public partial class WindController : Node
{
    [Export] AudioStreamPlayer player;
    [Export] public float MaxSpeed;
    [Export] public float Speed;
    [Export] public Vector2 volumeMinMax;
    public override void _Process(double delta)
    {

        player.VolumeDb = Mathf.Lerp(volumeMinMax.X, volumeMinMax.Y, Speed / MaxSpeed);

        base._Process(delta);
    }



}
