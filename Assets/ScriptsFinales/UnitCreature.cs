using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Les dejo en verde lo que hace ;)
public class UnitCreature : MonoBehaviour
{
    // Posición local de la criatura en el mapa
    public Vector2Int localPosition;

    public GameObject selectionIndicator;

    public float movementSpeed = 4f;

    private bool isSelected = false;

    // stats
    public int speed = 4;

    public int maxEnergy = 2;
    private int energy;

    void Start()
    {
        this.Recharge();

        this.SetSelectionStatus(false);
    }

    public void Recharge()
    {
        this.UpdateEnergy(this.maxEnergy);
    }

    public int CurrentMaxDistance()
    {
        return this.speed * this.energy;
    }

    private void UpdateEnergy(int e)
    {
        this.energy = e;
        UnitCreatureUI.current.DisplayEnergy(e);
    }


    public void SetSelectionStatus(bool isSelected)
    {
        this.selectionIndicator.SetActive(isSelected);

        this.isSelected = isSelected;

        if (this.isSelected)
        {
            UnitCreatureUI.current.Show();
            UnitCreatureUI.current.DisplayEnergy(this.energy);
        }
        else
        {
            UnitCreatureUI.current.Hide();
        }
    }

    public void FollowPath(Vector3[] worldPath)
    {
        // Detenemos cualquier rutina de seguimiento de ruta en curso antes de iniciar una nueva
        StopAllCoroutines();
        StartCoroutine(this.FollowPathRutine(worldPath));
    }

    private IEnumerator FollowPathRutine(Vector3[] worldPath)
    {
        // Iteramos a través de cada punto en la ruta del mundo
        foreach (var target in worldPath)
        {
            // Calculamos la longitud de la ruta que podemos recorrer, limitada por la energía actual
            int pathLength = Mathf.Min(this.CurrentMaxDistance(), worldPath.Length);
            int cost = this.GetEnergyCostForPathLength(pathLength);

            this.UpdateEnergy(this.energy - cost);

            // Movemos la criatura hacia el objetivo actual en la ruta
            for (int i = 0; i < pathLength; i++)
            {

                float percent = 0;

                Vector3 start = this.transform.position;

                while (percent < 1f)
                {
                    this.transform.position = Vector3.Lerp(start, worldPath[i], percent);

                    percent += Time.deltaTime * this.movementSpeed;
                    yield return null;
                }

                this.transform.position = worldPath[i];
            }
             
        }
    }

    public int GetEnergyCostForPathLength(int length)
    {
        // Calculamos el costo de energía basado en la longitud del camino y la velocidad de la criatura

        int cost = Mathf.CeilToInt(length / (float)this.speed);

        return cost;
    }
}
