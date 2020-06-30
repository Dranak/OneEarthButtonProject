using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnfoldDragger : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField] UnfoldButton unfoldManager;

    public void OnBeginDrag(PointerEventData eventData)
    {
        SelectUnfolder(unfoldManager.selectedUnfolder);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        SelectUnfolder(unfoldManager.selectedUnfolder);
    }

    void SelectUnfolder(Unfolder unfolder)
    {
        if (unfolder)
            UiManager.Instance.eventSystem.SetSelectedGameObject(unfolder.gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (unfoldManager.selectedUnfolder)
        {
            unfoldManager.Folding(unfoldManager.selectedUnfolder);
        }
    }
}
