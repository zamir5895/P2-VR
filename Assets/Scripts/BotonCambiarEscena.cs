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
        // Usar el cargador en lugar de LoadScene directo
        CargadorEscenas.CargarEscena(nombreEscena);
    }
}