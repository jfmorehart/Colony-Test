using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
	/*
		The map class creates each level. It is responsible for placing 
		landmarks like the grid, the flag, friendly ants and enemy insects. 
	*/

	public static Map instance;


	public Vector2Int grid_dimensions;
	public Grid<Unit> grid;

	//sorry for the weird naming conventions haha
	public int number_of_ants;
	public int number_of_aphids;
	public int number_of_beetles;
	public int number_of_ladybugs;

	public GameObject debug_grid_prefab;

	public GameObject unit_factory_prefab;
	public UnitFactory factory;

	//used for avoiding unity update loop
	public List<Unit> all_units = new List<Unit>();

	//just stores the flag currently, but easily modifiable for other gamemodes
	public Unit[] friendly_loses_if_dead;
	//same thing for this, currently just stores the waves of enemies
	public Unit[] enemy_loses_if_dead;

	private void Awake()
	{
		//shouldn't be an issue
		if (instance != null && instance != this) {
			Debug.LogError("Duplicate map instances!");
		}
		instance = this;

		InvokeRepeating(nameof(SlowUpdate), 0.5f, 0.5f);

		//this constructor call builds out the spatial partitoning system
		grid = new Grid<Unit>(grid_dimensions.x, grid_dimensions.y);

		//instantiate the debug grid cells
		for (int i = 0; i < grid.cells.GetLength(0); i++) {
			for (int j = 0; j < grid.cells.GetLength(1); j++)
			{
				Instantiate(debug_grid_prefab, grid.PositionFromCoordinates(i, j), Quaternion.identity, transform);
			}
		}

		//factory system not super helpful at this scope
		//but its decoupled enough to allow for enemy variants
		factory = Instantiate(unit_factory_prefab, transform).GetComponent<UnitFactory>();

		//what populates these arrays could be controlled by a gamemode enum
		Unit flag = factory.SpawnUnit(UnitFactory.UnitType.Flag);
		friendly_loses_if_dead = new Unit[] {flag};

		//spawn a handfull of random units
		List<Unit> enemies = new List<Unit>();
		enemies.AddRange(factory.SpawnRadiallyAroundPoint(number_of_aphids, UnitFactory.UnitType.Aphid, Vector2.zero, 6));
		enemies.AddRange(factory.SpawnRadiallyAroundPoint(number_of_beetles, UnitFactory.UnitType.Beetle, Vector2.zero, 4));
		enemies.AddRange(factory.SpawnRadiallyAroundPoint(number_of_ladybugs, UnitFactory.UnitType.LadyBug, Vector2.zero, 7));
		enemy_loses_if_dead = enemies.ToArray();

		//spawn ant defenders!
		_ = factory.SpawnRadiallyAroundPoint(number_of_ants, UnitFactory.UnitType.Ant, Vector2.zero, 2);
	}

	#region update

	List<Unit> units_to_remove = new List<Unit>();  //stores units that are marked for death

	//registration for the centralized update-loop!
	public void Register(Unit un) { all_units.Add(un); }
	public void DeRegister(Unit un) { units_to_remove.Add(un); }


	private void Update()
	{
		//unit update loop!

		//we centrally process it here in map to avoid unity update overhead
		//and generally give ourselves more control over when to simulate units
		foreach (Unit un in all_units) {
			un.MapUpdate();
		}

		//during the above loop, killed units were added into this list
		//we can now safely deregister and destroy them 
		foreach (Unit un in units_to_remove)
		{
			if (un == null) continue; //may have been killed multiple times in the last loop
			all_units.Remove(un);
			Destroy(un.gameObject);
			//check after next frame, since destroy is not immediate
			Invoke(nameof(WinLossCheck), 0.5f);
		}
	}
	//this runs every 0.5s
	private void SlowUpdate() {
		//remove some of the targeting data. if these lists get too long, the search algo will slow down
		for (int i = 0; i < recently_targeted.Length; i++)
		{
			if (recently_targeted[i].Count < 1) continue;
			recently_targeted[i].RemoveRange(0, Mathf.CeilToInt(recently_targeted[i].Count * 0.5f));
		}
	}
	#endregion

	//search grid squares for enemies
	//currently searches in a silly pattern, I would fix that with more time

	//three lists, one for each team
	List<Unit>[] recently_targeted = new List<Unit>[3] { new List<Unit>(), new List<Unit>(), new List<Unit>() };
	List<Unit> fallbacks = new List<Unit>();

	//todo exclude recently searched areas from grids to search
	public Unit AdjacentEnemy(Vector2Int grid_position, Unit.Team team, int searchSize = 1) {
		fallbacks.Clear();
		for(int i = -searchSize; i < 1 + searchSize; i++) { //iterate x offsets
			for (int j = -searchSize; j < 1 + searchSize; j++) //iterate y offsets
			{
				Vector2Int test_position = grid_position + new Vector2Int(i, j);
				if (!grid.IsInBounds(test_position)) continue; //test for invalid position
				int cell_size = grid.cells[test_position.x, test_position.y].Count;
				//iterate through list
				for(int k = 0; k < cell_size; k++) {
					if (grid.cells[test_position.x, test_position.y][k].team != team) {
						//found an enemy!

						if (recently_targeted[(int)team].Contains(grid.cells[test_position.x, test_position.y][k])) {
							//this enemy has been recently targeted by a friendly unit and should be disregarded
							fallbacks.Add(grid.cells[test_position.x, test_position.y][k]);
							continue;
						}
						//mark this enemy as recently targeted and return
						recently_targeted[(int)team].Add(grid.cells[test_position.x, test_position.y][k]);
						return grid.cells[test_position.x, test_position.y][k];
					}
				}
			}
		}
		//no enemies found
		int fallback_count = fallbacks.Count;
		if(fallback_count > 0) {
			//use one of the ones thats been recently targeted
			return fallbacks[fallback_count - 1];
		}
		//no fallbacks, return null
		return null;
    }

	//very simple win loss check system
	//not terribly scalable, but not that bad either
	public void WinLossCheck() {
		if (LossCheck()) {
			Debug.Log("ending game, defeat");
			GameManager.instance.EndGame(false);
		}
		else if (WinCheck())
		{
			Debug.Log("ending game, victory");
			GameManager.instance.EndGame(true);
		}
	}
	bool LossCheck() { 
		foreach(Unit un in friendly_loses_if_dead) {
			if (un != null) return false;
		}
		return true;
    }
	bool WinCheck()
	{
		foreach (Unit un in enemy_loses_if_dead)
		{
			Debug.Log("wincheck: " + (un != null));
			if (un != null) return false;
		}
		return true;
	}
}
