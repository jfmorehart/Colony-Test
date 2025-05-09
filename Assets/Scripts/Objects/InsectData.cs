using UnityEngine;


[CreateAssetMenu(menuName = "Insect")]
public class InsectData : ScriptableObject
{
	//Scriptable Object for data-driven insect designs

	//I usually use CSV files for my games so forgive me if i'm
    //not up to date with scriptableobject best practices

	//Unit
	public float max_health = 1;

	//Soldier
	public float speed = 0.5f;
	public float damage = 0.1f;
	public float attack_delay = 1;
	public float attack_range = 1;
	public int search_radius_max = 1;
	public int target_search_delay = 1;
	public int target_chase_range = 4;

	public void ImpartData(Unit unit)
	{
		unit.health = max_health;
	}

	public void ImpartData(Soldier soldier) {
		soldier.health = max_health;
		soldier.speed = speed;
		soldier.damage = damage;
		soldier.attack_delay = attack_delay;
		soldier.attack_range = attack_range;
		soldier.search_radius_max = search_radius_max;
		soldier.target_search_delay = target_search_delay;
		soldier.target_chase_range = target_chase_range;
    }

}
