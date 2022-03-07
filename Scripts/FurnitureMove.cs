using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Script controls furniture movement for furniture placement
public class FurnitureMove : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;
    public Bounds bounds;
    public GameObject placementLight;
    public MeshRenderer placementRenderer;
    public Furniture furniture;
    MainLandController mainLandController;
    public Vector3 originalRotation = new Vector3(), originalLocation = new Vector3();
    Room room = new Room();


    public bool placeable = true, active = false;
    
    // Start is called before the first frame update
    void Start()
    {
        mainLandController = GameObject.FindGameObjectWithTag("land_information").GetComponent<MainLandController>();
        placementLight = this.gameObject.transform.Find("PlacementLight").gameObject;
        placementRenderer = placementLight.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

 
 
    void OnMouseDown()
    {
        //Destroy empty gameobjects that got created
        foreach(GameObject obj in GameObject.FindObjectsOfType<GameObject>()){
            if(obj.name.Equals("New Game Object")){
                Destroy(obj);
            }
        }

        //Find the current active room for moving furniture
        foreach(GameObject foundRoom in GameObject.FindGameObjectsWithTag("room_information")){
            if(foundRoom.GetComponent<Room>().active){
                room = foundRoom.GetComponent<Room>();
                bounds = room.prefab.transform.Find("Floor").GetComponent<MeshRenderer>().bounds;
                break;
            }
        }

        if(mainLandController.furnitureScreen.activeSelf && !active){
            originalLocation = this.gameObject.transform.position;
            originalRotation = this.gameObject.transform.eulerAngles;
            bool found = false;

            //Find the active furniture to be moved
            foreach(GameObject obj in GameObject.FindObjectsOfType<GameObject>()){
                if(room.furnitureTags.Contains(obj.tag)){
                    if(obj.GetComponent<FurnitureMove>().active){
                        found = true;
                        break;
                    }
                }
            }

            //Move this furniture
            if(!found){
                active = true;
                room.MoveFurniture(furniture, this.gameObject);
            }
        }
        
        if(mainLandController.furnitureScreen.activeSelf && active){
            screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        }
    }
 
    void OnMouseDrag()
    {
        if(mainLandController.furnitureScreen.activeSelf && active){

            foreach(GameObject obj in GameObject.FindObjectsOfType<GameObject>()){
                if(obj.name.Equals("New Game Object")){
                    Destroy(obj);
                }
            }

            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;

            //Make sure the furniture being moved stays within the bounds of whatever surface the furniture is being placed on
            if(bounds != null){

                if(bounds.Contains(new Vector3(curPosition.x, bounds.center.y, curPosition.z))){

                    transform.position = new Vector3(curPosition.x, transform.position.y, curPosition.z);
                    bool found = false;
                    Bounds inBounds = this.gameObject.GetComponent<BoxCollider>().bounds;

                    foreach(GameObject item in GameObject.FindObjectsOfType<GameObject>()){

                        if(room.furnitureTags.Contains(item.tag) || room.roomTags.Contains(item.tag)){

                            if((this.gameObject.tag.Contains("floor") && !item.tag.Equals("floor")) || (this.gameObject.tag.Contains("wall") && !item.tag.Equals("wall"))){
                                
                                Bounds itemBounds = item.GetComponent<BoxCollider>().bounds;

                                if((inBounds.Contains(item.transform.localPosition) || inBounds.Intersects(itemBounds)) && (this.gameObject != item)){
                                    
                                    placeable = false;
                                    found = true;
                                    break;
                                }
                            }
                        }
                    }

                    
                    //Change the placement light underneath the furniture to indicate to the user whether or not the object can be placed
                    if(!found){
                        Color color = new Color();
                        placeable = true;
                        color.a = 0f;
                        placementRenderer.material.color = color;

                        mainLandController.furnitureSet.GetComponent<Button>().enabled = true;
                        
                    }
                    else if(found)
                        placementRenderer.material.color = Color.red;
                    
                }
            }
            else{
                Debug.Log("No bounds on this object!");
            }
        }

    }

}
