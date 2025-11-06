using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro; // Si usas TextMeshPro

public class CargadorEscenas : MonoBehaviour
{
    [Header("UI Referencias")]
    [SerializeField] private Slider barraProgreso;
    [SerializeField] private TextMeshProUGUI textoPorcentaje; // O usa "Text" si no es TMP
    [SerializeField] private TextMeshProUGUI textoCargando;

    [Header("Configuración")]
    [SerializeField] private float tiempoMinimoLoading = 2f; // Para que se vea el loading

    private static string escenaDestino;

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
        operacion.allowSceneActivation = false; // No activar hasta que esté listo

        // Mientras carga
        while (!operacion.isDone)
        {
            // Calcular progreso (0.0 a 0.9 = cargando, 0.9 = listo)
            float progreso = Mathf.Clamp01(operacion.progress / 0.9f);

            // Actualizar UI
            if (barraProgreso != null)
                barraProgreso.value = progreso;

            if (textoPorcentaje != null)
                textoPorcentaje.text = (progreso * 100f).ToString("F0") + "%";

            // Animación del texto "Cargando..."
            if (textoCargando != null)
            {
                int puntos = ((int)(Time.time * 2)) % 4;
                textoCargando.text = "Cargando" + new string('.', puntos);
            }

            // Cuando llegue al 90%, esperar tiempo mínimo
            if (operacion.progress >= 0.9f)
            {
                float tiempoTranscurrido = Time.time - tiempoInicio;

                if (tiempoTranscurrido >= tiempoMinimoLoading)
                {
                    // Completar barra
                    if (barraProgreso != null)
                        barraProgreso.value = 1f;
                    if (textoPorcentaje != null)
                        textoPorcentaje.text = "100%";

                    yield return new WaitForSeconds(0.5f); // Pausa pequeña
                    operacion.allowSceneActivation = true; // Activar escena
                }
            }

            yield return null;
        }
    }

    // Método estático para llamar desde otros scripts
    public static void CargarEscena(string nombreEscena)
    {
        escenaDestino = nombreEscena;
        SceneManager.LoadScene("LoadingScene");
    }
}