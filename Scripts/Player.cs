using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Object that holds information about the user/player

[CreateAssetMenu(menuName = "Custom Assets/Player")]
public class Player : ScriptableObject
{
    public string displayName = "";
    public double userID;
    public bool loggedIn;
    public int online;
    public Animal activePet;
    public List<Animal> pets = new List<Animal>();
    public List<Player> friends = new List<Player>();
    public List<Player> friendsPendingOut = new List<Player>(); //Friends the player has added, but they have not accepted yet
    public List<Player> friendsPendingIn = new List<Player>(); //Friends who added the player, but the player has not accepted yet
    public List<Player> blockedOut = new List<Player>();
    public List<Player> blockedIn = new List<Player>();
    //public List<Item> items = new List<Item>();
    public List<Furniture> furniture = new List<Furniture>();
    public List<Room> rooms = new List<Room>();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
