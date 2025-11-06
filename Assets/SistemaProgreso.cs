using UnityEngine;

public class SistemaProgreso : MonoBehaviour
{
    public static void GuardarProgreso(string clave, int valor)
    {
        PlayerPrefs.SetInt(clave, valor);
        PlayerPrefs.Save();
    }

    public static int CargarProgreso(string clave, int valorPorDefecto = 0)
    {
        return PlayerPrefs.GetInt(clave, valorPorDefecto);
    }

    public static void GuardarEscenaCompletada(string nombreEscena)
    {
        PlayerPrefs.SetInt("Completado_" + nombreEscena, 1);
        PlayerPrefs.Save();
    }

    public static bool EscenaCompletada(string nombreEscena)
    {
        return PlayerPrefs.GetInt("Completado_" + nombreEscena, 0) == 1;
    }
}