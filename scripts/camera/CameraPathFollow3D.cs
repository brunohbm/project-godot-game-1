using Godot;
using System;
using System.Diagnostics;

// TODO - Understand what happened here

public partial class CameraPathFollow3D : PathFollow3D
{
	[Export]
	public float ProgressSpeed = 0.01f;
	[Export]
	public float SlowMotionProgressSpeed = 0.01f;
	[Export]
	public Camera3D MainCamera;
	[Export]
	public float FirstCameraTransitionProgress;
	[Export]
	public float FirstCameraRotationSpeed;
	[Export]
	public float FirstCameraRotationStop;

	private bool HasFinishedFirstCameraTransition = false;
	public float ActualProgressSpeed = 0.01f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ActualProgressSpeed = ProgressSpeed;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	void GetUpCameraTransition()
	{
		Transform3D transform = MainCamera.Transform;
		transform.Basis = transform.Basis.Rotated(Vector3.Forward, FirstCameraRotationSpeed);
		MainCamera.Transform = transform;

		HasFinishedFirstCameraTransition = MainCamera.Transform.Basis.Y.X >= FirstCameraRotationStop;
	}

	void CheckCameraTransition()
	{
		if (!HasFinishedFirstCameraTransition && this.ProgressRatio >= FirstCameraTransitionProgress)
		{
			ActualProgressSpeed = SlowMotionProgressSpeed;
			GetUpCameraTransition();
			return;
		}

		ActualProgressSpeed = ProgressSpeed;
	}

	public override void _PhysicsProcess(double delta)
	{
		this.ProgressRatio += (float)(ActualProgressSpeed * delta);
		CheckCameraTransition();
	}
}
