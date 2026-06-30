using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Les dejo en verde lo que hace ;)
public class IAMaster : Master
{
    public override void BeginTurn()
    {
        this.RechargeAllUnitCreatures();
        StartCoroutine(this.TurnRutine());
    }

    private IEnumerator TurnRutine()
    {
        foreach (var creature in this.creatures)
        {
            int attempts = 0;

            while (attempts < 32)
            {
                attempts++;

                var offset = new Vector3(
                    Random.Range(-5, 5),
                    Random.Range(-5, 5)
                );

                Vector3 target = creature.transform.position + offset;

                if (GameManager.current.mapManager.IsAGroundTile(target) == false)
                {
                    continue;
                }

                GameManager.current.MoveUnitCreatureTo(creature, target);
                break;
            }

            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(1);
        GameManager.current.NextTurn();
    }
}
