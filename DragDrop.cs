using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour,IPointerDownHandler,IBeginDragHandler,IEndDragHandler,IDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    [SerializeField] private GameObject mainSeting;
    
   
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        mainSeting.GetComponent<MainSeting>().startPoint = rectTransform.position;
        
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        
        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
        //Debug.Log("Drag"+"x:" +rectTransform.anchoredPosition.x+"y:"+rectTransform.anchoredPosition.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        if ((eventData.position.x > Screen.width) || (eventData.position.y > Screen.height)|| (eventData.position.x<0)|| (eventData.position.y < 0))
        {
            rectTransform.position = mainSeting.GetComponent<MainSeting>().startPoint;
            eventData.pointerDrag.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("down");
    }

    
}
