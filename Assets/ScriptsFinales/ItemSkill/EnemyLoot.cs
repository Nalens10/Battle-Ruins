using UnityEngine;
using System.Collections.Generic; 

public class EnemyLoot : MonoBehaviour
{
    // Una pequeña estructura para emparejar el Ítem con su Probabilidad en el Inspector
    [System.Serializable]
    public struct DropConfig
    {
        public string nombreIdentificador; // Solo para que lo leas lindo en el inspector
        public ItemSkill itemASoltar;      // El ScriptableObject de la Cruz, Espada, etc.
        [Range(0f, 100f)]
        public float probabilidad;         // Su % de chance individual
    }

    [Header("Configuración del Contenedor")]
    public WorldItem worldItemPrefab; // El molde físico genérico del suelo

    [Header("Tabla de Botín (Lista de posibles drops)")]
    public List<DropConfig> listaDeDrops; 

    public void SoltarItem()
    {
        if (worldItemPrefab == null)
        {
            Debug.LogError(" ERROR: No has asignado el 'WorldItemPrefab' en el Zombie.");
            return;
        }

        if (listaDeDrops == null || listaDeDrops.Count == 0)
        {
            Debug.LogWarning(" El zombie murió pero su lista de drops está vacía.");
            return;
        }

        Vector3 posicionMuerte = transform.position;

        // Recorremos la lista completa ítem por ítem
        foreach (DropConfig drop in listaDeDrops)
        {
            if (drop.itemASoltar == null) continue;

            float dado = Random.Range(0f, 100f);

            // SI EL DADO SUPERA LA PROBABILIDAD, GANA 
            if (dado > drop.probabilidad) continue;

            // EL ITEM GANA EL SORTEO
            WorldItem clonItem = Instantiate(worldItemPrefab, posicionMuerte, Quaternion.identity);
            clonItem.SetItem(drop.itemASoltar);

            // Ajustamos a la baldosa táctica
            if (GameManager.current != null && GameManager.current.mapManager != null)
            {
                clonItem.transform.position = GameManager.current.mapManager.SnapToTile(posicionMuerte);
            }

            // Forzamos capa visual
            SpriteRenderer sr = clonItem.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sortingOrder = 10;

            Debug.Log($"[DROP LISTA] ¡ÉXITO! El zombie dropeó: {drop.itemASoltar.name} (Sacó {dado} de {drop.probabilidad}%)");
            
            //  Si QUERES que el zombie pueda soltar MÁS DE UN ÍTEM a la vez, borra la línea de abajo.
            // Si solo QUERES que suelte el primer ítem que gane el sorteo, DEJALO
            break; 
        }
    }
}