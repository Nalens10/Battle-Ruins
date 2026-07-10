using UnityEngine;

public class EnemyLoot : MonoBehaviour
{
    [Header("Configuración del Botín")]
    // 1. El Prefab base "WorldItem" (el contenedor vacío que tiene el script WorldItem)
    public WorldItem worldItemPrefab; 
    
    // 2. El objeto de datos de la Cruz Bendita (el ScriptableObject del ítem/habilidad)
    public ItemSkill itemASoltar; 
    
    

    [Range(0f, 100f)]
    public float probabilidadDeDrop = 100f;

    public void SoltarItem()
    {
        if (worldItemPrefab == null || itemASoltar == null)
        {
            Debug.LogError("[DROP ZOMBIE] ERROR: Asegúrate de asignar el 'WorldItemPrefab' y el 'ItemASoltar' en el Inspector.");
            return;
        }

        float dado = Random.Range(0f, 100f);
        if (dado <= probabilidadDeDrop)
        {
            Vector3 posicionMuerte = transform.position;

            // Instanciamos el contenedor vacío en el mapa
            WorldItem clonItem = Instantiate(worldItemPrefab, posicionMuerte, Quaternion.identity);

            // ¡AQUÍ ESTÁ LA MAGIA! Le pasamos los datos de la Cruz para que cargue su sprite
            clonItem.SetItem(itemASoltar);

            // Lo ajustamos a la baldosa del mapa correspondiente
            if (GameManager.current != null && GameManager.current.mapManager != null)
            {
                clonItem.transform.position = GameManager.current.mapManager.SnapToTile(posicionMuerte);
            }

            // Forzamos el Order in Layer para asegurarnos de que se dibuje por encima del suelo
            SpriteRenderer sr = clonItem.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = 10;
            }

            Debug.Log($"[DROP SEGURO] ¡Éxito! Se creó el WorldItem con el sprite de {itemASoltar.name} en el tablero.");
        }
    }
}