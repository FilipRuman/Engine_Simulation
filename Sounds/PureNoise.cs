using Godot;
using System;

public partial class PureNoise : Node
{
	[Export] public AudioStreamPlayer Player { get; set; }

	private AudioStreamGeneratorPlayback _playback; // Will hold the AudioStreamGeneratorPlayback.
	[Export] private int SampleRate = 44100;
	[Export] private float Duration = .1f; // Duration in seconds
	[Export] private float NoiseVolume = 0.1f; // Volume of the noise component
	[Export] private bool disable = false;
	[Export] bool distortion = false;
	public override void _Ready()
	{
		if (Player.Stream is AudioStreamGenerator generator) // Type as a generator to access MixRate.
		{
			Player.Play();
			_playback = (AudioStreamGeneratorPlayback)Player.GetStreamPlayback();
		}
	}
	public override void _Process(double delta)
	{
		if (disable)
			return;
		PlaySound();
		base._Process(delta);
	}
	// ? To change rpm just change Base Frequency
	public void PlaySound()
	{
		int totalSamples = (int)(SampleRate * Duration);
		Vector2[] samples = new Vector2[totalSamples];

		for (int i = 0; i < totalSamples; i++)
		{
			float time = (float)i / SampleRate;


			// Generate white noise
			float noise = (float)(GD.Randf() * 2.0 - 1.0) * NoiseVolume;

			// Combine the sine wave and noise
			samples[i] = new(noise, noise);
		}
		if (distortion)
			ApplyDistortion(totalSamples, ref samples);

		_playback.PushBuffer(samples);
	}

	[Export] private float DistortionAmount = 1.5f;
	[Export] private float FeedbackAmount = 0.3f;
	public void ApplyDistortion(int totalSamples, ref Vector2[] samples)
	{
		float feedback = 0f;
		for (int i = 0; i < totalSamples; i++)
		{
			Vector2 startSample = samples[i];
			startSample.Y = Distort(startSample.Y, ref feedback);
			startSample.X = startSample.Y;
			samples[i] = startSample;
		}
	}
	private float Distort(float input, ref float feedback)
	{
		float distortedSignal = input * DistortionAmount;

		// Clipping
		if (distortedSignal > 1.0f) distortedSignal = 1.0f;
		if (distortedSignal < -1.0f) distortedSignal = -1.0f;

		// Add feedback
		distortedSignal += feedback * FeedbackAmount;

		feedback = distortedSignal; // Update feedback for the next sample
		return distortedSignal;
	}
}
