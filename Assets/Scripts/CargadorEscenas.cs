using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class CargadorEscenas : MonoBehaviour
{
    [Header("UI Referencias")]
    [SerializeField] private Slider barraProgreso;
    [SerializeField] private TextMeshProUGUI textoPorcentaje;
    [SerializeField] private TextMeshProUGUI textoCargando;
    [SerializeField] private CanvasGroup canvasGroup; // AÑADE ESTO

    [Header("Configuración")]
    [SerializeField] private float tiempoMinimoLoading = 2f;

    private static string escenaDestino;

    void Awake() // Cambia Start por Awake
    {
        // Asegurar que el Canvas esté visible inmediatamente
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
    }

    void Start()
    {
        StartCoroutine(CargarEscenaAsync());
    }

    IEnumerator CargarEscenaAsync()
    {
        // Esperar un frame
        yield return null;

        // Verificar que hay una escena destino
        if (string.IsNullOrEmpty(escenaDestino))
        {
            Debug.LogError("No hay escena destino configurada");
            yield break;
        }

        // Tiempo de inicio
        float tiempoInicio = Time.time;

        // Iniciar carga asíncrona
        AsyncOperation operacion = SceneManager.LoadSceneAsync(escenaDestino);
        operacion.allowSceneActivation = false;

        // Mientras carga
        while (!operacion.isDone)
        {
            float progreso = Mathf.Clamp01(operacion.progress / 0.9f);

            if (barraProgreso != null)
                barraProgreso.value = progreso;

            if (textoPorcentaje != null)
                textoPorcentaje.text = (progreso * 100f).ToString("F0") + "%";

            if (textoCargando != null)
            {
                int puntos = ((int)(Time.time * 2)) % 4;
                textoCargando.text = "Cargando" + new string('.', puntos);
            }

            if (operacion.progress >= 0.9f)
            {
                float tiempoTranscurrido = Time.time - tiempoInicio;
                if (tiempoTranscurrido >= tiempoMinimoLoading)
                {
                    if (barraProgreso != null)
                        barraProgreso.value = 1f;
                    if (textoPorcentaje != null)
                        textoPorcentaje.text = "100%";

                    yield return new WaitForSeconds(0.5f);
                    operacion.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }

    public static void CargarEscena(string nombreEscena)
    {
        escenaDestino = nombreEscena;
        SceneManager.LoadScene("LoadingScene");
    }
}