using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

// Les dejo en verde lo que hace ;)
public class UnitCreature : MonoBehaviour
{

    public GameObject selectionIndicator;

    public float movementSpeed = 4f;

    private bool isSelected = false;

    public Master master;

    public Stats stats;

    private List<StatusCondition> conditions = new List<StatusCondition>();

    public List<ItemInstance> inventory = new List<ItemInstance>();

    public bool isMoving { get; protected set; }

    public int maxInventory = 4;

    public int currentUniqueCooldown;

    public UniqueItemSkill uniqueSkill;

    // Nuevas variables para la animacion 

    private Animator animator;
    private bool isDead = false;


    void Awake() {
        // Busca el componente Animator en este objeto o en sus hijos
        animator = GetComponentInChildren<Animator>();
    }
    
    void Start()
    {
        this.isMoving = false;
        this.SetSelectionStatus(false);

    }

    public Stats GetBaseStats()
    {
        return this.stats.Clone();
    }

    public Stats GetCurrentStats()
    {
        Stats modedStats = this.stats.Clone();

        foreach (var cond in this.conditions)
        {
            cond.ApplyStatsModifiers(modedStats);
        }

        return modedStats;
    }

    public StatusCondition[] GetCurrentStatusConditions()
    {
        return this.conditions.ToArray();
    }

    public StatusCondition GetStatusCondition(System.Type type)
    {
        foreach (StatusCondition condition in conditions)
        {
            if (condition.GetType() == type)
            {
                return condition;
            }
        }

        return null;
    }

    public void ModifyHealth(int amount)
    {
        int newHP = this.stats.hp + amount;

        this.stats.hp = Mathf.Clamp(newHP, 0, this.stats.maxhp);

        if (this.stats.hp == 0)
        {
            TriggerDeathAnimation(); // ESTO ES NUEVO, dispara la animacion antes dea visar al master, creo
            this.master.OnUnitCreatureDeath(this);
        }
    }

    public int DamageWithClamp(int amount)
    {
        if (isDead) return 0; // Si ya está muerto, no recibe daño

        int targetHp = Mathf.Clamp(this.stats.hp - amount, 0,this.stats.maxhp);

        int damageTaken = this.stats.hp - targetHp;

        this.stats.hp = targetHp;

        if (this.stats.hp <= 0)
        {
            TriggerDeathAnimation(); // ESTO ES NUEVO: Dispara la animación antes de avisar al master
            this.master.OnUnitCreatureDeath(this);
        }

        return damageTaken;
    }

    public int Heal(int amount)
    {
        // ES NUEVO
        if (isDead) return 0;

        int targetHp = Mathf.Clamp(this.stats.hp + amount, 1, this.stats.maxhp);
        int healed = targetHp - this.stats.hp;

        this.stats.hp = targetHp;

        // ESTO ES NUEVO LLAMA A LA ANIMACION ATACAR
        if (healed > 0 && animator != null)
        {
            animator.SetTrigger("attack"); 
        }

        return healed;
    }

    public void BeginTurn()
    {
        if (currentUniqueCooldown > 0) currentUniqueCooldown--;

        this.UpdateEnergy(this.stats.maxEnergy);

        for (int i = 0; i < this.conditions.Count; i++)
        {
            StatusCondition cond = this.conditions[i];
            cond.ApplyOnTurnStart(this.stats); 
            cond.ConsumeOneTurn();

            if (cond.isDepleted)
            {
                this.conditions.RemoveAt(i);
            }
        }
    }

    public void AddStatusCondition(StatusCondition condition)
    {
        this.conditions.Add(condition);

        condition.transform.position = this.transform.position;
        condition.transform.SetParent(this.transform);
    }

    public int CurrentMaxDistance()
    {
        return this.stats.speed * this.stats.energy;
    }

    private void UpdateEnergy(int e)
    {
        this.stats.energy = e;
        MessageManager.current.Send(new UnitCreatureUpdatedMessage(this));
    }

    public bool CanExecuteItemSkill(ItemSkill itemSkill)
    {
        return this.stats.energy >= itemSkill.cost;
    }

    public void ConsumeEnergyFor(ItemSkill itemSkill)
    {
        this.UpdateEnergy(this.stats.energy - itemSkill.cost);
    }

    public void SetSelectionStatus(bool isSelected)
    {
        this.selectionIndicator.SetActive(isSelected);
        this.isSelected = isSelected;
    }

    public void FollowPath(Vector3[] worldPath)
    {
        if (isDead) return; // Si está muerto no se mueve
        StopAllCoroutines();
        StartCoroutine(this.FollowPathRutine(worldPath));
    }

    private IEnumerator FollowPathRutine(Vector3[] worldPath)
    {
        this.isMoving = true;

        // SE ACTIVA LA ANIMACION DE CAMINAR
        if (animator != null) animator.SetBool("isMoving", true);

        int pathLength = Mathf.Min(this.CurrentMaxDistance(), worldPath.Length);
        int cost = this.GetEnergyCostForPathLength(pathLength);

        this.UpdateEnergy(this.stats.energy - cost);

        for (int i = 0; i < pathLength; i++)
        {
            Vector3 target = worldPath[i];

            float percent = 0;

            Vector3 start = this.transform.position;

            while (percent < 1f)
            {
                this.transform.position = Vector3.Lerp(start, target, percent);

                percent += Time.deltaTime * this.movementSpeed;
                yield return null;
            }

            this.transform.position = target;
            MessageManager.current.Send(new UnitCreatureMovedMessage(this));
        }

        this.isMoving = false;


        // SE VUELVE A LA ANIMACION DE IDLE, CREO
        if (animator != null) animator.SetBool("isMoving", false);

        // Buscar un item en la casilla donde termin�
        WorldItem worldItem = GameManager.current.FindItemAtPosition(transform.position);

        if (worldItem != null)
        {
            if (AddItem(worldItem.itemSkill))
            {
                Debug.Log("Recogi�: " + worldItem.itemSkill.itemSkillName);

                ItemSpawnerManager spawner = FindObjectOfType<ItemSpawnerManager>();

                if (spawner != null)
                {
                    spawner.RemoveItem(worldItem);
                }

                Destroy(worldItem.gameObject);
            }
            else
            {
                Debug.Log("Inventario lleno.");
            }
        }
    }

    public int GetEnergyCostForPathLength(int length)
    {
        int cost = Mathf.CeilToInt(length / (float)this.stats.speed);

        return cost;
    }

    public ItemSkill[] GetItemSkills()
    {
        return this.GetComponentsInChildren<ItemSkill>();
    }

    public bool AddItem(ItemSkill item)
    {
        ItemSkill copy = Instantiate(item);

        inventory.Add(new ItemInstance()
        {
            itemSkill = copy,
            remainingUses = copy.maxUses
        });

        return true;
    }

    public void UseInventoryItem(ItemInstance item)
    {
        item.ConsumeUse();

        if (item.IsDepleted)
        {
            inventory.Remove(item);
        }

        MessageManager.current.Send(new UnitCreatureUpdatedMessage(this));
    }

    public bool CanUseUniqueSkill()
    {
        return currentUniqueCooldown <= 0;
    }

    public void ConsumeUniqueSkill()
    {
        currentUniqueCooldown = uniqueSkill.cooldownTurns;
    }
    //  NUEVO: Ejecuta el trigger 'die' en el animator
    private void TriggerDeathAnimation()
    {
        if (isDead) return;
        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger("die");
        }
    }
}
