using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Les dejo en verde lo que hace ;)
public class CameraMove : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;

    [Header("Zoom")]
    public float zoomSpeed = 35f;
    public float maxZoom = 10;
    public float minZoom = 4;

    private Camera cameraComp;

    void Start()
    {
       cameraComp = GetComponent<Camera>();
    }

    void Update()
    {
        // Para el movimiento
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (h != 0 || v != 0)
        {
            // Normalizamos el vector de dirección para que la velocidad sea constante en todas las direcciones
            var direction = (new Vector3(h, v, 0)).normalized;
            transform.position = transform.position + (direction * moveSpeed * Time.deltaTime);
        }

        // Para el zoom
        if (Input.mouseScrollDelta.y != 0)
        {
            // Calculamos el cambio de zoom basado en la rueda del ratón, la velocidad de zoom y el tiempo entre frames
            float zoomDelta = (Input.mouseScrollDelta.y * (-1)) * zoomSpeed * Time.deltaTime;

            float nextZoom = cameraComp.orthographicSize + zoomDelta;
            cameraComp.orthographicSize = Mathf.Clamp(nextZoom, minZoom, maxZoom);
        }
    }
}
