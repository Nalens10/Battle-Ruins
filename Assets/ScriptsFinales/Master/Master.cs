using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Master : MonoBehaviour
{
    public GameObject unitCreaturePrfb;

    public string masterName = "";

    protected List<UnitCreature> creatures = new List<UnitCreature>();

    public void SpawnCreatures(List<Vector3> spawnPoints)
    {
        foreach (var point in spawnPoints)
        {
            this.CreateUnitCreature(this.unitCreaturePrfb, point);
        }
    }

    protected void RechargeAllUnitCreatures()
    {
        foreach (var unitCreature in this.creatures)
        {
            unitCreature.Recharge();
        }
    }

    protected void CreateUnitCreature(GameObject unitCreaturePrfb, Vector3 worldPosition)
    {
        GameObject go = Instantiate(unitCreaturePrfb);
        UnitCreature unitCreature = go.GetComponent<UnitCreature>();
        unitCreature.master = this;

        GameManager.current.EmplaceCreature(unitCreature, worldPosition);

        this.creatures.Add(unitCreature);
    }

    public abstract void BeginTurn();
}
