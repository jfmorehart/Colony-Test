using UnityEngine;

public class Soldier : Unit
{

	/*
		This is the Soldier class, which should be inherited by any unit that wants
		access to basic virtual functions like target search and attack. 
		Not all variables are used by this class, like damage, but they are stored
		here for easier scriptable object reading
	*/

	//target is a specific enemy unit this soldier wants to kill
	//being near enough to a target will override objective orders
	protected Unit target_enemy;
	float last_target_search_time;
	public float target_search_delay = 1;
	//objective override base range, modified by angles to target/objective
	public float target_chase_range = 4;

	//this objective is a position this soldier wants to reach
	//might be the flag, might be a player-given directive
	//this is the fallback command if there are no nearby targets to chase
	protected bool consider_objective = true;
	protected Vector2 objective;


	//search radius, expands to maximum over consecutive misses
	int search_radius;
	//different strategies will require different search radii
	public int search_radius_max = 2; 

	float last_attack_time;  //the last time we attacked
	public float attack_delay;  //time between attacks
	public float attack_range;

	public float speed, damage;

	public Transform line;
	public Transform circle;

	public string debug_state = "uninitialized";

	public override void Start() {
		base.Start();

		//initialize these counters with the cooldowns mostly expired
		last_target_search_time = Random.Range(-1, 0);
		last_attack_time = Random.Range(-1, 0);

		if (GameManager.active_player_control && team == Team.Friendly)
		{
			//overwrite the default flag-protect directive
			Direct(transform.position);
			consider_objective = false;
		}

		renderingCommandUI = false;
		line.gameObject.SetActive(false);
		circle.gameObject.SetActive(false);
	}
	public override void MapUpdate()
	{

		//Soldier Reasoning
		Vector2 objective_delta = objective - (Vector2)transform.position;

		if (target_enemy != null)
		{
			//Attack if in range
			Vector2 target_delta = target_enemy.transform.position - transform.position;
			if (target_delta.magnitude < attack_range) {
				//in attack range, so try to attack;
				if (Time.time - last_attack_time > attack_delay) {
					last_attack_time = Time.time;
					AttackTarget();
					RenderCommandUI(target_enemy.transform.position);
					debug_state = "attacking";
				}
			}
			else { //enemy is out of range

				//decide whether to move towards the target or the objective
				//this direction_multiplier will let the soldier chase targets that are on the way
				//but will discourage turning around in order to attack an enemy behind them
				float direction_multiplier = Vector2.Dot(target_delta.normalized, objective_delta.normalized);
				//values are now from -1 to 1
				//adjust the possible value range into ideal range
				direction_multiplier = (direction_multiplier + 2) / 3; //values are now from 0.333 to 1;

				bool target_enroute = target_delta.magnitude < target_chase_range * direction_multiplier;

				//if we dont care about the objective, then we force this to be true
				target_enroute |= !consider_objective;

				//attack enemy if they're close to our obj
				Vector2 obj_to_enemy = (Vector2)target_enemy.transform.position - objective;
				bool near_obj = (obj_to_enemy.magnitude < attack_range * 2) && consider_objective;

				if (target_enroute || near_obj) {
					//charge the target
					MoveTowards(target_enemy.transform.position, target_delta);
					RenderCommandUI(target_enemy.transform.position);
					debug_state = "charging target: " + target_enemy.name + '\n' + "distance ="+ target_delta.magnitude;
				}
				else if(objective_delta.magnitude > attack_range && consider_objective) {
					//charge the objective
					target_delta = objective - (Vector2)transform.position;
					MoveTowards(objective, target_delta);
					RenderCommandUI(objective);
					debug_state = "moving towards objective";
				}
				else {
					//refresh target to make sure we aren't missing a bigger threat
					target_enemy = null;
					debug_state = "guarding objective";
				}

			}

		}
		else //idle logic
		{
			if(Time.time - last_target_search_time > target_search_delay) {
				last_target_search_time = Time.time;
				if (team != Team.Friendly || !GameManager.active_player_control) {
					SearchForTarget();
				}

			}

			if(objective_delta.magnitude > attack_range && consider_objective) {
				//go to the objective
				Vector2 delta = objective - (Vector2)transform.position;
				MoveTowards(objective, delta);
				RenderCommandUI(objective);
				debug_state = "moving towards objective";
			}
		}

		//grid check, should be at the end of the update loop
		base.MapUpdate();
	}

	//this function is called every MapUpdate, and controls how the 
	//lines and target UI is drawn to the screen
	bool renderingCommandUI;
	public void RenderCommandUI(Vector2 dest) {
		if (line.gameObject == null) return;
		if (circle.gameObject == null) return;

		if (!selected_by_player) {
			if (renderingCommandUI) {
				renderingCommandUI = false;
				line.gameObject.SetActive(false);
				circle.gameObject.SetActive(false);
			}
			return;
		}
		if (!renderingCommandUI) {
			renderingCommandUI = true;
			line.gameObject.SetActive(true);
			circle.gameObject.SetActive(true);
		}

		Vector2 delta = dest - (Vector2)transform.position;
		line.position = (dest + (Vector2)transform.position) * 0.5f;
		line.right = delta;
		line.localScale = new Vector3(delta.magnitude, 1, 1);
		circle.position = dest;
	}

	public override void Direct(Vector2 worldPosition)
	{
		target_enemy = null; 
		objective = worldPosition;
		consider_objective = true;
	}

	public override void DirectTarget(Unit u)
	{
		target_enemy = u;
		consider_objective = false;
	}

	//both position and delta parameters are passed in case inheritors have novel movement systems
	public virtual void MoveTowards(Vector2 pos, Vector2 delta) {
		transform.Translate(speed * Time.deltaTime * delta.normalized, Space.World);
		transform.right = delta;
	}

	//base virtual attack function
	public virtual void AttackTarget() {
		//todo implement
		Debug.Log("unimplemented attack!");
    }

	//simple search that gets larger every failed attempt
	public virtual void SearchForTarget()
	{
		target_enemy = Map.instance.AdjacentEnemy(grid_coordinates, team, search_radius);

		if(target_enemy != null) {
			search_radius = 1;
		}
		else if(search_radius < search_radius_max) {
			search_radius++;
		}
	}

	public override void Hurt(float damage, Unit attacker)
	{
		base.Hurt(damage, attacker);
		if (target_enemy == null) target_enemy = attacker;
	}
}
