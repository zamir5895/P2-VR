using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FeedbackBoton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button boton;
    private AudioSource audioSource;

    [SerializeField] private AudioClip sonidoHover;
    [SerializeField] private AudioClip sonidoClick;

    void Start()
    {
        boton = GetComponent<Button>();
        audioSource = GetComponent<AudioSource>();

        if (boton != null)
            boton.onClick.AddListener(ReproducirSonidoClick);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (audioSource && sonidoHover)
            audioSource.PlayOneShot(sonidoHover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Opcional: otro efecto
    }

    void ReproducirSonidoClick()
    {
        if (audioSource && sonidoClick)
            audioSource.PlayOneShot(sonidoClick);
    }
}