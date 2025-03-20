using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Box : MonoBehaviour, IDropHandler
{
    
    private float groundY;
    [SerializeField] private float majore;
    [SerializeField] private GameObject mainSetings;
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            if (this.name == "BoxS")
            {
                
                groundY = this.transform.position.y + this.GetComponent<RectTransform>().rect.position.y;
                eventData.pointerDrag.GetComponent<RectTransform>().localScale = new Vector3(majore, majore, majore);
                pointTarrget(eventData);
                
            }
            
            else if (this.name == "BoxN")
            {
                Vector2 startPoint = mainSetings.GetComponent<MainSeting>().startPoint;
                eventData.pointerDrag.GetComponent<RectTransform>().localScale = new Vector3(majore, majore, majore);
                eventData.pointerDrag.GetComponent<RectTransform>().position=startPoint;
            }
            
            
        }
    }
    void pointTarrget(PointerEventData eventData)
    {
        eventData.pointerDrag.GetComponent<RectTransform>().position = new Vector2(
                   eventData.pointerDrag.GetComponent<RectTransform>().position.x, groundY);
    }
    
}
