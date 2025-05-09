using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Antlike : Soldier
{
	/*	ant-likes have a simple headbutt attack and simple sinwave movement
		this class doesn't override the solider's MapUpdate function, but instead
		overrides the MoveTowards and Attack functions
	*/

	//movement deviation controls
	float sway_frequency = 1.5f;
	float sway_amplitude = 0.5f;

	// moved during the attack animation
	[SerializeField] Transform headbutt_joint;
	Vector2 headbutt_origin;

	//animation controls
	//these should add up to less than the attack_delay 
	public float headbutt_forwardDuration, headbutt_backwardDuration;
	private void Awake()
	{
		headbutt_origin = headbutt_joint.transform.localPosition;
	}
	public override void MoveTowards(Vector2 pos, Vector2 delta) {
		//sway consists of adding a perpendicular component to the desired direction
		//and modulating that by some sinwave or smoothed randomness
		Vector2 perpendicular = Vector2.Perpendicular(delta);
		float sway_value = Mathf.Sin(Time.time * sway_frequency) * sway_amplitude * speed;

		if (delta.magnitude > 0.5)
		{
			delta += sway_value * perpendicular;
			transform.right = delta;
		}
		transform.Translate(speed * Time.deltaTime * delta.normalized, Space.World);


	}

	//base virtual attack function
	public override void AttackTarget()
	{
		Vector2 delta = target_enemy.transform.position - transform.position;
		transform.right = delta;
		StartCoroutine(nameof(Headbutt));
	}

	//simple headbutt animation
	//could use while loops but why risk crashing over a silly animation
	IEnumerator Headbutt() {

		//store target position in case target is killed by other ant
		Vector2 localPosition = transform.InverseTransformPoint(target_enemy.transform.position);

		Vector2 delta = localPosition - headbutt_origin;
		localPosition = headbutt_origin + delta * 0.25f; //just a little forward
		float lerpAmt = 0;
		float start_time = Time.time;
		for(int frame = 0; frame < 999; frame++) {
			lerpAmt = (Time.time - start_time) / headbutt_forwardDuration ;
			Debug.Log(name + " " + lerpAmt);
			headbutt_joint.localPosition = Vector2.Lerp(headbutt_origin, localPosition, lerpAmt);
			yield return null; // wait for next frame
			if (lerpAmt > 1) break;
		}

		if (target_enemy != null) { //target might be dead by now
			target_enemy.Hurt(damage, this);
		}
		start_time = Time.time;
		for (int frame = 0; frame < 999; frame++)
		{
			lerpAmt = (Time.time - start_time) / headbutt_backwardDuration;
			Debug.Log(name + " " + lerpAmt);
			headbutt_joint.localPosition = Vector2.Lerp(headbutt_origin, localPosition, 1 -  lerpAmt);
			yield return null; // wait for next frame
			if (lerpAmt > 1) break;
		}
	}

}
