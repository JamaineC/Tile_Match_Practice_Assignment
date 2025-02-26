using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    public int level;
    public int gridLength = 6;

    // Start is called before the first frame update
    void Start()
    {
    
                // Singleton logic
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps the object alive across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
