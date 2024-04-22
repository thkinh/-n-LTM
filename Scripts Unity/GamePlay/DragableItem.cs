using Assets.Scripts;
using Assets.Scripts.GamePlay;
using Unity.VisualScripting;
using UnityEngine;

public class DragableItem : MonoBehaviour
{
    private bool dragging;
    private Vector2 offset, original_pos;
    public Food food;
    private void Awake()
    {
        //food = new Food(0);
        transform.position = new Vector2(2,2);
        dragging = false;
        original_pos = transform.position;
        offset = new Vector2(0,0);
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
        dragging = false;
        Debug.Log("Dropped!");
        if (!GetComponent<Renderer>().isVisible)
        {
            transform.position = original_pos;
        }

        if (Vector2.Distance(transform.position, EntityManager.instance.trashcan.transform.position) < 1f )
        {
            Debug.Log("Destroyed this object");
            Destroy(this.GameObject());
        }

        if (Vector2.Distance(transform.position, EntityManager.instance.SendtoServer.transform.position) < 1f)
        {
            ClientManager.client.SendPacket(this.food, true);

            Destroy(this.gameObject);
        }
    }



    Vector2 GetMousePos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
