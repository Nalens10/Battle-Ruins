using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;

// Les dejo en verde lo que hace ;)
public class GameManager : MonoBehaviour
{
    public GameObject unitCreaturePrfb;
    public int creatureCount = 3;

    public TextAsset mapData;

    void Start()
    {
        // Creamos un mapa a partir de los datos de texto proporcionados en el archivo mapData
        Map map = Map.CreateWithStringData(this.mapData.text);

        // Cargamos el mapa desde un archivo de texto
        MapDisplay display = GameObject.FindObjectOfType<MapDisplay>();
        display.RenderMapData(map);

        // Instanciamos la criatura en el mapa
        this.SpawnRandomCreatures(3, map, display);
    }
    
    private void SpawnRandomCreatures(int count, Map map, MapDisplay display)
    {
        // Contador de criaturas restantes por instanciar
        int creatureCount = count;
        int attempts = 0;

        // Lista para almacenar las posiciones ya utilizadas para evitar duplicados
        List<Vector2Int> usedPositions = new List<Vector2Int>();

        // Mientras queden criaturas por instanciar y no se hayan superado los intentos máximos
        while (creatureCount > 0 && attempts < 100)
        {
            // Incrementamos el contador de intentos para evitar bucles infinitos
            attempts++;

            // Generamos una posición aleatoria dentro de los límites del mapa
            int x = Random.Range(0, map.width);
            int y = Random.Range(0, map.height);
            if (map.GetTileType(x, y) != TileType.GROUND)
            continue;
            
            bool positionUsed = false;

            // Verificamos si la posición generada ya ha sido utilizada
            foreach (var pos in usedPositions)
            {
                if (pos.x == x && pos.y == y)
                {
                    positionUsed = true;
                    break;
                }
            }
            
            if (positionUsed) continue;
            
            Vector2Int spawnPoint = new Vector2Int(x, y);
            usedPositions.Add(spawnPoint);
            
            GameObject go = Instantiate(this.unitCreaturePrfb);
            
            UnitCreature creature = go.GetComponent<UnitCreature>();
            display.EmplaceUnitCreature(creature, spawnPoint);
            
            creatureCount--;
        }
    }
}
