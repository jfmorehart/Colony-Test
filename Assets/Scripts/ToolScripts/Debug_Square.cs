using UnityEngine;

public class Debug_Square : MonoBehaviour
{
    Vector2Int grid_position;
    SpriteRenderer renderer;
    bool highlighted;

	private void Start()
	{
        renderer = GetComponent<SpriteRenderer>();
        grid_position = Map.instance.grid.CoordinatesFromPosition(transform.position);
        Toggle(false);
	}
	
    void Update()
    {
        Vector2 wpos = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 10);
        if(grid_position == Map.instance.grid.CoordinatesFromPosition(wpos)) {
            if (highlighted) {
                //show debug information
            }
            else {
                Toggle(true);
	        }
	    }else if (highlighted) {
            Toggle(false);
	    }

	}
	//renderer's individual colors channels are unmodifiable, so we need our own copy
	Color color;

	void Toggle(bool on) {
        highlighted = on;

        color = on ? Color.green : Color.white;
        color.a = 0.04f;
        renderer.color = color;
    }
}
