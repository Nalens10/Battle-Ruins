using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ItemSkill : MonoBehaviour
{
    public string itemSkillName;

    public Sprite worldSprite;

    public int maxUses = 3;

    public int cost = 1;

    public float range = 1f;

    public float area = 0;

    public float distancePenalizationMultiplier = 0.1f;

    public GameObject vfx;

    protected IEffect[] effects;

    public ElementalType elementalType;

    public float currentDistancePenalization { get; protected set; }

    protected SpawnEffect spawnEffect;
    public bool isSpawner = false;

    void Awake()
    {
        this.effects = this.GetComponents<IEffect>();
        this.spawnEffect = this.GetComponent<SpawnEffect>();
    }

    public void ResolveForReceiver(UnitCreature emitter, UnitCreature receiver)
    {
        Debug.Log("==========");
        Debug.Log("Skill: " + itemSkillName);

        // ESTO ES NUEVO: Le dice al personaje que LANZA la habilidad que haga la ANIMACIOOOOOOON
        // Buscamos el componente Animator en el emisor o sus hijos visuales
        Animator emitterAnimator = emitter.GetComponentInChildren<Animator>();
        if (emitterAnimator != null)
        {
            emitterAnimator.SetTrigger("attack");
        }
        
        if (effects == null)
        {
            Debug.LogError("effects == NULL");
            return;
        }

        Debug.Log("Cantidad de efectos: " + effects.Length);

        for (int i = 0; i < effects.Length; i++)
        {
            Debug.Log("Effect " + i + ": " + effects[i]);

            if (effects[i] == null)
            {
                Debug.LogError("El efecto " + i + " es NULL");
                return;
            }
        }


        float tileDistance = Vector3.Distance(emitter.transform.position, receiver.transform.position);
        this.currentDistancePenalization = (-tileDistance + 2) * this.distancePenalizationMultiplier;

        if (this.effects.Length == 0)
        {
            Debug.LogError($"This skill ({this.itemSkillName}) has no effects!");
            return;
        }

        bool canHit = this.CalculateIfCanHit(emitter.GetCurrentStats(), receiver.GetCurrentStats());
        if (canHit)
        {
            foreach (var effect in this.effects)
            {
                Debug.Log(effect);

                effect.Resolve(emitter, receiver);
            }
        }
        else
        {
            MessageManager.current.Send(new ItemSkillMissMessage(this, receiver));
        }

        if (this.vfx != null)
        {
            GameObject go = Instantiate(this.vfx, receiver.transform.position, Quaternion.identity);
            Destroy(go, 2f);
        }
    }

    public void ResolveAsSpawner(UnitCreature emitter, List<Vector3> area)
    {
        foreach (var point in area)
        {
            this.spawnEffect.ResolveAtPoint(emitter, point);

            if (this.vfx != null)
            {
                GameObject go = Instantiate(this.vfx, point, Quaternion.identity);
                Destroy(go, 2f);
            }
        }
    }

    private bool CalculateIfCanHit(Stats eStats, Stats rStats)
    {
        float hitChance = 1f - Mathf.Max(rStats.evasion - eStats.accuracy, 0) / (float)rStats.evasion;
        hitChance += this.currentDistancePenalization;

        float dice = Random.Range(0f, 1f);

        return dice < hitChance;
    }
}
