// ESTO ES NUEVO, SI SE ROMPE SE ELIMINA Y FUE

public class UnitDeathMessage
{
    public UnitCreature DeadUnit { get; private set; }

    public UnitDeathMessage(UnitCreature deadUnit)
    {
        this.DeadUnit = deadUnit;
    }
}