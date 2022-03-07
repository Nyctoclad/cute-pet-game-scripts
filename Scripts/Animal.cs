using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Scriptable object that holds information about the animal/pet
[CreateAssetMenu(menuName = "Custom Assets/Animal")]
public class Animal : ScriptableObject
{
    public int animalType; //ID of the animal in the database
    public string animalName; 
    public string petName;
    public int petID; //ID of the pet in the database
    public int glow;
    public int special;
    public GameObject prefab;
    public AnimalFace face; 
    public AnimalColor color;
    public Clothing clothing; 
    public List<Accessory> accessories = new List<Accessory>();
    public List<PetFriendship> petFriendships = new List<PetFriendship>(); //Friendships between pets
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
