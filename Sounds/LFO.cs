using Godot;
using System;

public partial class LFO : Node
{
	private RandomNumberGenerator random;

	[Export] public float Rate;
	[Export] public float Depth;

	private float StartHz;
	private AudioEffectFilter target;
	public override void _Ready()
	{
		var effect = AudioServer.GetBusEffect(2, 0);
		if (effect is not AudioEffectFilter target)
		{
			GD.Print("NotThatEffect"); return;
		}
		this.target = target;
		random = new();
		StartHz = target.CutoffHz;
		TargetHz = StartHz;
		base._Ready();
	}

	public override void _Process(double delta)
	{
		var effect = AudioServer.GetBusEffect(2, 0);

		if (target.CutoffHz > TargetHz)
		{
			delta *= -1;
			target.CutoffHz = (float)Mathf.Clamp(target.CutoffHz + Rate * delta, TargetHz, float.MaxValue);
		}
		else
			target.CutoffHz = (float)Mathf.Clamp(target.CutoffHz + Rate * delta, 0, TargetHz);

		/* 		GD.Print(target.CutoffHz);
		 */
		if (Mathf.IsEqualApprox(target.CutoffHz, TargetHz))
			SelectTarget();
		base._Process(delta);
	}
	float TargetHz;
	void SelectTarget()
	{

		TargetHz = StartHz * (1 + random.RandfRange(-Depth, Depth));
		GD.Print($"SelectTarget {Mathf.RoundToInt(TargetHz)} {Mathf.RoundToInt(target.CutoffHz)}");
	}
}
