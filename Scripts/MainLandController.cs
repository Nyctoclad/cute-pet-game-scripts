using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;


public class MainLandController : MonoBehaviour
{
    Player player;
    Vector3 middlePosition;
    public bool petScreenUp = false, swipeCheckAvailable = false, moving, movingAble, crActive = false;
    bool traveling = false; //, travelingUp = false, travelingFront = false, travelingLeft = false, travelingRight = false;
    public GameObject petScreen, landScreen, currentPet, activeMark, relationshipsScreen, friendsListSeparator, friendsScreen, furnitureScreen, fcb, pcb, bcb, confirmBox, friendsPetInfo, petHolder, backSection, furnitureCancel, furnitureRotate, furnitureSet, furnitureDelete;
    public Text petName, type, color, friendPetName, friendPetType, friendPetColor;
    public List<GameObject> friendButtons;
    public Button friendsRequest, pendingFriends, friends, acceptAll, declineAll;
    int i = 0, n = 0;
    public double pbuserID = 0;
    Database database;
    Vector3 originalFriendScreenLocation = new Vector3(), originalFurnitureScreenLocation = new Vector3(), newFurnitureScreenLocation = new Vector3(), travelposition = new Vector3();
    public InputField friendsSearchBar;
    public List<Transform> furnitureButtonHolders;
    public GameObject furnitureButton, materialButton, moveableObject;
    RectTransform furnitureScreenPosition;
    HeightCheck heightCheck;
    public Vector3 frontLocation, topLocation, leftLocation, rightLocation;
    public List<Room> roomBoxes = new List<Room>();
    Room currentRoom = new Room();
    
    //string optionCast = "none";


    // Start is called before the first frame update
    void Start()
    {
        database = GameObject.FindGameObjectWithTag("database").GetComponent<Database>();

        player = database.player;
        middlePosition = new Vector3(8.96f, 4.05f, 7.29f);

        //If the player is logged in, instantiate the player's objects like pets and rooms
        if(player.loggedIn){
            currentPet = database.InstantiatePet(middlePosition, player.pets.IndexOf(player.activePet));
            
            foreach(Room room in player.rooms){

                GameObject instantiatedRoom = GameObject.Instantiate(room.prefab), furnitureMover = new GameObject();
                instantiatedRoom.transform.localPosition = room.location;
                instantiatedRoom.transform.localRotation = room.rotation;

                

                instantiatedRoom.transform.Find("Floor").gameObject.GetComponent<MeshRenderer>().material = room.floor.material;
                instantiatedRoom.transform.Find("LeftWall").gameObject.GetComponent<MeshRenderer>().material = room.wall.material;
                instantiatedRoom.transform.Find("BackWall").gameObject.GetComponent<MeshRenderer>().material = room.wall.material;
                instantiatedRoom.transform.Find("RightWall").gameObject.GetComponent<MeshRenderer>().material = room.wall.material;
                Room roomInformation = instantiatedRoom.transform.Find("RoomInformation").gameObject.GetComponent<Room>();
                roomInformation = room;
                roomInformation.furnitureMover = instantiatedRoom.transform.Find("FurnitureMover").gameObject;
                if(roomInformation.defaultRoom){
                    roomInformation.active = true;
                }
                roomBoxes.Add(roomInformation);
               
            }
        }

        //Variables to help with UI management in main scene
        originalFriendScreenLocation = friendsScreen.GetComponent<RectTransform>().localPosition;

        heightCheck = furnitureScreen.GetComponentInChildren<HeightCheck>();
        originalFurnitureScreenLocation = heightCheck.gameObject.GetComponent<RectTransform>().localPosition;
        newFurnitureScreenLocation = originalFriendScreenLocation - new Vector3(0,430,0);
        furnitureScreenPosition = heightCheck.gameObject.GetComponent<RectTransform>();
        BoxCollider2D bc2D = friendsScreen.GetComponentInChildren<BoxCollider2D>();
        database.RetrieveFurniture(1);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(friendsSearchBar.isFocused){
            SearchUpdate(friendsSearchBar.text);
        }

        if(furnitureScreen.activeSelf && Input.GetMouseButton(0) && !traveling){
            if (TouchToLowerCheck().tag.Contains("furniture") && movingAble){

                movingAble = false;
                moving = true;

            }
        }

        //UI up or down for furniture screen
        if(traveling){
            int i = 0;
            //Going down
            if(travelposition.y < 0)
                i = -n;

            //Going up
            if(travelposition.y >= 0)
                i = n;

            if(((furnitureScreenPosition.localPosition.y > travelposition.y) && heightCheck.down) || ((furnitureScreenPosition.localPosition.y < travelposition.y) && !heightCheck.down)){
                furnitureScreenPosition.localPosition += new Vector3(0,i,0);
                n++;
            }
            else{
                n = 0;
                traveling = false;
            }
        }

      
    }

    //Camera change top or front to see furniture movement easier
    public void CameraChange(string direction){
        if(direction == "default"){
            Camera.main.orthographic = false;
            Camera.main.clearFlags = CameraClearFlags.Skybox;
        }
        else{
            Camera.main.orthographic = true;
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
            Camera.main.backgroundColor = Color.black;
            

            if(direction == "up"){
                
            }
            else if(direction == "front"){

            }
            else if(direction == "left"){

            }
            else if(direction == "right"){

            }
        }
        
    }

    //Instantiate the player's last placed furniture in the room
    public void PutDownFurniture(){
        foreach(Furniture furniture in player.furniture){
            if(furniture.placed){
                GameObject placedFurniture = GameObject.Instantiate(furniture.prefab, new Vector3(0,0,0), new Quaternion(0,0,0,0));
                placedFurniture.transform.localPosition = furniture.location;
                placedFurniture.transform.rotation = new Quaternion(0,0,0,0);
                placedFurniture.transform.localRotation = new Quaternion(0,0,0,0);
                placedFurniture.transform.localRotation = Quaternion.Euler(furniture.rotation);
                FurnitureMove fm = placedFurniture.AddComponent<FurnitureMove>();
                foreach(GameObject r in GameObject.FindGameObjectsWithTag("roombox")){
                    if(r.GetComponent<Room>().roomID == furniture.roomID){
                        fm.bounds = r.GetComponent<Room>().prefab.transform.Find("Floor").gameObject.GetComponent<MeshRenderer>().bounds;
                        fm.furniture = furniture;
                    }
                }
            }
        }
    }

    //Check to see what part of the furniture UI was touched
    GameObject TouchToLowerCheck(){
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        GameObject item = new GameObject();

        if(Physics.Raycast(ray, out hit)){
            GameObject tempObject;
            if(hit.collider != null){
               
                tempObject = hit.collider.gameObject;

                //If the player touched someplace in the room instead of on the furniture UI, move the furniture UI out of the way
                if(tempObject.name == "BackSwipeSection" && !heightCheck.down){
                    
                    if(crActive){
                        furnitureCancel.SetActive(true);
                        furnitureRotate.SetActive(true);
                        furnitureSet.SetActive(true);
                    }
                    travelposition = newFurnitureScreenLocation;
                    heightCheck.down = true;
                    traveling = true;
                    backSection.SetActive(false);
                }

                //If the player touched the furniture UI section, bring the furniture UI back up
                else if(tempObject.tag == "downswipe" && heightCheck.down){

                    backSection.SetActive(true);
                    travelposition = originalFurnitureScreenLocation;
                    if(crActive){
                        furnitureCancel.SetActive(false);
                        furnitureRotate.SetActive(false);
                        furnitureSet.SetActive(false);
                    }
                    heightCheck.down = false;
                    traveling = true;
                }

                item = tempObject;


            }

            
        }

        return item;
    }

    public void SeeScreen(){
        travelposition = newFurnitureScreenLocation;
        heightCheck.down = true;
        traveling = true;
        backSection.SetActive(false);
    }

    public void HideScreen(){
        travelposition = originalFurnitureScreenLocation;
        heightCheck.down = false;
        traveling = true;
        backSection.SetActive(true);
    }

    //Bring up the next pet on swipe
    void SwipeLeft(){
        if(petScreen.activeSelf){
            Destroy(currentPet);
            if(i + 1 >= player.pets.Count){
                i = 0;
            }
            else i++;
            currentPet = database.InstantiatePetInPreview(new Vector3(0,0,0), i, GameObject.FindGameObjectWithTag("petholder").transform);
            CheckActivePet();
        }
    }
    
    //Bring up the previous pet on swipe
    void SwipeRight(){
        if(petScreen.activeSelf){
            Destroy(currentPet);
            if(i - 1 < 0){
                i = player.pets.Count - 1;
            }
            else i--;

            currentPet = database.InstantiatePetInPreview(new Vector3(0,0,0), i, GameObject.FindGameObjectWithTag("petholder").transform);
            CheckActivePet();
        }
    }

    //Manage UI to show if the pet is the current pet instantiated in the main scene
    void CheckActivePet(){

        if(player.activePet.petID == player.pets[i].petID){
            activeMark.SetActive(false);
        }
        else{
            activeMark.SetActive(true);
        }

        petName.text = player.pets[i].petName;
        color.text = player.pets[i].color.colorName;
        type.text = player.pets[i].animalName;
    }

    //Check if the pet screen is active to instantiate the pet in the pet screen or in the main scene
    public void Press(){
        petScreenUp = !petScreenUp;
        if(petScreenUp){  
            Destroy(currentPet);

            SwipeEvents.OnSwipeLeft += SwipeLeft;
            SwipeEvents.OnSwipeRight += SwipeRight;
        
            foreach(Animal pet in player.pets){
                if(pet == player.activePet){
                    break;
                }
                else   i++;
            }
            currentPet = database.InstantiatePetInPreview(new Vector3(0,0,0), player.pets.IndexOf(player.activePet), GameObject.FindGameObjectWithTag("petholder").transform);

            CheckActivePet();
        }
        if(!petScreenUp){
            SwipeEvents.OnSwipeLeft -= SwipeLeft;
            SwipeEvents.OnSwipeRight -= SwipeRight;
            i = 0;
            CheckActivePet();
            Destroy(currentPet);
            currentPet = database.InstantiatePet(middlePosition, player.pets.IndexOf(player.activePet));
        }
    }

    //Select this pet to instantiate in the main scene
    public void SummonPet(){
        player.activePet = player.pets[i];
        CheckActivePet();
    }


    //Retrieve friends information from database
    public void CreateFriendsList(){
        fcb.SetActive(false);
        Debug.Log("Setting friendScreen disable.");
        player.friends.Clear();
        player.friendsPendingIn.Clear();
        player.friendsPendingOut.Clear();
        player.blockedIn.Clear();
        player.blockedOut.Clear();
        if(friendButtons.Count > 0){
            foreach(GameObject button in friendButtons){
                Destroy(button);
            }
            friendButtons.Clear();
        }

        //StartCoroutine(Wait(1f));
  
        Debug.Log("Creating friends list.");

        database.RetriveFriends();

    }

    //Create friends list screen making buttons in a list for each friend
    public void SetFriendsList(){
        
        Vector3 lastLocation, firstLocation = new Vector3(256,-96,0);

        lastLocation = firstLocation;
        if(player.friendsPendingIn.Count > 0){
            
            acceptAll.gameObject.SetActive(true);
            declineAll.gameObject.SetActive(true);
            for(int l = 0; l < player.friendsPendingIn.Count; l++){
                if(player.friendsPendingIn[l].userID == pbuserID){
                    player.friendsPendingIn.RemoveAt(l);
                    break;
                }
            }

            foreach(Player friend in player.friendsPendingIn){
                GameObject pend = GameObject.Instantiate(friendsRequest.gameObject, lastLocation, new Quaternion(0,0,0,0), pcb.transform);
                pend.GetComponent<RectTransform>().localPosition = lastLocation;
                pend.name = "Friend " + player.friendsPendingIn.IndexOf(friend);
                pend.tag = "fpin";
                PlayerButtonInfo pbi = pend.GetComponent<PlayerButtonInfo>();
                pbi.username.text = friend.displayName;
                pbi.confirmBox = confirmBox;

                pbi.friend = friend;
                pbi.playerName = friend.displayName;
                if(friend.online > 0)
                    pbi.Online();
                else pbi.Offline();

                lastLocation = lastLocation + new Vector3(0,-152,0);
                friendButtons.Add(pend);
            }
            
        }

        if(player.friendsPendingOut.Count > 0){
            Debug.Log("This person has asked someone to be friends.");
            lastLocation = firstLocation;
            acceptAll.gameObject.SetActive(false);
            declineAll.gameObject.SetActive(false);
            foreach(Player friend in player.friendsPendingOut){
                Debug.Log("PENDING OUT " + friend.displayName);
                GameObject pend = GameObject.Instantiate(friendsRequest.gameObject, lastLocation, new Quaternion(0,0,0,0), fcb.transform);
                pend.GetComponent<RectTransform>().localPosition = lastLocation;
                pend.name = "Friend " + player.friendsPendingOut.IndexOf(friend);
                pend.tag = "fpout";
                PlayerButtonInfo pbi = pend.GetComponent<PlayerButtonInfo>();
                pbi.username.text = friend.displayName;
                if(friend.online > 0)
                    pbi.Online();
                else pbi.Offline();
                lastLocation = lastLocation + new Vector3(0,-152,0);
                friendButtons.Add(pend);
            }
        }

        if(player.friends.Count > 0){
            lastLocation = firstLocation;
            foreach(Player friend in player.friends){

                Debug.Log("FRIEND " + friend.displayName);
                GameObject pend = GameObject.Instantiate(friendsRequest.gameObject, lastLocation, new Quaternion(0,0,0,0), fcb.transform);
                pend.GetComponent<RectTransform>().localPosition = lastLocation;
                pend.name = "Friend " + player.friends.IndexOf(friend);
                pend.tag = "friends";
                
                
                Debug.Log("ADDING FRIEND INFO STUFF.");
                PlayerButtonInfo pbi = pend.GetComponent<PlayerButtonInfo>();
                pbi.username.text = friend.displayName;
                pbi.friend = friend;
                pbi.playerName = friend.displayName;
                Debug.Log("Added username " + pbi.username.text);
                pend.GetComponent<Button>().onClick.AddListener(()=> pbi.ShowFriendsPetInfo());
                pbi.petName = friendPetName;
                pbi.petType = friendPetType;
                pbi.petColor = friendPetColor;
                pbi.friendsPetInfo = friendsPetInfo;

                if(friend.online > 0)
                    pbi.Online();
                else pbi.Offline();
                lastLocation = lastLocation + new Vector3(0,-152,0);
                friendButtons.Add(pend);
            }

            fcb.SetActive(true);
        }

        if(player.blockedOut.Count > 0){
            lastLocation = firstLocation;
            for(int l = 0; l < player.blockedOut.Count; l++){
                if(player.blockedOut[l].userID == pbuserID){
                    player.blockedOut.RemoveAt(l);
                    break;
                }
            }
            foreach(Player friend in player.blockedOut){
                Debug.Log("BLOCKED " + friend.displayName);
                GameObject pend = GameObject.Instantiate(friendsRequest.gameObject, lastLocation, new Quaternion(0,0,0,0), fcb.transform);
                pend.GetComponent<RectTransform>().localPosition = lastLocation;
                pend.name = "Friend " + player.blockedOut.IndexOf(friend);
                pend.tag = "bout";

                PlayerButtonInfo pbi = pend.GetComponent<PlayerButtonInfo>();
                pbi.username.text = friend.displayName;
                pbi.confirmBox = confirmBox;

                

                if(friend.online > 0)
                    pbi.Online();
                else pbi.Offline();
                lastLocation = lastLocation + new Vector3(0,-152,0);
                friendButtons.Add(pend);
            }

        }
        friendsScreen.SetActive(true);

        ResetUI(0.5f);
        pbuserID = 0;
    }

    //Retrieve player furniture information from the database
    public void CreateFurnitureList(){
        player.furniture.Clear();
        database.RetrieveFurniture();
    }

    //Load the furniture buttons into the UI and bring up the furniture screen
    public void SetFurnitureList(){
        furnitureScreen.SetActive(true);
        Room room = new Room();
        foreach(GameObject r in GameObject.FindGameObjectsWithTag("roombox")){
            if(r.GetComponent<Room>().active){
                room = r.GetComponent<Room>();
                break;
            }
        }
        foreach(Transform furnitureHolder in furnitureButtonHolders){
            foreach(Transform child in furnitureHolder){
                GameObject.Destroy(child.gameObject);
            }
        }

        //Determine which category to put each furniture item in
        foreach(Furniture furniture in player.furniture){
            Transform holder;
            if(!furniture.placed){

                GameObject button = new GameObject(), icon = new GameObject();
                switch(furniture.furnitureType){
                    
                    case 1:
                        foreach(Transform furnitureHolder in furnitureButtonHolders){
                            if(furnitureHolder.parent.gameObject.name.ToLower().Contains("walls")){
                                //Use a material button
                                holder = furnitureHolder;
                                button = GameObject.Instantiate(materialButton, new Vector3(0,0,0), new Quaternion(0,0,0,0), holder);
                                GameObject.Instantiate(furniture.material, new Vector3(0,0,0), new Quaternion(0,0,0,0), button.transform.Find("MaterialHolder"));

                                break;
                            }
                        }
                    break;

                    case 2:
                        foreach(Transform furnitureHolder in furnitureButtonHolders){
                            if(furnitureHolder.parent.gameObject.name.ToLower().Contains("floor")){
                                //Use a material button
                                holder = furnitureHolder;
                                button = GameObject.Instantiate(materialButton, new Vector3(0,0,0), new Quaternion(0,0,0,0), holder);
                                GameObject.Instantiate(furniture.material, new Vector3(0,0,0), new Quaternion(0,0,0,0), button.transform.Find("MaterialHolder"));
                                break;
                            }
                        }
                    break;

                    case 3:
                        foreach(Transform furnitureHolder in furnitureButtonHolders){
                            if(furnitureHolder.parent.gameObject.name.ToLower().Contains("table")){
                                //Use a material button
                                holder = furnitureHolder;
                                button = GameObject.Instantiate(furnitureButton, new Vector3(0,0,0), new Quaternion(0,0,0,0), holder);
                                GameObject.Instantiate(furniture.icon, new Vector3(0,0,0), new Quaternion(0,0,0,0), button.transform.Find("FurnitureHolder"));
                                break;
                            }
                        }
                    break;

                    case 4:
                        foreach(Transform furnitureHolder in furnitureButtonHolders){
                            if(furnitureHolder.parent.gameObject.name.ToLower().Contains("chair")){
                                //Use a material button
                                holder = furnitureHolder;
                                button = GameObject.Instantiate(furnitureButton, new Vector3(0,0,0), new Quaternion(0,0,0,0), holder);
                                GameObject.Instantiate(furniture.icon, new Vector3(0,0,0), new Quaternion(0,0,0,0), button.transform.Find("FurnitureHolder"));
                                break;
                            }
                        }
                    break;

                    case 5:
                        foreach(Transform furnitureHolder in furnitureButtonHolders){
                            if(furnitureHolder.parent.gameObject.name.ToLower().Contains("bed")){
                                //Use a material button
                                holder = furnitureHolder;
                                button = GameObject.Instantiate(furnitureButton, new Vector3(0,0,0), new Quaternion(0,0,0,0), holder);
                                button.transform.localPosition = new Vector3(button.transform.localPosition.x, button.transform.localPosition.y, -60);
                                button.gameObject.GetComponent<FurniturePlacement>().furniture = furniture;
                                icon = GameObject.Instantiate(furniture.icon, new Vector3(0,0,0), new Quaternion(0,0,0,0), button.transform.Find("FurnitureHolder"));
                                icon.transform.localPosition = new Vector3(0,0,0);
                                break;
                            }
                        }
                    break;

                    case 6:
                        foreach(Transform furnitureHolder in furnitureButtonHolders){
                            if(furnitureHolder.parent.gameObject.name.ToLower().Contains("storage")){
                                //Use a material button
                                holder = furnitureHolder;
                                button = GameObject.Instantiate(furnitureButton, new Vector3(0,0,0), new Quaternion(0,0,0,0), holder);
                                GameObject.Instantiate(furniture.icon, new Vector3(0,0,0), new Quaternion(0,0,0,0), button.transform.Find("FurnitureHolder"));
                                break;
                            }
                        }
                    break;

                    case 7:
                        foreach(Transform furnitureHolder in furnitureButtonHolders){
                            if(furnitureHolder.parent.gameObject.name.ToLower().Contains("bath")){
                                //Use a material button
                                holder = furnitureHolder;
                                button = GameObject.Instantiate(furnitureButton, new Vector3(0,0,0), new Quaternion(0,0,0,0), holder);
                                GameObject.Instantiate(furniture.icon, new Vector3(0,0,0), new Quaternion(0,0,0,0), button.transform.Find("FurnitureHolder"));
                                break;
                            }
                        }
                    break;

                    case 8:
                        foreach(Transform furnitureHolder in furnitureButtonHolders){
                            if(furnitureHolder.parent.gameObject.name.ToLower().Contains("kitchen")){
                                //Use a material button
                                holder = furnitureHolder;
                                button = GameObject.Instantiate(furnitureButton, new Vector3(0,0,0), new Quaternion(0,0,0,0), holder);
                                GameObject.Instantiate(furniture.icon, new Vector3(0,0,0), new Quaternion(0,0,0,0), button.transform.Find("FurnitureHolder"));
                                break;
                            }
                        }
                    break;

                    case 9:
                        foreach(Transform furnitureHolder in furnitureButtonHolders){
                            if(furnitureHolder.parent.gameObject.name.ToLower().Contains("electronics")){
                                //Use a material button
                                holder = furnitureHolder;
                                button = GameObject.Instantiate(furnitureButton, new Vector3(0,0,0), new Quaternion(0,0,0,0), holder);
                                GameObject.Instantiate(furniture.icon, new Vector3(0,0,0), new Quaternion(0,0,0,0), button.transform.Find("FurnitureHolder"));
                                break;
                            }
                        }
                    break;
                    
                    case 10:
                        foreach(Transform furnitureHolder in furnitureButtonHolders){
                            if(furnitureHolder.parent.gameObject.name.ToLower().Contains("light")){
                                //Use a material button
                                holder = furnitureHolder;
                                button = GameObject.Instantiate(furnitureButton, new Vector3(0,0,0), new Quaternion(0,0,0,0), holder);
                                GameObject.Instantiate(furniture.icon, new Vector3(0,0,0), new Quaternion(0,0,0,0), button.transform.Find("FurnitureHolder"));
                                break;
                            }
                        }
                    break;

                    case 11:
                        foreach(Transform furnitureHolder in furnitureButtonHolders){
                            if(furnitureHolder.parent.gameObject.name.ToLower().Contains("wall deco")){
                                //Use a material button
                                holder = furnitureHolder;
                                button = GameObject.Instantiate(furnitureButton, new Vector3(0,0,0), new Quaternion(0,0,0,0), holder);
                                GameObject.Instantiate(furniture.icon, new Vector3(0,0,0), new Quaternion(0,0,0,0), button.transform.Find("FurnitureHolder"));
                                break;
                            }
                        }
                    break;
                }
                if(button.GetComponent<Button>() != null){
                    button.GetComponent<Button>().onClick.RemoveAllListeners();
                    button.GetComponent<Button>().onClick.AddListener(() => room.InstantiateFurniture(furniture, button.GetComponent<Button>()));
                }

            }
        }
    }

    //Resets and updates the user interface
    public void ResetUI(float seconds = 1f){
        pcb.SetActive(false);
        bcb.SetActive(false);
        StartCoroutine(WaitForReset(seconds));
    }

    //Allow the individual friends information page to show by sending the main friends page backwards
    public void SendBackward(){
        friendsScreen.transform.localPosition = originalFriendScreenLocation + new Vector3(0,0,400);
    }

    //Bring the main friends page to front
    public void SendForward(){
        friendsScreen.transform.localPosition = originalFriendScreenLocation;
        foreach(Transform child in petHolder.transform){
            Destroy(child.gameObject);
        }
    }

    //Update the friends list based on what the user is typing in
    public void SearchUpdate(string searchText){
        Vector3 lastLocation, firstLocation = new Vector3(256,-96,0);
        lastLocation = firstLocation;
        if(!(string.IsNullOrEmpty(searchText) || string.IsNullOrWhiteSpace(searchText))){
            foreach(GameObject pend in friendButtons){
                if(pend.tag == "friends" && pend.GetComponent<PlayerButtonInfo>().playerName.ToLower().StartsWith(searchText.ToLower())){
                    pend.SetActive(true);
                    pend.GetComponent<RectTransform>().localPosition = lastLocation;
                    lastLocation = lastLocation + new Vector3(0,-152,0);
                }
                else{
                    pend.SetActive(false);
                }
            }
        }
        else{
            foreach(GameObject pend in friendButtons){
                if(pend.tag == "friends"){
                    pend.GetComponent<RectTransform>().localPosition = lastLocation;
                    lastLocation = lastLocation + new Vector3(0,-152,0);
                    pend.SetActive(true);
                }
            }
        }
    }

    //Updating the UI
    IEnumerator WaitForReset(float seconds){

        yield return new WaitForSeconds(seconds);
        Canvas.ForceUpdateCanvases();
        pcb.SetActive(true);
        pcb.SetActive(true);

        Debug.Log("Finished RESET.");
        
    }

    //Wait for a certain number of seconds
    public IEnumerator Wait(float seconds){
        yield return new WaitForSeconds(seconds);
    }


}
