using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Object that holds information about animal face/head
[CreateAssetMenu(menuName = "Custom Assets/Animal Face")]
public class AnimalFace : ScriptableObject
{
    public List<int> eligibleAnimals;
    public Material faceMaterial;
    public int faceID;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
