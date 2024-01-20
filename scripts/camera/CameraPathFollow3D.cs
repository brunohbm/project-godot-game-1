using Godot;
using System;
using System.Diagnostics;

public partial class CameraPathFollow3D : PathFollow3D
{
	[Export]
	public float InitialProgressSpeed = 0.04f;
	[Export]
	public float SlowMotionProgressSpeed = 0.001f;
	[Export]
	public Camera3D MainCamera;
	[Export]
	public Camera3D SecondCamera;
	[Export]
	public Camera3D ThirdCamera;
	[Export]
	public float FirstCameraTransitionProgress;
	[Export]
	public float FirstCameraRotationSpeed;
	[Export]
	public float FirstCameraRotationStop;
	[Export]
	public float RunningProgressSpeed = 0.1f;
	[Export]
	public float SecondCameraTransitionProgress;
	[Export]
	public float SecondCameraRotationSpeed;
	[Export]
	public float SecondCameraTransitionSpeed = 0.1f;
	[Export]
	public float SecondCameraRotationStop;

	private bool HasFinishedFirstCameraTransition = false;
	private bool HasFinishedSecondCameraTransition = false;
	public float ActualProgressSpeed = 0.04f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ActualProgressSpeed = InitialProgressSpeed;
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

	void FallingCameraTransition()
	{
		Transform3D transform = MainCamera.Transform;
		transform.Basis = transform.Basis.Rotated(Vector3.Back, SecondCameraRotationSpeed);
		MainCamera.Transform = transform;

		HasFinishedSecondCameraTransition = MainCamera.Transform.Basis.Y.X <= SecondCameraRotationStop;
	}

	void CheckCameraTransition()
	{
		if (HasFinishedFirstCameraTransition)
		{
			if (HasFinishedSecondCameraTransition)
			{
				ActualProgressSpeed = 0;
				SecondCamera.MakeCurrent();
				// TODO - Add camera transition.
				return;
			}

			if (this.ProgressRatio >= SecondCameraTransitionProgress)
			{
				ActualProgressSpeed = SecondCameraTransitionSpeed;
				FallingCameraTransition();
				return;
			}

			ActualProgressSpeed = RunningProgressSpeed;
			return;
		}

		if (this.ProgressRatio >= FirstCameraTransitionProgress)
		{
			ActualProgressSpeed = SlowMotionProgressSpeed;
			GetUpCameraTransition();
			return;
		}

		ActualProgressSpeed = InitialProgressSpeed;
	}

	public override void _PhysicsProcess(double delta)
	{
		this.ProgressRatio += (float)(ActualProgressSpeed * delta);
		CheckCameraTransition();
	}
}
