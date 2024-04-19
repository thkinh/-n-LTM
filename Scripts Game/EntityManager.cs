using Assets.Scripts.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager instance;
    [SerializeField] private int playerID = 0;

    private float timer = 0.0f, previous_time = 0.0f;
    public GameObject trashcan;
    public GameObject m_Food;

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

    private void Start()
    {
        Instantiate(m_Food, transform);
        playerID = Client.instance.ID;
        timer = 0;

    }

    private void Update()
    {
        timer += Time.fixedDeltaTime;
        if (timer - previous_time > 60)
        {
            Debug.Log(timer);
            SpawnRandomFood();
            previous_time = timer;
        }
    }

    public static void SpawnRandomFood()
    {
        int seed = Random.Range(1,3);
        Food food = new Food(seed);

        GameObject newfood = Instantiate(instance.m_Food, new Vector2(10,10), Quaternion.identity);
        newfood.name = food.name;
    }

    
        

}
