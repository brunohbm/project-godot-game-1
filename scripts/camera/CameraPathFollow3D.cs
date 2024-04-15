using Godot;
using System;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;

// TODO - Make Reverb steps with transition sound

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
	[Export]
	public CanvasLayer UiCanvasLayer;
	[Export]
	public AudioStreamPlayer IndoorStepsAudio;
	[Export]
	public AudioStreamPlayer OutdoorStepsAudio;
	[Export]
	public AudioStreamPlayer AbruptDoorOpen;
	[Export]
	public AudioStreamPlayer TensionSound;
	[Export]
	public AudioStreamPlayer MusicAfterDoorOpen;
	[Export]
	public AnimatedSprite2D StairWalkAnimation;
	[Export]
	public MeshInstance3D DoorHinge;

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
	private bool isPlayingIntro = true;
	private int ActionPressAmount = 0;


	public float ActualProgressSpeed = 0.04f;

	void _OnFinishIntro()
	{
		isPlayingIntro = false;
		UiCanvasLayer.Visible = true;
	}

	void _StartVolumeIntro()
	{
		IndoorStepsAudio.VolumeDb = -30;
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(IndoorStepsAudio, "volume_db", 0, 13.5f);
		tween.TweenCallback(Callable.From(this._OnFinishIntro));
	}

	void _StartAnimationIntro()
	{
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(StairWalkAnimation, "modulate", new Color(1, 1, 1, 0.03f), 7f);
		tween.TweenProperty(StairWalkAnimation, "modulate", new Color(1, 1, 1, 1f), 9f);
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ActualProgressSpeed = InitialProgressSpeed;
		this._StartVolumeIntro();
		this._StartAnimationIntro();
	}

	void _MakeGlowCameraAnimation()
	{
		if (MainCamera.Environment.AdjustmentBrightness > 1)
		{
			MainCamera.Environment.AdjustmentBrightness -= 0.00389f;
		}
	}

	void _MakeDoorAnimation()
	{
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(DoorHinge, "rotation", new Vector3(0, -2, 0), 3f);
	}

	public override void _Process(double delta)
	{
		if (!this.IsAnimationPaused)
		{
			this._MakeGlowCameraAnimation();
			this._MakeDoorAnimation();
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
			ActualProgressSpeed = 0.12f;
			MainCamera.Rotation = new Vector3(-89.59f, -142.95f, 0);
			MainCamera.Fov = 120f;
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

	void _AudioTransitionController()
	{
		if (!OutdoorStepsAudio.Playing && StartFirstCameraTransition)
		{
			OutdoorStepsAudio.Play();
		}

		if (this.ProgressRatio >= 0.28f && OutdoorStepsAudio.Playing)
		{
			OutdoorStepsAudio.Stop();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (IsAnimationPaused || isPlayingIntro) return;

		this._AudioTransitionController();

		if (this.ProgressRatio >= 0.869)
		{
			MainCamera.Environment.AdjustmentBrightness = 0;
			return;
		}

		this.ProgressRatio += (float)(ActualProgressSpeed * delta);
		CheckCameraTransition();
	}

	async public override void _Input(InputEvent @event)
	{
		if (!IsAnimationPaused || isPlayingIntro) return;

		if (@event.IsActionPressed("action"))
		{
			ActionPressAmount++;
			IsAnimationPaused = ActionPressAmount <= 18;
			IndoorStepsAudio.PitchScale += 0.01f;
			IndoorStepsAudio.VolumeDb += 1.5f;
			if (!IsAnimationPaused)
			{
				StairWalkAnimation.Visible = false;
				MainCamera.Environment.AdjustmentBrightness = 2;
				IndoorStepsAudio.PitchScale = 0.9f;
				IndoorStepsAudio.VolumeDb = 0;
				IndoorStepsAudio.Stop();
				TensionSound.Stop();
				UiCanvasLayer.QueueFree();
				MusicAfterDoorOpen.Play();
				AbruptDoorOpen.Play();
				return;
			}

			await ToSignal(GetTree().CreateTimer(4), "timeout");
			if (IsAnimationPaused)
			{
				ActionPressAmount = 0;
				IndoorStepsAudio.PitchScale = 1;
				IndoorStepsAudio.VolumeDb = 0;
			}
		}
	}
}
