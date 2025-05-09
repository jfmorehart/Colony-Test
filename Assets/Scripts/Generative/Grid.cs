using UnityEngine;
using System.Collections.Generic;

public class Grid<TCell> {

	/*
		This class is a generic implementation of a grid data structure
	I took some liberties with the architecture because the suggested functions seemed to imply a
	fairly bloated design for a simple 2D grid system. Perhaps it would have made sense in a more complex
	graph system that required adjacency matrices to be generated for each added cell. 
	For a small assignment scope like this, it didn't make sense to get fancy with it.
	*/

	public Vector2Int dimensions;
	public List<TCell>[,] cells;

	public float cell_spacer = 1;
	Vector2 grid_offset;

	public Grid(int xSize, int ySize) {

		//center the grid
		grid_offset = new Vector2(-xSize * 0.5f, -ySize * 0.5f) * cell_spacer;
		dimensions = new Vector2Int(xSize, ySize);
		//prepare the lists
		cells = new List<TCell>[xSize, ySize];
		for(int i = 0; i < xSize; i++) {
			for (int j = 0; j < ySize; j++)
			{
				cells[i, j] = new List<TCell>();
				Debug.Log(i + " " + j + " " + cells[i, j].Count);
			}
		}
    }

	public Vector2Int RandomPositionOnGrid() {
		return new Vector2Int(Random.Range(0, dimensions.x), Random.Range(0, dimensions.y));
    }

	public void RegisterUnit(int i, int j, TCell unit) {
		cells[i, j].Add(unit);
    }
	public Vector2Int RegisterUnit(Vector2 worldPosition, TCell unit) {
		Vector2Int gridPosition = CoordinatesFromPosition(worldPosition);
		cells[gridPosition.x, gridPosition.y].Add(unit);
		Debug.Log("world:" + worldPosition + " grid:" + gridPosition);
		return gridPosition;
	}

	public void DeRegisterUnit(int i, int j, TCell unit) {
		cells[i, j].Remove(unit);
	}

	public void Move(Vector2Int old_position, TCell unit, Vector2Int new_position)
	{
		DeRegisterUnit(old_position.x, old_position.y, unit);
		RegisterUnit(new_position.x, new_position.y, unit);
	}

	public Vector2Int CoordinatesFromPosition(Vector2 xypos) {

		xypos -= grid_offset;
		int x = Mathf.RoundToInt(xypos.x / cell_spacer);
		int y = Mathf.RoundToInt(xypos.y / cell_spacer);

		//correct bounds
		if (x < 0) x = 0;
		if (x >= dimensions.x) x = dimensions.x - 1;
		if (y < 0) y = 0;
		if (y >= dimensions.y) y = dimensions.y - 1;
		return new Vector2Int(x, y);
	}

	public Vector2 PositionFromCoordinates(int x, int y) {
		return new Vector2(x * cell_spacer + grid_offset.x, y * cell_spacer + grid_offset.y);
    }
	public Vector2 PositionFromCoordinates(Vector2Int coordinates)
	{
		return new Vector2(coordinates.x * cell_spacer + grid_offset.x, coordinates.y * cell_spacer + grid_offset.y);
	}

	public bool IsInBounds(Vector2Int gridPos) {

		if (gridPos.x < 0) return false;
		if (gridPos.x >= dimensions.x) return false;
		if (gridPos.y < 0) return false;
		if (gridPos.y >= dimensions.y) return false;
		return true;
	}
}
