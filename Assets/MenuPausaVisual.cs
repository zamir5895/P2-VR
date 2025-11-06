using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPausaVisual : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private GameObject canvasMenuPausa;
    [SerializeField] private Button botonVolverMenu;
    [SerializeField] private Button botonContinuar;

    [Header("Configuración")]
    [SerializeField] private string nombreEscenaMenu = "MenuPrincipal";

    private bool menuActivo = false;

    void Start()
    {
        // Ocultar menú al inicio
        if (canvasMenuPausa != null)
            canvasMenuPausa.SetActive(false);

        // Configurar botones
        if (botonVolverMenu != null)
            botonVolverMenu.onClick.AddListener(VolverAlMenu);

        if (botonContinuar != null)
            botonContinuar.onClick.AddListener(CerrarMenu);
    }

    void Update()
    {
        // Presionar botón Start/Menu del visor para abrir/cerrar
        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            if (menuActivo)
                CerrarMenu();
            else
                AbrirMenu();
        }
    }

    private void AbrirMenu()
    {
        menuActivo = true;
        if (canvasMenuPausa != null)
            canvasMenuPausa.SetActive(true);

        // Pausar el tiempo del juego si deseas
        // Time.timeScale = 0;
    }

    private void CerrarMenu()
    {
        menuActivo = false;
        if (canvasMenuPausa != null)
            canvasMenuPausa.SetActive(false);

        // Reanudar el tiempo si lo pausaste
        // Time.timeScale = 1;
    }

    public void VolverAlMenu()
    {
        // Restaurar tiempo antes de cambiar de escena
        Time.timeScale = 1;

        // Llamar al cargador de escenas
        CargadorEscenas.CargarEscena(nombreEscenaMenu);
    }
}
