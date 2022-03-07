using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

//Script controls furniture placement
public class FurniturePlacement : MonoBehaviour
{
    public Furniture furniture;
    Button button;

    // Start is called before the first frame update
    void Start()
    {
        button = this.gameObject.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //Press the button to place the furniture in the scene
    public void FurniturePlace(){

        Room room = new Room();

        foreach(GameObject foundRoom in GameObject.FindGameObjectsWithTag("room_information")){
            
            if(foundRoom.GetComponent<Room>().active){
                
                room = foundRoom.GetComponent<Room>();
                break;
            }
        }

        room.InstantiateFurniture(furniture, button);
    }

}