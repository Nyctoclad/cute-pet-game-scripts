using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Object that holds a list of current furniture
[CreateAssetMenu(menuName = "Custom Assets/Furniture List")]
public class FurnitureList : ScriptableObject
{
    public List<Furniture> furniture;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
