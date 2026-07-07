using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEffect : MonoBehaviour
{
    public GameObject prfb;

    [Header("Necesita un tile sin criaturas")]
    public bool requiresEmptySpace = false;

    public void ResolveAtPoint(UnitCreature emitter, Vector3 point)
    {
        if (this.requiresEmptySpace)
        {
            UnitCreature posibleUnitCreature = GameManager.current.GetUnitCreatureAtPosition(point);
            if (posibleUnitCreature != null)
            {
                return;
            }
        }
        GameObject go = Instantiate(this.prfb, point, Quaternion.identity);

        UnitCreature posibleNewUnitCreature = go.GetComponent<UnitCreature>();
        if (posibleNewUnitCreature != null)
        {
            // Es una invocación.
            emitter.master.AdoptUnitCreature(posibleNewUnitCreature);
        }
    }
}
