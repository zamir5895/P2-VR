using UnityEngine;
using UnityEngine.UI;

public class PuntoInteraccion : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private string nombrePunto = "Punto RCP";
    [SerializeField] private string descripcion = "Presiona para interactuar";
    [SerializeField] private TipoPuntoRCP tipo;

    [Header("Efectos Visuales")]
    [SerializeField] private GameObject visualSphere;
    [SerializeField] private float velocidadPulso = 2f;
    [SerializeField] private float escalaMinima = 0.9f;
    [SerializeField] private float escalaMaxima = 1.2f;

    [Header("UI")]
    [SerializeField] private Canvas canvasInfo;
    [SerializeField] private Text textoNombre;

    private Vector3 escalaOriginal;
    private bool visible = false;
    private float tiempo = 0f;

    public enum TipoPuntoRCP
    {
        Pecho,
        Boca,
        Pulso,
        Muneca
    }

    void Start()
    {
        if (visualSphere != null)
        {
            escalaOriginal = visualSphere.transform.localScale;
            visualSphere.SetActive(false);
        }

        if (canvasInfo != null)
            canvasInfo.gameObject.SetActive(false);

        if (textoNombre != null)
            textoNombre.text = nombrePunto;
    }

    void Update()
    {
        if (visible && visualSphere != null)
        {
            // Efecto de pulso
            tiempo += Time.deltaTime * velocidadPulso;
            float escala = Mathf.Lerp(escalaMinima, escalaMaxima, (Mathf.Sin(tiempo) + 1f) / 2f);
            visualSphere.transform.localScale = escalaOriginal * escala;

            // Rotar suavemente
            visualSphere.transform.Rotate(Vector3.up, 50f * Time.deltaTime);
        }

        // Hacer que el canvas mire siempre a la cámara
        if (canvasInfo != null && canvasInfo.gameObject.activeSelf)
        {
            Transform camara = Camera.main.transform;
            canvasInfo.transform.LookAt(camara);
            canvasInfo.transform.Rotate(0, 180, 0);
        }
    }

    public void Mostrar()
    {
        visible = true;
        if (visualSphere != null)
            visualSphere.SetActive(true);
        if (canvasInfo != null)
            canvasInfo.gameObject.SetActive(true);
    }

    public void Ocultar()
    {
        visible = false;
        if (visualSphere != null)
            visualSphere.SetActive(false);
        if (canvasInfo != null)
            canvasInfo.gameObject.SetActive(false);
    }

    // Cuando el usuario interactúa (desde otro script)
    public void Interactuar()
    {
        Debug.Log($"Interactuando con: {nombrePunto}");

        // Aquí puedes añadir lógica específica según el tipo
        switch (tipo)
        {
            case TipoPuntoRCP.Pecho:
                IniciarCompresiones();
                break;
            case TipoPuntoRCP.Boca:
                IniciarRespiracion();
                break;
            case TipoPuntoRCP.Pulso:
                VerificarPulso();
                break;
            case TipoPuntoRCP.Muneca:
                VerificarPulsoMuneca();
                break;
        }
    }

    void IniciarCompresiones()
    {
        Debug.Log("Iniciando compresiones torácicas");
        // Tu lógica de RCP aquí
    }

    void IniciarRespiracion()
    {
        Debug.Log("Iniciando respiración boca a boca");
    }

    void VerificarPulso()
    {
        Debug.Log("Verificando pulso carotídeo");
    }

    void VerificarPulsoMuneca()
    {
        Debug.Log("Verificando pulso radial");
    }
}
