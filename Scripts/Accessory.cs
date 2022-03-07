using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Scriptable object that holds information about pet accessories

[CreateAssetMenu(menuName = "Custom Assets/Accessory")]
public class Accessory : ScriptableObject
{
    
    public string accessoryName;
    public int accessoryID;
    public float userAccessoryID;
    public Sprite accessoryIcon;
    public int accessoryLocation; //accessoryLocation refers to position the accessory is on: back, neck, head, left arm, right arm, etc.
    public Material accessoryMaterial;
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
