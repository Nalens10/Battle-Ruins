using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Les dejo en verde lo que hace ;)
public class IAMaster : Master
{
    public GameObject[] enemyPrefabs;

    protected UnitCreature lastTarget;

    public override void BeginTurn()
    {
        this.lastTarget = null;

        this.BeginTurnToAllUnitCreatures();
        StartCoroutine(this.TurnRutine());
    }

    private Vector3 GenerateUnitCreatureTarget(UnitCreature unitCreature)
    {
        Stats stats = unitCreature.GetCurrentStats();
        List<Vector3> reachArea = GameManager.current.mapManager.PredictAreaFor(
            unitCreature.transform.position,
            stats.speed
        );
        List<UnitCreature> enemies = GameManager.current.GetEnemyUnitCreaturesInArea(
            reachArea,
            this
        );

        if (enemies.Count != 0)
        {
            // Obtenemos el enemigo más cercano
            UnitCreature nearest = enemies[0];
            float lastDistance = 9999;

            foreach (var enemy in enemies)
            {
                float distance = Vector3.Distance(enemy.transform.position, unitCreature.transform.position);
                if (distance < lastDistance)
                {
                    lastDistance = distance;
                    nearest = enemy;
                }
            }

            this.lastTarget = nearest;
        }

        if (this.lastTarget != null)
        {
            return this.GenerateRandomTargetInArea(unitCreature, this.lastTarget.transform.position, 1f);
        }

        return this.GenerateRandomTargetInArea(unitCreature, unitCreature.transform.position, stats.speed);
    }

    private Vector3 GenerateRandomTargetInArea(UnitCreature unitCreature, Vector3 center, float distance)
    {
        int attempts = 0;

        while (attempts < 32)
        {
            attempts++;

            var offset = new Vector3(
                Random.Range(-distance, distance),
                Random.Range(-distance, distance)
            );

            Vector3 target = center + offset;

            if (GameManager.current.CanMoveUnitCreatureTo(unitCreature, target))
            {
                return target;
            }
        }

        // No nos movemos.
        return center;
    }

    private IEnumerator TurnRutine()
    {
        foreach (var unitCreature in this.unitCreatures)
        {
            Vector3 target = this.GenerateUnitCreatureTarget(unitCreature);

            GameManager.current.MoveUnitCreatureTo(unitCreature, target);

            while (unitCreature.isMoving)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            if (this.lastTarget != null)
            {
                ItemSkill[] itemSkills = unitCreature.GetItemSkills();
                int rndIndex = Random.Range(0, itemSkills.Length);
                ItemSkill selectedItemSkill = itemSkills[rndIndex];

                GameManager.current.TryToPerformItemSkillAtPoint(unitCreature, selectedItemSkill, this.lastTarget.transform.position);
            }

            yield return new WaitForSeconds(0.5f);
        }

        GameManager.current.NextTurn();
    }

    public override void SpawnUnitCreatures(List<Vector3> spawnPoints)
    {
        int amount = Mathf.Min(enemyPrefabs.Length, spawnPoints.Count);

        for (int i = 0; i < amount; i++)
        {
            CreateUnitCreature(enemyPrefabs[i], spawnPoints[i]);
        }

        if (enemyPrefabs.Length > spawnPoints.Count)
        {
            Debug.LogWarning("No hay suficientes SpawnPoints para todos los enemigos.");
        }
    }
}
