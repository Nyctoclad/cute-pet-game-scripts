using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Object that holds information about animal color
[CreateAssetMenu(menuName = "Custom Assets/Animal Color")]
public class AnimalColor : ScriptableObject
{
    public string colorName;
    public int color;
    public int animalID;
    public Material animalMaterial;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
