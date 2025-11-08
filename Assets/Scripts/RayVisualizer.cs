using UnityEngine;
using Oculus.Interaction;

public class RayVisualizer : MonoBehaviour
{
    public RayInteractor rayInteractor;
    public LineRenderer lineRenderer;
    public float rayWidth = 0.01f;
    public Color rayColor = Color.green;

    void Start()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        if (rayInteractor == null)
            rayInteractor = GetComponent<RayInteractor>();

        // Configurar el Line Renderer
        if (lineRenderer != null)
        {
            lineRenderer.startWidth = rayWidth;
            lineRenderer.endWidth = rayWidth;
            lineRenderer.material.color = rayColor;
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true; // Cambiar a true para este caso
        }
    }

    void Update()
    {
        if (rayInteractor != null && lineRenderer != null)
        {
            // Obtener el origen del rayo
            Vector3 startPoint = rayInteractor.Origin;

            // Calcular el punto final
            Vector3 direction = rayInteractor.Forward;
            float maxDistance = rayInteractor.MaxRayLength;
            Vector3 endPoint = startPoint + (direction * maxDistance);

            // Actualizar las posiciones del Line Renderer
            lineRenderer.SetPosition(0, startPoint);
            lineRenderer.SetPosition(1, endPoint);

            // Mostrar el rayo solo cuando el interactor esté activo
            lineRenderer.enabled = true;

        }
    }
}