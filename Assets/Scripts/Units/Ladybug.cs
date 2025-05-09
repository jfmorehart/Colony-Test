using System.Collections;
using UnityEngine;

public class Ladybug : Antlike
{
	//I thought ladybugs ate aphids? But here they are, in kahoots with them

	public float flyby_range;
	bool started_attack_run;

	//silly little animationish thing
	[SerializeField] GameObject nofly, fly;
	public float flight_duration;


	public override void Start()
	{
		base.Start();

		target_enemy = Map.instance.friendly_loses_if_dead[0]; //flag
	}

	public override void  MapUpdate()
	{
		base.MapUpdate();

		if (started_attack_run) return;
		Vector2 target_delta = target_enemy.transform.position - transform.position;
		if(target_delta.magnitude < flyby_range) {
			StartCoroutine(nameof(FlyBy));
		}
	}

	IEnumerator FlyBy() {
		started_attack_run = true;
		float realspeed = speed;
		speed = 0;
		bool flysprite = false;
		for(int i = 0; i < 5; i++) {
			flysprite = !flysprite;
			nofly.SetActive(!flysprite);
			fly.SetActive(flysprite);
			yield return new WaitForSeconds(0.5f - i * 0.08f);
		}
		Vector2 start_pos = transform.position;
		float start_time = Time.time;

		for (int i = 0; i < 999; i++) {
			float lerpAmt = (Time.time - start_time)/flight_duration;
			transform.position = Vector3.Lerp(start_pos, target_enemy.transform.position, lerpAmt);
			if(i % 5 == 0 && !(i % 10 == 0)) {
				flysprite = !flysprite;
				nofly.SetActive(!flysprite);
				fly.SetActive(flysprite);
			}
			yield return null;
			if (lerpAmt >= 1) break;
		}
		speed = realspeed;
		nofly.SetActive(true);
		fly.SetActive(false);
		//if(target_enemy != null) {
		//	target_enemy.Hurt(damage * 2, this);
		//}
	}
}
