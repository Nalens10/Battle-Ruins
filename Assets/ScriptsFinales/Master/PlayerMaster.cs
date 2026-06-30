using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Les dejo en verde lo que hace ;)
public class PlayerMaster : Master
{
    protected MapDisplay mapDisplay;

    public override void BeginTurn()
    {
        this.RechargeAllUnitCreatures();
    }
}
