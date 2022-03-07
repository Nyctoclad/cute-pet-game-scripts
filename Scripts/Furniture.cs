using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Scriptable object holding furniture information
[CreateAssetMenu(menuName = "Custom Assets/Furniture")]
public class Furniture : ScriptableObject
{
    public double userFurnitureID; //ID of furniture in player's furniture database
    public float furnitureID; //ID of furniture type in total database
    public string furnitureName;
    public float furnitureColorID;
    public string colorName;
    public int furnitureType, roomID; //Furniture type: wall or floor or ceiling furniture; Room ID is what room the furniture is in
    public Material material;
    public GameObject prefab;
    public GameObject icon;
    public Vector3 location, rotation;
    public bool placed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
