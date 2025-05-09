using UnityEngine;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;

public class GameManager : MonoBehaviour
{

	/*
		The game manager is the entry point for this little demo
		It's main job is handling the UI and spawning the Map, 
		which in turn, starts the game
	*/
	public static GameManager instance;

	public static bool active_player_control;
	public void SetPlayerControl(bool playercontrol) { active_player_control = playercontrol; }

	public enum State { NotStarted, Running, Over}
	//running includes paused
	//imo it's easier that way
	public static bool is_paused;

	//using a bool for pausing makes toggling it trivial

	public static State gam_state;

	//control over UI
	public GameObject menu_object;
	public GameObject end_object;
	public TMP_Text end_title;

	//the game manager owns the map, which owns everything else
	//easy loading/unloading
	public Map current_map;
	public GameObject map_prefab;

	private void Awake()
	{
		gam_state = State.NotStarted;
		if(instance != this && instance != null) {
			Debug.LogError("Multiple managers!");
		}
		instance = this;
		Pause(true);
	}
	public void QuitGame() {
		Application.Quit();
    }
	public void StartGame() {
		if (current_map != null) Destroy(current_map.gameObject);

		Pause(false);
		end_object.SetActive(false);
		gam_state = State.Running;

		//spawn a map
		current_map = Instantiate(map_prefab, transform).GetComponent<Map>();
	}

	public void EndGame(bool we_won) {
		gam_state = State.Over;
		if (we_won) {
			end_title.text = "Victory";
		}
		else {
			end_title.text = "Defeat";
		}
		is_paused = true;
		Time.timeScale = 0;
		end_object.SetActive(true);
	}

	public void Pause(bool paused) {

		is_paused = paused;
		Time.timeScale = paused ? 0 : 1;
		menu_object.SetActive(paused);
	}
}
