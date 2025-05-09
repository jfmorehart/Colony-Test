using UnityEngine;

public class HealthBar : MonoBehaviour
{
	//not meant to be scalable, just attached onto units as a debug tool
	bool active;
	[SerializeField] Unit unit;

	float max_health;
	float current_health;
	public Transform health_bar;
	public Transform back_bar;

	private void Start()
	{
		if (unit == null) {
			Destroy(gameObject);
			return;
		}
		active = true;
		max_health = unit.health;
		current_health = unit.health;
		float xSize = back_bar.localScale.x * (current_health / max_health);
		health_bar.localScale = new Vector3(xSize, 1, 1);
	}

	private void Update()
	{
		if (active) {
			transform.eulerAngles = Vector3.zero;
			if(current_health != unit.health) {
				//adjust healthbar to fix
				current_health = unit.health;
				float xSize = back_bar.localScale.x * (current_health / max_health);
				health_bar.localScale = new Vector3(xSize, 1, 1);

			}
		}
	}
}
