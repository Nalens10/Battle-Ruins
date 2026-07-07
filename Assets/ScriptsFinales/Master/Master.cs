using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Master : MonoBehaviour
{
    
    public string masterName = "";

    protected List<UnitCreature> unitCreatures = new List<UnitCreature>();

    public abstract void SpawnUnitCreatures(List<Vector3> spawnPoints);
  

    protected void BeginTurnToAllUnitCreatures()
    {
        List<UnitCreature> units = new List<UnitCreature>(this.unitCreatures);

        foreach (var unitCreature in units)
        {
            if (unitCreature != null)
            {
                unitCreature.BeginTurn();
            }
        }
    }

    protected void CreateUnitCreature(GameObject unitCreaturePrfb, Vector3 worldPosition)
    {
        GameObject go = Instantiate(unitCreaturePrfb);
        UnitCreature unitCreature = go.GetComponent<UnitCreature>();

        unitCreature.transform.position = worldPosition;

        this.AdoptUnitCreature(unitCreature);
    }

    public void AdoptUnitCreature(UnitCreature unitCreature)
    {
        unitCreature.master = this;
        this.unitCreatures.Add(unitCreature);

        GameManager.current.EmplaceUnitCreature(unitCreature, unitCreature.transform.position);
    }

    public void OnUnitCreatureDeath(UnitCreature unitCreature)
    {
        this.unitCreatures.Remove(unitCreature);

        GameManager.current.OnUnitCreatureDeath(unitCreature);
    }

    public bool HasAliveUnitCreatures()
    {
        return this.unitCreatures.Count != 0;
    }


    public abstract void BeginTurn();
}
