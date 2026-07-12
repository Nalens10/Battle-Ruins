using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Combo: Hielo + Fuego. Deja al jugador "mojado".
// Por ahora es un marcador pasivo (no hace daño ni cambia stats), pero al
// tener 'element' seteado, sirve como pieza para futuros combos
// (ej. si despues lo golpean con Rayo mientras esta mojado).
public class WetStatusCondition : StatusCondition
{
    protected override void ExecuteOnTurnStart(Stats targetStats)
    {

    }

    protected override void ExecuteStatsModifiers(Stats targetStats)
    {

    }
}
