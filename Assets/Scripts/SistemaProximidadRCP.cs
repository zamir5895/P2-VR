using UnityEngine;
using System.Collections.Generic;

public class SistemaProximidadRCP : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Transform jugador; // El Camera Rig
    [SerializeField] private float distanciaActivacion = 2f;
    [SerializeField] private List<PuntoInteraccion> puntosInteraccion = new List<PuntoInteraccion>();

    [Header("UI Información")]
    [SerializeField] private GameObject panelInstrucciones;

    private bool cercaDelManiqui = false;

    void Start()
    {
        // Si no se asigna jugador, buscar la cámara principal
        if (jugador == null)
        {
            Camera cam = Camera.main;
            if (cam != null)
                jugador = cam.transform;
        }

        // Ocultar todos los puntos al inicio
        foreach (var punto in puntosInteraccion)
        {
            if (punto != null)
                punto.Ocultar();
        }

        if (panelInstrucciones != null)
            panelInstrucciones.SetActive(false);
    }

    void Update()
    {
        if (jugador == null) return;

        // Calcular distancia al maniquí
        float distancia = Vector3.Distance(jugador.position, transform.position);

        // Verificar si está cerca
        if (distancia <= distanciaActivacion && !cercaDelManiqui)
        {
            // Entró en rango
            ActivarPuntos();
            cercaDelManiqui = true;
        }
        else if (distancia > distanciaActivacion && cercaDelManiqui)
        {
            // Salió del rango
            DesactivarPuntos();
            cercaDelManiqui = false;
        }
    }

    void ActivarPuntos()
    {
        foreach (var punto in puntosInteraccion)
        {
            if (punto != null)
                punto.Mostrar();
        }

        if (panelInstrucciones != null)
            panelInstrucciones.SetActive(true);
    }

    void DesactivarPuntos()
    {
        foreach (var punto in puntosInteraccion)
        {
            if (punto != null)
                punto.Ocultar();
        }

        if (panelInstrucciones != null)
            panelInstrucciones.SetActive(false);
    }

    // Dibujar el rango en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, distanciaActivacion);
    }
}