using UnityEngine;
using UnityEngine.UI;

public class BotonCambiarEscena : MonoBehaviour
{
    [SerializeField] private string nombreEscena;

    void Start()
    {
        Button boton = GetComponent<Button>();
        if (boton != null)
        {
            boton.onClick.AddListener(CambiarEscena);
        }
    }

    void CambiarEscena()
    {
        // IMPORTANTE: Usar CargadorEscenas en lugar de SceneManager
        CargadorEscenas.CargarEscena(nombreEscena);
    }
}