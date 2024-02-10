using Godot;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

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
	public Camera3D FourthCamera;
	[Export]
	public Camera3D FifthCamera;
	[Export]
	public Camera3D SixthCamera;
	[Export]
	public Camera3D SeventhCamera;
	[Export]
	public Camera3D EightCamera;
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

	private bool StartFirstCameraTransition = false;
	private bool StartSecondCameraTransition = false;
	private bool StartThirdCameraTransition = false;
	private bool StartFourthCameraTransition = false;
	private bool StartFifthCameraTransition = false;
	private bool StartSixthCameraTransition = false;
	private bool StartSeventhCameraTransition = false;
	private bool StartEightCameraTransition = false;
	private bool HasFinishedCamerasTransitions = false;
	private bool IsAnimationPaused = true;
	private int ActionPressAmount = 0;


	public float ActualProgressSpeed = 0.04f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ActualProgressSpeed = InitialProgressSpeed;
	}

	void _MakeGlowCameraAnimation()
	{
		if (MainCamera.Environment.AdjustmentBrightness > 1)
		{
			MainCamera.Environment.AdjustmentBrightness -= 0.00389f;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!this.IsAnimationPaused)
		{
			this._MakeGlowCameraAnimation();
		}
	}

	void GetUpCameraTransition()
	{
		Transform3D transform = MainCamera.Transform;
		transform.Basis = transform.Basis.Rotated(Vector3.Forward, FirstCameraRotationSpeed);
		MainCamera.Transform = transform;

		StartFirstCameraTransition = MainCamera.Transform.Basis.Y.X >= FirstCameraRotationStop;
	}

	void FallingCameraTransition()
	{
		Transform3D transform = MainCamera.Transform;
		transform.Basis = transform.Basis.Rotated(Vector3.Back, SecondCameraRotationSpeed);
		MainCamera.Transform = transform;

		StartSecondCameraTransition = MainCamera.Transform.Basis.Y.X <= SecondCameraRotationStop;
	}

	void ZoomInCamera(Camera3D camera)
	{
		camera.Fov -= 0.02f;
	}

	async void CheckCameraTransition()
	{
		if (HasFinishedCamerasTransitions)
		{
			return;
		}


		if (StartEightCameraTransition)
		{
			EightCamera.MakeCurrent();
			ZoomInCamera(EightCamera);
			await ToSignal(GetTree().CreateTimer(0.4), "timeout");
			StartEightCameraTransition = false;
			HasFinishedCamerasTransitions = true;
			ActualProgressSpeed = 0.135f;
			MainCamera.Rotation = new Vector3(-89.59f, -142.95f, 0);
			MainCamera.Fov = 130f;
			MainCamera.MakeCurrent();
			return;
		}

		if (StartSeventhCameraTransition)
		{
			SeventhCamera.MakeCurrent();
			ZoomInCamera(SeventhCamera);
			await ToSignal(GetTree().CreateTimer(0.4), "timeout");
			StartEightCameraTransition = true;
			StartSeventhCameraTransition = false;
			return;
		}

		if (StartSixthCameraTransition)
		{
			SixthCamera.MakeCurrent();
			ZoomInCamera(SixthCamera);
			await ToSignal(GetTree().CreateTimer(0.4), "timeout");
			StartSeventhCameraTransition = true;
			StartSixthCameraTransition = false;
			return;
		}


		if (StartFifthCameraTransition)
		{
			FifthCamera.MakeCurrent();
			ZoomInCamera(FifthCamera);
			await ToSignal(GetTree().CreateTimer(0.4), "timeout");
			StartSixthCameraTransition = true;
			StartFifthCameraTransition = false;
			return;
		}

		if (StartFourthCameraTransition)
		{
			FourthCamera.MakeCurrent();
			ZoomInCamera(FourthCamera);
			await ToSignal(GetTree().CreateTimer(0.4), "timeout");
			StartFifthCameraTransition = true;
			StartFourthCameraTransition = false;
			return;
		}

		if (StartThirdCameraTransition)
		{
			ThirdCamera.MakeCurrent();
			ZoomInCamera(ThirdCamera);
			await ToSignal(GetTree().CreateTimer(1.6), "timeout");
			StartFourthCameraTransition = true;
			StartThirdCameraTransition = false;
			return;
		}


		if (StartSecondCameraTransition)
		{
			StartFirstCameraTransition = false;
			ActualProgressSpeed = 0;
			SecondCamera.MakeCurrent();
			ZoomInCamera(SecondCamera);
			await ToSignal(GetTree().CreateTimer(2.3), "timeout");
			StartThirdCameraTransition = true;
			StartSecondCameraTransition = false;
			return;
		}

		if (StartFirstCameraTransition)
		{
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
		if (IsAnimationPaused) return;

		this.ProgressRatio += (float)(ActualProgressSpeed * delta);
		CheckCameraTransition();
	}

	async public override void _Input(InputEvent @event)
	{
		if (!IsAnimationPaused) return;

		if (@event.IsActionPressed("action"))
		{
			ActionPressAmount++;
			IsAnimationPaused = ActionPressAmount <= 7;
			if (!IsAnimationPaused)
			{
				MainCamera.Environment.AdjustmentBrightness = 2;
				return;
			}

			await ToSignal(GetTree().CreateTimer(3), "timeout");
			ActionPressAmount = 0;
		}
	}
}
