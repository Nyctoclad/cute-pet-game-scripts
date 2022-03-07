using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Object that holds animals in a database. Other scripts can grab the available animals and colors from here.
[CreateAssetMenu(menuName = "Custom Assets/Animal List")]
public class AnimalList : ScriptableObject
{
    public List<Animal> animals;
    public List<AnimalColor> colors;
    public List<AnimalFace> faces;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
