using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Master : MonoBehaviour
{
    public GameObject[] unitCreatureTeamPrfbs;

    public string masterName = "";

    protected List<UnitCreature> creatures = new List<UnitCreature>();

    public void SpawnUnitCreatures(List<Vector3> spawnPoints)
    {
        for (int i = 0; i < this.unitCreatureTeamPrfbs.Length; i++)
        {
            if (i >= spawnPoints.Count)
            {
                Debug.Log("No more spawn points!");
                break;
            }

            GameObject prfb = this.unitCreatureTeamPrfbs[i];
            this.CreateUnitCreature(prfb, spawnPoints[i]);
        }
    }

    protected void BeginTurnToAllUnitCreatures()
    {
        foreach (var unitCreature in this.creatures)
        {
            unitCreature.BeginTurn();
        }
    }

    protected void CreateUnitCreature(GameObject unitCreaturePrfb, Vector3 worldPosition)
    {
        GameObject go = Instantiate(unitCreaturePrfb);
        UnitCreature unitCreature = go.GetComponent<UnitCreature>();
        unitCreature.master = this;

        GameManager.current.EmplaceUnitCreature(unitCreature, worldPosition);

        this.creatures.Add(unitCreature);
    }

    public abstract void BeginTurn();
}
