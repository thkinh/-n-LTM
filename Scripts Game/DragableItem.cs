using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.ComponentModel.Design.Serialization;
using Unity.VisualScripting;
using Assets.Scripts.Game;

public class DragableItem : MonoBehaviour
{
    private bool dragging;
    private Vector2 offset, original_pos;

    private void Awake()
    {
        original_pos = transform.position;
    }

    void Update()
    {
        if (!dragging) return;

        var mousepos = GetMousePos();


        transform.position = mousepos - offset;
    }

    void OnMouseDown()
    {
        dragging = true;
        Debug.Log("Dragging");

        offset = GetMousePos() - (Vector2)transform.position;
    }

    private void OnMouseUp()
    {
        if(!GetComponent<Renderer>().isVisible)
        {
            transform.position = original_pos;
        }

        if (Vector2.Distance(transform.position, EntityManager.instance.trashcan.transform.position) < 3)
        {
            Packet packet = new Packet(Food.Getdata(this.name));
            packet.WriteLength();
            packet.Write(Client.instance.ID);
            Client.instance.tcp.SendData(packet);
        }

        dragging = false;
        Debug.Log("Dropped!");
    }

    Vector2 GetMousePos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

}
