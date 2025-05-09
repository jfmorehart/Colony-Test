using UnityEngine;

using TMPro;

public class DebugMenu : MonoBehaviour
{
	public TMP_Text bug_name;
	public TMP_Text health;
	public TMP_Text speed;
	public TMP_Text state;

	public bool debug_enabled;
	public GameObject menu_panel;
	public void SetDebugEnable(bool on) {
		debug_enabled = on;
		menu_panel.SetActive(on);

    }

	private void Update()
	{
		if (debug_enabled) {
			UpdateDebugMenu();
		}
	}

	public void UpdateDebugMenu() {
		int n = PlayerControls.instance.selected.Count;
		if(n == 0) {
			bug_name.text = "None";
			health.text = "";
			speed.text = "";
			state.text = "";
			return;
		}else if(n > 1) {
			bug_name.text = "Multiple";
			health.text = "";
			speed.text = "";
			state.text = "";
			return;
		}
		if (PlayerControls.instance.selected[0] == null) return;

		if (PlayerControls.instance.selected[0] is Soldier) {
			bug_name.text = PlayerControls.instance.selected[0].name;
			health.text = "health = " + PlayerControls.instance.selected[0].health;
			Soldier s = PlayerControls.instance.selected[0] as Soldier;
			speed.text = "speed = " + s.speed; ;
			state.text = "state = " + s.debug_state;
		}
		else {
			bug_name.text = PlayerControls.instance.selected[0].name;
			health.text = "health = " + PlayerControls.instance.selected[0].health;
			speed.text = "";
			state.text = "";
		}


	}

}
