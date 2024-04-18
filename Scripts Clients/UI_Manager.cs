using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;

    public GameObject startMenu;
    public InputField username;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Instance already exists");
            Destroy(instance);
            return;
        }
        instance = this;
    }

    public void ConnectToServer()
    {
        //startMenu.SetActive(false); 
        //username.interactable = false;
        
        Client.instance.ConnectToServer();
    }    
}
