using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A room's prefab based on the size of the room
[CreateAssetMenu(menuName = "Custom Assets/Room Size")]
public class RoomSize : ScriptableObject
{
    public int size;
    public GameObject prefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
