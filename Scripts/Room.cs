using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    public int roomSize, roomID, floorMaterialID, wallMaterialID;
    public bool active, defaultRoom;
    public Mat floor, wall;
    public GameObject prefab;
    public Vector3 location;
    public Quaternion rotation;
    public GameObject furnitureMover;
    MainLandController mainLandController;
    Player player;
    Database database;
    Vector3 bounds1, bounds2, bounds3, bounds4, screenTouch, offset;
    GridSnap grid;

    Vector3 camOriginalPosition, camOriginalRotation;
    public List<string> furnitureTags = new List<string>(), roomTags = new List<string>();

    public bool moving = false;

    // Start is called before the first frame update
    void Start()
    {
        mainLandController = GameObject.FindGameObjectWithTag("land_information").GetComponent<MainLandController>();
        database = GameObject.FindGameObjectWithTag("database").GetComponent<Database>();
        player = database.player;
        grid = GameObject.FindGameObjectWithTag("scriptholder").GetComponent<GridSnap>();
        camOriginalPosition = Camera.main.transform.position;
        camOriginalRotation = Camera.main.transform.eulerAngles;

        foreach(string tag in database.furnitureTags.tags){
            furnitureTags.Add(tag);
        }
        foreach(string tag in database.roomTags.tags){
            roomTags.Add(tag);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Method to instantiate the furniture in the room
    public void InstantiateFurniture(Furniture furniture, Button button){
        GameObject instantiatedFurniture = new GameObject();
        List<GameObject> pets = new List<GameObject>();
        Room instantiatedRoom = new Room();
        foreach(GameObject pet in GameObject.FindGameObjectsWithTag("pet")){
            pets.Add(pet);
            pet.SetActive(false);
        }
        foreach(GameObject g in GameObject.FindObjectsOfType<GameObject>()){
        	if(furnitureTags.Contains(g.tag) || roomTags.Contains(g.tag)){
                if(!g.GetComponentInChildren<MeshRenderer>().enabled){
                    Destroy(g);
                }
            }
        }   
        instantiatedFurniture = GameObject.Instantiate(furniture.prefab);
        if(active){
            instantiatedFurniture.transform.position = new Vector3(this.gameObject.transform.position.x, 4, this.gameObject.transform.position.z);
            instantiatedRoom = this;
        }

        FurnitureMove fm = instantiatedFurniture.AddComponent<FurnitureMove>();
        fm.bounds = instantiatedRoom.prefab.transform.Find("Floor").gameObject.GetComponent<MeshRenderer>().bounds;
        fm.active = true;
        fm.furniture = furniture;

        mainLandController.SeeScreen();
        
        MoveObject(furniture, instantiatedRoom, instantiatedFurniture);
        
        mainLandController.furnitureCancel.SetActive(true);
        mainLandController.furnitureCancel.GetComponent<Button>().onClick.RemoveAllListeners();
        mainLandController.furnitureCancel.GetComponent<Button>().onClick.AddListener(() => CancelAfterInstantiate(instantiatedFurniture, pets, button));
        mainLandController.furnitureRotate.SetActive(true);
        mainLandController.furnitureRotate.GetComponent<Button>().onClick.RemoveAllListeners();
        mainLandController.furnitureRotate.GetComponent<Button>().onClick.AddListener(() => Rotate(instantiatedFurniture, instantiatedRoom, furniture));
        mainLandController.furnitureSet.SetActive(true);
        mainLandController.furnitureSet.GetComponent<Button>().onClick.RemoveAllListeners();
        mainLandController.furnitureSet.GetComponent<Button>().onClick.AddListener(() => PlaceFunitureAfterInstantiate(instantiatedFurniture, button, pets, furniture));

        mainLandController.crActive = true;
        instantiatedRoom.furnitureMover.transform.position = instantiatedFurniture.transform.position;
        button.enabled = false;

    }

    //For the cancel placing furniture button and returning the placed object back to where it was
    public void CancelAfterInstantiate(GameObject rf, List<GameObject> pets, Button button){

        Camera.main.transform.position = camOriginalPosition;
        Camera.main.transform.eulerAngles = camOriginalRotation;

    	foreach(Transform child in rf.transform){
    		if(child.gameObject.GetComponent<MeshRenderer>() != null){
    			child.gameObject.GetComponent<MeshRenderer>().enabled = false;
    		}
    	}


    	mainLandController.HideScreen();

    	mainLandController.furnitureCancel.SetActive(false);
    	mainLandController.furnitureRotate.SetActive(false);
        mainLandController.furnitureSet.SetActive(false);
        mainLandController.crActive = false;

        foreach(GameObject pet in pets){
            pet.SetActive(true);
        }

        pets.Clear();


    	button.enabled = true;
        rf.GetComponent<FurnitureMove>().active = false;
    }

    //For placing the furniture and submitting that new furniture location to the database
    public void PlaceFunitureAfterInstantiate(GameObject rf, Button button, List<GameObject> pets, Furniture furniture){

        FurnitureMove fm = rf.GetComponent<FurnitureMove>();
        bool found = false;
        Bounds bounds = rf.GetComponent<BoxCollider>().bounds;
        foreach(GameObject item in GameObject.FindObjectsOfType<GameObject>()){
            if(furnitureTags.Contains(item.tag) || roomTags.Contains(item.tag)){
                if((this.gameObject.tag.Contains("floor") && !item.tag.Equals("floor")) || (this.gameObject.tag.Contains("wall") && !item.tag.Equals("wall"))){
                    Bounds itemBounds = item.GetComponent<BoxCollider>().bounds;
                    if((bounds.Contains(item.transform.localPosition) || bounds.Intersects(itemBounds)) && (this.gameObject != item)){
                        found = true;
                        break;
                    }
                }
            }
        }

        if(!found){
            foreach(Furniture f in player.furniture){
                if(f.userFurnitureID == furniture.userFurnitureID){

                    f.placed = true;
                    f.location = rf.transform.position;
                    f.rotation = rf.transform.eulerAngles;
                    f.roomID = roomID;

                    Camera.main.transform.position = camOriginalPosition;
                    Camera.main.transform.eulerAngles = camOriginalRotation;


                    mainLandController.HideScreen();
                    mainLandController.furnitureCancel.SetActive(false);
                    mainLandController.furnitureRotate.SetActive(false);
                    mainLandController.furnitureSet.SetActive(false);
                    mainLandController.crActive = false;

                    foreach(GameObject pet in pets){
                        pet.SetActive(true);
                    }

                    pets.Clear();
                    button.enabled = true;
                    
                    foreach(GameObject obj in GameObject.FindObjectsOfType<GameObject>()){
                        if(obj.name.Equals("New Game Object")){
                            Destroy(obj);
                        }
                    }
                    
                    string furnitureLocation = "" + f.location.x + ":" + f.location.y + ":" + f.location.z;
                    string furnitureRotation = "" + f.rotation.x + ":" + f.rotation.y + ":" + f.rotation.z;
                    database.SubmitFurniture(f.userFurnitureID, f.furnitureID.ToString(), f.furnitureColorID.ToString(), furnitureLocation, furnitureRotation, f.roomID);
                    
                    rf.GetComponent<FurnitureMove>().active = false;
                    
                    mainLandController.SetFurnitureList();
                }
            }
            fm.placementLight.SetActive(false);
        }
        else if(found){
            database.messages.errorText.text = "Cannot place; furniture blocked.";
            database.messages.errorTitle.text = "Error";
            database.messages.ErrorMessage.SetActive(true);
            fm.placementLight.SetActive(true);
            fm.placementRenderer.material.color = Color.red;
        }
    }

    //For moving the furniture around the room
    public void MoveFurniture(Furniture furniture, GameObject instantiatedFurniture){
        List<GameObject> pets = new List<GameObject>();
        foreach(GameObject pet in GameObject.FindGameObjectsWithTag("pet")){
            pets.Add(pet);
            pet.SetActive(false);
        }

        foreach(GameObject g in GameObject.FindObjectsOfType<GameObject>()){
        	foreach(MeshRenderer mr in g.GetComponentsInChildren<MeshRenderer>()){
                if(mr != null){
                    if(!mr.enabled && furnitureTags.Contains(g.tag)){
                        Destroy(g);
                    }
                }
            }
        }

        Room instantiatedRoom = new Room();


        //Get active room and get active room floor
        if(active){
            instantiatedRoom = this;
        }

        FurnitureMove fm = instantiatedFurniture.GetComponent<FurnitureMove>();
        fm.active = true;

        MoveObject(furniture, instantiatedRoom, instantiatedFurniture);
        mainLandController.furnitureCancel.SetActive(true);
        mainLandController.furnitureCancel.GetComponent<Button>().onClick.RemoveAllListeners();
        mainLandController.furnitureCancel.GetComponent<Button>().onClick.AddListener(() => Cancel(instantiatedFurniture, pets, furniture));
        mainLandController.furnitureRotate.SetActive(true);
        mainLandController.furnitureRotate.GetComponent<Button>().onClick.RemoveAllListeners();
        mainLandController.furnitureRotate.GetComponent<Button>().onClick.AddListener(() => Rotate(instantiatedFurniture, instantiatedRoom, furniture));
        mainLandController.furnitureSet.SetActive(true);
        mainLandController.furnitureSet.GetComponent<Button>().onClick.RemoveAllListeners();
        mainLandController.furnitureSet.GetComponent<Button>().onClick.AddListener(() => PlaceFuniture(instantiatedFurniture, pets, furniture));
        mainLandController.furnitureDelete.SetActive(true);
        mainLandController.furnitureDelete.GetComponent<Button>().onClick.RemoveAllListeners();
        mainLandController.furnitureDelete.GetComponent<Button>().onClick.AddListener(() => DeleteFurniture(instantiatedFurniture, pets, furniture));
    }

    //Cancel placing the furniture and remove it from the room
    public void Cancel(GameObject rf, List<GameObject> pets, Furniture furniture){
        rf.transform.position = rf.GetComponent<FurnitureMove>().originalLocation;
        rf.transform.eulerAngles = rf.GetComponent<FurnitureMove>().originalRotation;
        Camera.main.transform.position = camOriginalPosition;
        Camera.main.transform.eulerAngles = camOriginalRotation;

    	
    	
    	mainLandController.furnitureCancel.SetActive(false);
        mainLandController.furnitureRotate.SetActive(false);
        mainLandController.furnitureSet.SetActive(false);
        mainLandController.furnitureDelete.SetActive(false);

        foreach(GameObject pet in pets){
            pet.SetActive(true);
        }

        pets.Clear();
        rf.GetComponent<FurnitureMove>().active = false;
    }

    //For placing new furniture and submitting that furniture's location to the database
    public void PlaceFuniture(GameObject rf, List<GameObject> pets, Furniture furniture){
		FurnitureMove furnitureMover = rf.GetComponent<FurnitureMove>();
        bool found = false;
        Bounds bounds = rf.GetComponent<BoxCollider>().bounds;

        foreach(GameObject item in GameObject.FindObjectsOfType<GameObject>()){
            if(furnitureTags.Contains(item.tag) || roomTags.Contains(item.tag)){
                if((this.gameObject.tag.Contains("floor") && !item.tag.Equals("floor")) || (this.gameObject.tag.Contains("wall") && !item.tag.Equals("wall"))){
                    Bounds itemBounds = item.GetComponent<BoxCollider>().bounds;
                    if((bounds.Contains(item.transform.localPosition) || bounds.Intersects(itemBounds)) && (this.gameObject != item)){
                        found = true;
                        break;
                    }
                }
            }
        }

        if(!found){
            foreach(Furniture f in player.furniture){
                if(f.userFurnitureID == furniture.userFurnitureID){

                    f.placed = true;
                    f.location = rf.transform.position;
                    f.rotation = rf.transform.eulerAngles;

                    Camera.main.transform.position = camOriginalPosition;
                    Camera.main.transform.eulerAngles = camOriginalRotation;

                    mainLandController.furnitureCancel.SetActive(false);
                    mainLandController.furnitureRotate.SetActive(false);
                    mainLandController.furnitureSet.SetActive(false);
                    mainLandController.furnitureDelete.SetActive(false);
                    
                    foreach(GameObject pet in pets){
                        pet.SetActive(true);
                    }

                    pets.Clear();
                    
                    foreach(GameObject obj in GameObject.FindObjectsOfType<GameObject>()){
                        if(obj.name.Equals("New Game Object")){
                            Destroy(obj);
                        }
                    }
                    
                    string furnitureLocation = "" + rf.transform.position.x + ":" + rf.transform.position.y + ":" + rf.transform.position.z;
                    string furnitureRotation = "" + rf.transform.eulerAngles.x + ":" + rf.transform.eulerAngles.y + ":" + rf.transform.eulerAngles.z;
                    Debug.Log("Location set as " + furnitureLocation);
                    database.SubmitFurniture(f.userFurnitureID, f.furnitureID.ToString(), f.furnitureColorID.ToString(), furnitureLocation, furnitureRotation, f.roomID);
                    
                    rf.GetComponent<FurnitureMove>().active = false;
                    
                    mainLandController.SetFurnitureList();
                }
            }
            furnitureMover.placementLight.SetActive(false);
        }
        else if(found){
            database.messages.errorText.text = "Cannot place; furniture blocked.";
            database.messages.errorTitle.text = "Error";
            database.messages.ErrorMessage.SetActive(true);
            furnitureMover.placementLight.SetActive(true);
            furnitureMover.placementRenderer.material.color = Color.red;
        }
    }

    //For removing a placed piece of furniture from the room
    public void DeleteFurniture(GameObject rf, List<GameObject> pets, Furniture furniture){
        foreach(Transform child in rf.transform){
            if(child.gameObject.GetComponent<MeshRenderer>() != null){
                child.gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
        }

        rf.GetComponent<FurnitureMove>().active = false;
        foreach(Furniture f in player.furniture){
            if(f.userFurnitureID == furniture.userFurnitureID){
                f.placed = false;
                //f.location = "n";

            }
        }
        
        Camera.main.transform.position = camOriginalPosition;
        Camera.main.transform.eulerAngles = camOriginalRotation;

    	
    	
    	mainLandController.furnitureCancel.SetActive(false);
    	mainLandController.furnitureRotate.SetActive(false);
        mainLandController.furnitureSet.SetActive(false);
        mainLandController.furnitureDelete.SetActive(false);

        foreach(GameObject pet in pets){
            pet.SetActive(true);
        }

        pets.Clear();
        mainLandController.SetFurnitureList();
    }

    //Rotate furniture
    public void Rotate(GameObject rf, Room r, Furniture f){
    	Vector3 newRotations = rf.transform.localEulerAngles + new Vector3(0,90,0);
    	rf.transform.localEulerAngles = new Vector3(newRotations.x, newRotations.y % 360, newRotations.z);

    }

    //Move furniture
    public void MoveObject(Furniture f, Room r, GameObject rf){
        Bounds bounds = r.prefab.transform.Find("Floor").gameObject.GetComponent<MeshRenderer>().bounds;

        BoxCollider bc = rf.GetComponent<BoxCollider>();

        if(f.prefab.tag.Contains("floor")){
            if(r.roomSize == 4){
                Camera.main.transform.localPosition = new Vector3(bounds.center.x, 18, bounds.center.z);
                Camera.main.transform.eulerAngles = new Vector3(90,0,0);
            }
        }
        else if(f.prefab.tag.Contains("wall")){

        }
    }


}
