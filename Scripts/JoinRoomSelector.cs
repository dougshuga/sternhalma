using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoinRoomSelector : MonoBehaviour, ISelectHandler
{

    private Dropdown joinRoomDropdown;
	[SerializeField] Button joinRoomSubmit;

    private void Start()
    {
        //joinRoomSubmit.interactable = false;
        joinRoomDropdown = GetComponent<Dropdown>();
    }

    public void OnSelect(BaseEventData eventData)
	{
        //joinRoomSubmit.interactable = true;
	}
}
