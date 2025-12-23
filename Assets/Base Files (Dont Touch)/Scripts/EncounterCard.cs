using UnityEngine;
using UnityEngine.EventSystems;

public class EncounterCard : MonoBehaviour, IPointerClickHandler
{
    public bool IsCardSelected { get; private set; } = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        IsCardSelected = true;
    }

    public void ResetSelection()
    {
        IsCardSelected = false;
    }
}
