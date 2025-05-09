using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
	/*
		I usually like handling most of my player input in specialized classes 
		just for convenience sake. If I want to come back to a project and figure
		out the control flow, a player input class is usually a good start. 
		
		I don't love the new unity input system. It just seems like another overly complex
		non-code method for setting things up, when doing it through modifiable keycodes
		is trivial. That said, I'm open to using whatever method is used. But in this timeframe,
		at this scale, the regular input system is probably the best choice. 
	*/
	public static PlayerControls instance;

	Vector2 click0_pos_world;
	public float acceptable_click_distance;

	public float drag_start_distance;
	public List<Unit> selected = new List<Unit>();

	bool dragging;
	public GameObject highlighter;

	private void Awake()
	{
		highlighter.SetActive(false);
		instance = this;
	}
	void SelectUnit(Unit unit) {
		selected.Add(unit);
		unit.ToggleSelect(true);
	}
	void DeselectAll() { 
		foreach(Unit un in selected) {
			un.ToggleSelect(false);
		}
		selected.Clear();
    }
	private void Update()
	{
		if(GameManager.gam_state == GameManager.State.Running) {
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				GameManager.instance.Pause(!GameManager.is_paused);
			}

			//Orders

			if (Input.GetMouseButtonDown(1) && GameManager.active_player_control) {
				Vector2 click_world_position = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 10);

				Vector2Int grid_coords = Map.instance.grid.CoordinatesFromPosition(click_world_position);
				Unit target = null;
				for (int i = 0; i < Map.instance.grid.cells[grid_coords.x, grid_coords.y].Count; i++)
				{
					if (Map.instance.grid.cells[grid_coords.x, grid_coords.y][i].team == Unit.Team.Friendly) continue;
					if (Vector2.Distance(Map.instance.grid.cells[grid_coords.x, grid_coords.y][i].transform.position, click_world_position) < acceptable_click_distance)
					{
						target = Map.instance.grid.cells[grid_coords.x, grid_coords.y][i];
						break;
					}
				}
				if(target != null) {
					foreach (Unit i in selected)
					{
						if (i.team != Unit.Team.Friendly) continue;
						i.DirectTarget(target);
					}
				}
				else {
					foreach (Unit i in selected)
					{
						if (i.team != Unit.Team.Friendly) continue;
						i.Direct(click_world_position);
					}
				}

			}


			//Selection
			if (Input.GetMouseButtonDown(0)) {
				//user clicked

				if(!Input.GetKey(KeyCode.LeftShift)) { //shift is add
					DeselectAll();
				}

				//select clicked unit (no colliders, so its an estimation)
				Vector2 click_world_position = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 10);
				click0_pos_world = click_world_position;
				Vector2Int grid_coords = Map.instance.grid.CoordinatesFromPosition(click_world_position);
				for(int i = 0; i < Map.instance.grid.cells[grid_coords.x, grid_coords.y].Count; i++) { 
					if(Vector2.Distance(Map.instance.grid.cells[grid_coords.x, grid_coords.y][i].transform.position, click_world_position) < acceptable_click_distance) {
						SelectUnit(Map.instance.grid.cells[grid_coords.x, grid_coords.y][i]);
						return;
					}
				}
			}
			if (Input.GetMouseButton(0))
			{
				//currently dragging mouse?
				Vector2 drag_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 10);
				if (Vector2.Distance(drag_pos, click0_pos_world) > drag_start_distance)
				{
					if (!dragging) {
						highlighter.SetActive(true);
						dragging = true;
					}
					highlighter.transform.position = (drag_pos + click0_pos_world) * 0.5f;
					Vector3 scale = Vector3.zero;
					scale.x = Mathf.Abs(drag_pos.x - click0_pos_world.x);
					scale.y = Mathf.Abs(drag_pos.y - click0_pos_world.y);
					highlighter.transform.localScale = scale;
				}
			}

			if (Input.GetMouseButtonUp(0)) {
				highlighter.SetActive(false);
				dragging = false;

				Vector2 click2_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 10);
				if(Vector2.Distance(click2_pos, click0_pos_world) > drag_start_distance) {
					//user has dragged over some area
					//get all the units in the selected zone

					float max_x = Mathf.Max(click0_pos_world.x, click2_pos.x);
					float max_y = Mathf.Max(click0_pos_world.y, click2_pos.y);
					float min_x = Mathf.Min(click0_pos_world.x, click2_pos.x);
					float min_y = Mathf.Min(click0_pos_world.y, click2_pos.y);
					//this could use the spatial partitioning grid, but its sorta non-trivial
					for (int i = 0; i < Map.instance.all_units.Count; i++) {
						Unit un = Map.instance.all_units[i];
						if (un.team != Unit.Team.Friendly) continue;
						if (un.transform.position.x > max_x) continue;
						if (un.transform.position.y > max_y) continue;
						if (un.transform.position.x < min_x) continue;
						if (un.transform.position.y < min_y) continue;
						SelectUnit(un);
					}
				}
			}

		}
	}
}
