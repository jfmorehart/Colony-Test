using System.Collections.Generic;
using UnityEngine;

public class UnitFactory : MonoBehaviour
{
	public enum UnitType {Flag, Ant, Aphid, Beetle, LadyBug}

	public GameObject[] prefab_array;
	public InsectData[] data_array;

	public Unit SpawnUnit(UnitType kind) {
		Unit u = Instantiate(prefab_array[(int)kind], transform).GetComponent<Unit>();
		SetValuesFromObject(u, data_array[(int)kind]);
		return u;
    }

	public List<Unit> SpawnRadiallyAroundPoint(int numberOf, UnitType unittype, Vector2 point, float radius) {

		//heap allocate in case caller needs this not to be overwritten by subsequent calls
		List<Unit> unitlistReference = new List<Unit>(); 

		for(int i = 0; i < numberOf; i++) {
			unitlistReference.Add(SpawnUnit(unittype));

			//presently random, but could be angle-based if consistency was desired
			unitlistReference[i].transform.position = point + (Random.insideUnitCircle).normalized * radius;
			//these positions will be snapped onto the grid anyhow
		}
		return unitlistReference;
    }

	public void SetValuesFromObject(Unit unit, InsectData data) {
		//a little silly to be honest, but it works
		if(unit is Soldier) {
			data.ImpartData(unit as Soldier);
		}
		else {
			data.ImpartData(unit);
		}
	}
}
