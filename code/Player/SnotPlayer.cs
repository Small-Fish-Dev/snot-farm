using Sandbox.Citizen;

public sealed class SnotPlayer : Component
{

	[Property]
	public ThirdPersonController Controller { get; set; }

	[Property]
	public CitizenAnimationHelper CitizenAnimation { get; set; }

	[Property]
	public UnitInfo UnitInfo { get; set; }

	[Range( 0f, 10f, 0.1f, true, true )]
	[Property]
	public float PunchDamage { get; set; } = 1f;

	[Range( 10f, 500f, 1f, true, true )]
	[Property]
	public float PunchRange { get; set; } = 120f;

	[Range( 1f, 100f, 1f, true, true )]
	[Property]
	public float PunchWidth { get; set; } = 20f;

	[Range( 0f, 2f, 0.1f, true, true )]
	[Property]
	public float PunchCooldown { get; set; } = 0.5f;

	public TimeSince LastPunch { get; set; } = 0f;

	protected override void OnFixedUpdate()
	{
		if ( Controller == null ) return;
		//if ( Animator == null ) return;

		//Animator.WithVelocity( Controller.Velocity );

		if ( Input.Down( "Punch" ) )
			if ( LastPunch >= PunchCooldown )
				Punch();

		if ( LastPunch >= PunchCooldown * 4f )
			if ( CitizenAnimation != null )
				CitizenAnimation.HoldType = CitizenAnimationHelper.HoldTypes.None;
	}

	public void Punch()
	{
		if ( Controller == null ) return;
		if ( UnitInfo == null ) return;

		var enemyTagsArray = UnitInfo.EnemyUnitTypes
			.Select( x => x.ToString() )
			.ToArray();

		var punchTrace = Scene.Trace
			.FromTo( Transform.Position + Controller.EyePosition, Transform.Position + Controller.EyePosition + Controller.EyeRotation.Forward * PunchRange )
			.Size( PunchWidth )
			.WithAnyTags( enemyTagsArray )
			.WithoutTags( "player" )
			.Run();

		if ( punchTrace.Hit )
			if ( punchTrace.GameObject.Components.TryGet<UnitInfo>( out UnitInfo unitInfo ) )
				unitInfo.Damage( PunchDamage );

		if ( CitizenAnimation != null )
		{
			CitizenAnimation.HoldType = CitizenAnimationHelper.HoldTypes.Punch;
			CitizenAnimation.Target.Set( "b_attack", true );
		}

		LastPunch = 0;

		//Log.Info( punchTrace.GameObject.GetAllObjects( true ).FirstOrDefault().Name );
	}
}
