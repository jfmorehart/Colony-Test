using UnityEngine;

public class Unit : MonoBehaviour
{
	/*
		This is the base unit class! This class should be inherited by anything that
		wants to be searchable on the grid. 
		This intentionally omits some important variables like speed and damage because
		it might be used for map-objects like breakable walls, or plants, or whatever
	*/

	public enum Team { Neutral, Friendly, Enemy};
	public Team team; 

	public Vector2Int grid_coordinates;
	protected Grid<Unit> grid_instance;

	public float health = 1;

	protected bool selected_by_player;
	public GameObject highlight_object;

	public virtual void Start()
	{
		ToggleSelect(false);//turn off highlight object

		//unimportant
		grid_instance = Map.instance.grid;

		//store spawning coordinates to compare against while moving
		grid_coordinates = grid_instance.RegisterUnit(transform.position, this);

		//snap onto grid
		transform.position = grid_instance.PositionFromCoordinates(grid_coordinates);

		//register in order to recieve MapUpdates
		Map.instance.Register(this);
	}

	public virtual void MapUpdate()
	{
		//Adjust grid data structure if necessary
		Vector2Int testPosition = grid_instance.CoordinatesFromPosition(transform.position);
		if (testPosition != grid_coordinates)
		{
			//we have moved into a new gridzone!
			grid_instance.Move(grid_coordinates, this, testPosition);
			grid_coordinates = testPosition;
		}
	}

	public virtual void Direct(Vector2 worldPosition) {
		Debug.Log("Unimplemented Direct Call");
	}

	public virtual void DirectTarget(Unit u) {
		Debug.Log("Unimplemented DirectTarget Call");
	}


	#region utility
	public void ToggleSelect(bool on) {
		selected_by_player = on;
		if(highlight_object != null) {
			highlight_object.SetActive(on);
		}
    }
	public virtual void Hurt(float damage, Unit attacker) {
		health -= damage;
		if (health < 0) Kill();
    }
	public virtual void Kill() {
		grid_instance.DeRegisterUnit(grid_coordinates.x, grid_coordinates.y, this);
		Map.instance.DeRegister(this);
    }
	#endregion
}
