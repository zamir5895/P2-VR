using UnityEngine;

public class MenuPausa : MonoBehaviour
{
    [SerializeField] private string nombreEscenaMenu = "MenuPrincipal";

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            VolverAlMenu();
        }
    }

    public void VolverAlMenu()
    {
        // Usar el cargador
        CargadorEscenas.CargarEscena(nombreEscenaMenu);
    }
}