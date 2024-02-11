using Godot;
using System;

public partial class SpaceButtonControll : TextureRect
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	async public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("action"))
		{
			this.Visible = true;
			await ToSignal(GetTree().CreateTimer(0.1), "timeout");
			this.Visible = false;
		}
	}
}
