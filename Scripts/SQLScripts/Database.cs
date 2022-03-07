using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;


public class Database : MonoBehaviour
{
    string url = "http://64.225.63.10/CPG/PHPs/";
    double lastID;
    public AnimalList animalList;
    public FurnitureList furnitureList;
    public Messages messages;
    public SurfaceMaterials surfaceMaterials;
    public List<RoomSize> roomSizes = new List<RoomSize>();
    public TagList furnitureTags, roomTags;
    public Player player;
    
    void Start()
    {
        if(!player){
            player = ScriptableObject.CreateInstance<Player>();
            player.displayName = "test";
        }
        
        DontDestroyOnLoad(this.gameObject);
        MessageComponents();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Initiate message components for pop-up messages to the user
    void MessageComponents(){
        Transform errorObject, inputObject, confirmObject;
        errorObject = messages.ErrorMessage.transform.GetChild(0).GetChild(0);
        inputObject = messages.InputMessage.transform.GetChild(0).GetChild(0);
        confirmObject = messages.ConfirmMessage.transform.GetChild(0).GetChild(0);

        foreach(Transform erroritem in errorObject.transform){
            if(erroritem.gameObject.tag == "title")
                messages.errorTitle = erroritem.gameObject.GetComponent<Text>();
            
            if(erroritem.gameObject.tag == "text")
                messages.errorText = erroritem.gameObject.GetComponent<Text>();
        }

        foreach(Transform inputItem in inputObject.transform){  
            if(inputItem.gameObject.tag == "submit")
                messages.inputButton = inputItem.gameObject.GetComponent<Button>();
            
            if(inputItem.gameObject.tag == "input")
                messages.input = inputItem.gameObject.GetComponent<InputField>();
            
        }

        foreach(Transform confirmitem in confirmObject.transform){
            if(confirmitem.gameObject.tag == "title")
                messages.confirmTitle = confirmitem.gameObject.GetComponent<Text>();
            
            if(confirmitem.gameObject.tag == "text")
                messages.confirmText = confirmitem.gameObject.GetComponent<Text>();
            
            if(confirmitem.gameObject.tag == "submit")
                messages.confirmButton = confirmitem.gameObject.GetComponent<Button>();
        }
    }

    //Method for instantiating the pet in the main scene
    public GameObject InstantiatePet(Vector3 position, int petIndex){
        GameObject pet = GameObject.Instantiate(player.pets[petIndex].prefab,position, player.pets[petIndex].prefab.transform.rotation);
        SkinnedMeshRenderer body = pet.GetComponent<PetInformation>().body, face = pet.GetComponent<PetInformation>().face;
        body.material = player.pets[petIndex].color.animalMaterial;
        face.material = player.pets[petIndex].face.faceMaterial;
        return pet;
    }

    //Method for instantiating the pet in the pet information screen
    public GameObject InstantiatePetInPreview(Vector3 position, int petIndex, Transform parent){
        GameObject pet = GameObject.Instantiate(player.pets[petIndex].prefab,position, player.pets[petIndex].prefab.transform.rotation, parent);
        SkinnedMeshRenderer body = pet.GetComponent<PetInformation>().body, face = pet.GetComponent<PetInformation>().face;
        pet.transform.localPosition = position;
        pet.transform.localScale = new Vector3(730.4f, 730.4f, 2f);
        body.material = player.pets[petIndex].color.animalMaterial;
        face.material = player.pets[petIndex].face.faceMaterial;
        return pet;
    }

    //Start coroutine to create a new user with a new userID
    public void CreateUser(){
        StartCoroutine(CheckUser());
    }

    //Start the coroutine to retireve user information
    public void RetrieveUser(){
        StartCoroutine(RetrieveUserInformationEnum(player, false));
    }

    //Start the coroutine to retrieve pet information
    public void RetrievePet(){
        StartCoroutine(RetrievePetEnum());
    }

    //Start coroutine to retrieve a user's friends list
    public void RetriveFriends(){  
        StartCoroutine(RetrieveFriendsEnum());
    }

    //Go through friends and add their information
    public void RetrieveFriendsUserInformation(){
        foreach(Player friend in player.friends){
            StartCoroutine(RetrieveUserInformationEnum(friend, true));
        }
        foreach(Player friend in player.friendsPendingIn){
            StartCoroutine(RetrieveUserInformationEnum(friend, true));
        }
        foreach(Player friend in player.friendsPendingOut){
            StartCoroutine(RetrieveUserInformationEnum(friend, true));
        }
        foreach(Player friend in player.blockedOut){
            StartCoroutine(RetrieveUserInformationEnum(friend, true));
        }

        if(SceneManager.GetActiveScene().name == "MainScene")
            GameObject.FindGameObjectWithTag("land_information").GetComponent<MainLandController>().SetFriendsList();
    }

    //Start the coroutine to load furniture, default choice 0 for new list, 1 for if the user has already been loaded and the list is reloading
    public void RetrieveFurniture(int choice = 0){
        StartCoroutine(RetrieveFurnitureEnum(choice));
    }

    //Start the coroutine to load user rooms
    public void RetriveRooms(){
        StartCoroutine(RetrieveRoomsEnum());
    }

    //Submit all user rooms
    public void SubmitRooms(double userID, int roomSize, int defaultRoom, int floorMaterialID, int wallMaterialID, string roomLocation, string roomRotation, int roomID){
        WWWForm form = new WWWForm();
        string phpURL = "SubmitRooms.php";


        form.AddField("user_id", userID.ToString());
        form.AddField("room_size", roomSize);
        form.AddField("default_room", defaultRoom);
        form.AddField("floor_material_id", floorMaterialID);
        form.AddField("wall_material_id", wallMaterialID);
        form.AddField("room_location", roomLocation);
        form.AddField("room_rotation", roomRotation);
        form.AddField("user_room_id", roomID);

        StartCoroutine(SubmitSQL(phpURL, form));
    }

    //Submit user furniture
    public void SubmitFurniture(double userFurnitureID, string furnitureID, string furnitureColorID, string furnitureLocation, string furnitureRotation, int roomID){
        WWWForm form = new WWWForm();
        string phpURL = "SubmitFurniture.php";

        form.AddField("user_id", player.userID.ToString());
        form.AddField("user_furniture_id", userFurnitureID.ToString());
        form.AddField("furniture_id", furnitureID);
        form.AddField("furniture_color_id", furnitureColorID);
        form.AddField("furniture_location", furnitureLocation);
        form.AddField("furniture_rotation", furnitureRotation);
        form.AddField("user_room_id", roomID);

        StartCoroutine(SubmitSQL(phpURL, form));
    }

    //Submit user information to the SQL database
    public void SubmitNewUserInfo(){
        WWWForm form = new WWWForm();
        string phpURL = "SubmitNewUser.php"; 

        form.AddField("username", player.displayName);
        form.AddField("user_id", player.userID.ToString());
        form.AddField("active", "1");

        StartCoroutine(SubmitSQL(phpURL, form));

    }

    //Create new pet to add to the user's pet list
    public void AddNewPet(Animal pet, string petName){
        Animal newPet = ScriptableObject.CreateInstance<Animal>();
        newPet.animalType = pet.animalType;
        newPet.prefab = pet.prefab;
        newPet.color = pet.color;
        newPet.face = pet.face;
        newPet.glow = pet.glow;
        newPet.special = pet.special;
        newPet.accessories.Clear();
        foreach(Accessory accessory in pet.accessories){
            newPet.accessories.Add(accessory);
        }
        newPet.animalName = pet.animalName;
        newPet.petName = petName;
        newPet.clothing = pet.clothing;

        player.pets.Add(newPet);

        player.pets[player.pets.IndexOf(newPet)].petID = player.pets.IndexOf(newPet);

        if(player.pets.Count == 1)
            player.activePet = player.pets[0];

        SubmitNewPetInfo();
    }

    //Submit new pet information to the SQL database
    public void SubmitNewPetInfo(){
        foreach(Animal pet in player.pets){
            WWWForm form = new WWWForm();
            string phpURL = "SubmitNewPet.php";

            form.AddField("user_id", player.userID.ToString());
            form.AddField("pet_name", pet.petName);
            form.AddField("pet_id", pet.petID);
            form.AddField("pet_type", pet.animalType);
            form.AddField("pet_color", pet.color.color);
            form.AddField("pet_glow", pet.glow);
            form.AddField("pet_special", pet.special);
            form.AddField("pet_face", pet.face.faceID);
            form.AddField("pet_clothing", pet.clothing.clothingID);

            if(pet == player.activePet)
                form.AddField("active", "1");
            if(pet != player.activePet)
                form.AddField("active", "0");

            StartCoroutine(SubmitSQL(phpURL, form));

            foreach(Accessory accessory in pet.accessories){
                WWWForm accessoryForm = new WWWForm();
                string accessoryPHPURL = "SubmitPetAccessories.php";

                accessoryForm.AddField("user_id", player.userID.ToString());
                accessoryForm.AddField("pet_id", pet.petID);
                accessoryForm.AddField("accessory_name", accessory.accessoryName);
                accessoryForm.AddField("accessory_id", accessory.accessoryID);
                accessoryForm.AddField("user_accessory_id", accessory.userAccessoryID.ToString());

                StartCoroutine(SubmitSQL(accessoryPHPURL, accessoryForm));
            }

        }
    }

    //Update friends information
    public void UpdateFriends(double friendUserID, int pendingUserToFriend, int pendingFriendToUser, int mutualFriend, int blockUserToFriend, int blockFriendToUser){
        string phpURL = "UpdateFriends.php";
        WWWForm form = new WWWForm();
        form.AddField("user_id", player.userID.ToString());
        form.AddField("friend_user_id", friendUserID.ToString());
        form.AddField("pending_user_to_friend", pendingUserToFriend);
        form.AddField("pending_friend_to_user", pendingFriendToUser);
        form.AddField("mutual_friend", mutualFriend);
        form.AddField("block_user_to_friend", blockUserToFriend);
        form.AddField("block_friend_to_user", blockFriendToUser);

        StartCoroutine(SubmitSQL(phpURL, form));
    }

    //Coroutine for submitting SQL information
    IEnumerator SubmitSQL(string phpURL, WWWForm form){
        UnityWebRequest sqlWebRequest;
        using(sqlWebRequest = UnityWebRequest.Post(url + phpURL, form)){
            yield return sqlWebRequest.SendWebRequest();
            if(sqlWebRequest.isNetworkError){
                //Debug.Log(sqlWebRequest.error);
                ShowError("Unable to save information.", "Network Error");
            }
                
            else{
                sqlWebRequest.Dispose();
                StopCoroutine(SubmitSQL(phpURL, form));
            }
        }
    }

    //Coroutine for retrieving user rooms from SQL database
    IEnumerator RetrieveRoomsEnum(){
        UnityWebRequest wwwRetrieveRooms;
        WWWForm form = new WWWForm();
        form.AddField("user_id", player.userID.ToString());

        using(wwwRetrieveRooms = UnityWebRequest.Post(url + "RetrieveRooms.php", form)){
            yield return wwwRetrieveRooms.SendWebRequest();
            if(wwwRetrieveRooms.isNetworkError){
                Debug.Log(wwwRetrieveRooms.error);
                ShowError(wwwRetrieveRooms.error);
            }
            else{
                Debug.Log(wwwRetrieveRooms.downloadHandler.text);

                String[] lines = wwwRetrieveRooms.downloadHandler.text.Split(';');

                foreach(string line in lines){
                    if(!(String.IsNullOrEmpty(line) || String.IsNullOrWhiteSpace(line))){
                        Room room = new Room();

                        room.roomSize = int.Parse(line.Split(',')[0]);
                        foreach(RoomSize roomSize in roomSizes){
                            if(roomSize.size == room.roomSize){
                                room.prefab = roomSize.prefab;
                                break;
                            }
                        }

                        room.defaultRoom = Convert.ToBoolean(int.Parse(line.Split(',')[1]));
                        foreach(Mat mat in surfaceMaterials.materials){
                            if(mat.num == int.Parse(line.Split(',')[2])){
                                room.floor = mat;
                            }
                            if(mat.num == int.Parse(line.Split(',')[3])){
                                room.wall = mat;
                            }
                        }
                        String loc = line.Split(',')[4];
                        String rot = line.Split(',')[5];

                        room.rotation = Quaternion.Euler(float.Parse(rot.Split(':')[0]), float.Parse(rot.Split(':')[1]), float.Parse(rot.Split(':')[2]));
                        
                        float xloc = float.Parse(loc.Split(':')[0]);
                        
                        float yloc = float.Parse(loc.Split(':')[1]);
                        
                        float zloc = float.Parse(loc.Split(':')[2]);
                        
                        room.location = new Vector3(xloc, yloc, zloc);
                        room.roomID = int.Parse(line.Split(',')[6]);
                        player.rooms.Add(room);
                    }

                }
                

                wwwRetrieveRooms.Dispose();
                StopCoroutine(RetrieveRoomsEnum());
        
                
            }
        }
    }

    //Coroutine for retrieving user furniture from SQL database
    IEnumerator RetrieveFurnitureEnum(int choice){
        if(choice > 0)
            player.furniture.Clear();

        UnityWebRequest wwwRetrieveFurniture;
        WWWForm form = new WWWForm();
        form.AddField("user_id", player.userID.ToString());

        using(wwwRetrieveFurniture = UnityWebRequest.Post(url + "RetrieveFurniture.php", form)){
            yield return wwwRetrieveFurniture.SendWebRequest();
            if(wwwRetrieveFurniture.isNetworkError){
               // Debug.Log(wwwRetrieveFurniture.error);
                ShowError(wwwRetrieveFurniture.error);
            }
            else{
              //  Debug.Log(wwwRetrieveFurniture.downloadHandler.text);
                string[] lines = wwwRetrieveFurniture.downloadHandler.text.Split(';');
                foreach(string line in lines){
                    if(!(string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))){
                        Furniture furniture = ScriptableObject.CreateInstance<Furniture>();
                        furniture.userFurnitureID = double.Parse(line.Split(',')[4]);
                        furniture.furnitureID = int.Parse(line.Split(',')[0]);
                        furniture.furnitureColorID = int.Parse(line.Split(',')[1]);
                        foreach(Furniture f in furnitureList.furniture){
                            if(f.furnitureID == furniture.furnitureID){
                                if(f.furnitureColorID == furniture.furnitureColorID){
                                    furniture.colorName = f.colorName;
                                    furniture.furnitureName = f.furnitureName;
                                    furniture.prefab = f.prefab;
                                    furniture.furnitureType = f.furnitureType;
                                    furniture.icon = f.icon;
                                    if(f.material)
                                        furniture.material = f.material;
                                    break;
                                }

                            }
                        }
                        string furnitureLocation = line.Split(',')[2];
                        if(!furnitureLocation.Contains("n")){
                            Vector3 loc = new Vector3(float.Parse(furnitureLocation.Split(':')[0]),float.Parse(furnitureLocation.Split(':')[1]),float.Parse(furnitureLocation.Split(':')[2]));
                            furniture.location = loc;
                            furniture.placed = true;
                            string furnitureRotation = line.Split(',')[3];
                            furniture.rotation = new Vector3(float.Parse(furnitureRotation.Split(':')[0]), float.Parse(furnitureRotation.Split(':')[1]), float.Parse(furnitureRotation.Split(':')[2]));
                        }
                        if(furnitureLocation.Contains("n")){
                            furniture.placed = false;
                            furniture.location = new Vector3();
                        }
                        player.furniture.Add(furniture);
                    }
                }
                

                wwwRetrieveFurniture.Dispose();
                if(choice <= 0){
                    GameObject.FindGameObjectWithTag("land_information").GetComponent<MainLandController>().SetFurnitureList();
                }
                if(choice > 0)
                    GameObject.FindGameObjectWithTag("land_information").GetComponent<MainLandController>().PutDownFurniture();

                StopCoroutine(RetrieveFurnitureEnum(choice));
                
            }
        }
    }
    
    //Coroutine for retrieving user or friend information from SQL database
    IEnumerator RetrieveUserInformationEnum(Player user, bool isFriend){
        UnityWebRequest wwwRetriveUserInformation;
        WWWForm form = new WWWForm();
        form.AddField("user_id", user.userID.ToString());

        using(wwwRetriveUserInformation = UnityWebRequest.Post(url + "RetrieveUser.php", form)){
            yield return wwwRetriveUserInformation.SendWebRequest();
            if(wwwRetriveUserInformation.isNetworkError){
                // Debug.Log(wwwRetriveUserInformation.error);
                //ShowError(wwwRetriveUserInformation.error);
            }
            else{

                String line = wwwRetriveUserInformation.downloadHandler.text.Split(';')[0];
                user.displayName = line.Split(',')[0];
                

                wwwRetriveUserInformation.Dispose();


                if(isFriend){
                    UnityWebRequest wwwRetrievePet;
                    using(wwwRetrievePet = UnityWebRequest.Post(url + "RetrievePet.php", form)){
                        yield return wwwRetrievePet.SendWebRequest();
                        if(wwwRetrievePet.isNetworkError){
                            // Debug.Log(wwwRetrievePet.error);
                            //ShowError(wwwRetrievePet.error);
                        }
                        else{
                            String[] lines = wwwRetrievePet.downloadHandler.text.Split(';');
                            AddAnimals(lines, user);
                            
                            wwwRetrievePet.Dispose();                           
                        }
                    } 
                }
            }
        }

        StopCoroutine(RetrieveUserInformationEnum(user, isFriend));

    }
    
    //Coroutine for retrieving the user's friends list from SQL database
    IEnumerator RetrieveFriendsEnum(){
        UnityWebRequest wwwRetrieveFriends;
        WWWForm form = new WWWForm();
        form.AddField("user_id", player.userID.ToString());

        using(wwwRetrieveFriends = UnityWebRequest.Post(url + "RetrieveFriends.php", form)){
            yield return wwwRetrieveFriends.SendWebRequest();
            if(wwwRetrieveFriends.isNetworkError){
                Debug.Log(wwwRetrieveFriends.error);
                ShowError(wwwRetrieveFriends.error);
            }
            else{
                //Debug.Log(wwwRetrieveFriends.downloadHandler.text);

                foreach(String line in wwwRetrieveFriends.downloadHandler.text.Split(';')){
                    //If you are the user!
                    if(!(string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))){
                        if(double.Parse(line.Split(',')[0]) == player.userID){
                            Player friend = ScriptableObject.CreateInstance<Player>();
                            double friendNum = double.Parse(line.Split(',')[1]);
                            //Debug.Log("FriendNum is: " + friendNum);
                            friend.userID = friendNum;
                            if(int.Parse(line.Split(',')[2]) == 1){
                                player.friendsPendingOut.Add(friend);
                            }
                            else if(int.Parse(line.Split(',')[3]) == 1){
                                player.friendsPendingIn.Add(friend);
                            }
                            else if(int.Parse(line.Split(',')[4]) == 1){
                                player.friends.Add(friend);
                            }
                            else if(int.Parse(line.Split(',')[5]) == 1){
                                player.blockedOut.Add(friend);
                            }
                            else if(int.Parse(line.Split(',')[6]) == 1){
                                player.blockedIn.Add(friend);
                            }

                           // Debug.Log("Friend user ID is: " + friend.userID);

                        }
                        else if(double.Parse(line.Split(',')[1]) == player.userID){
                            Player friend = ScriptableObject.CreateInstance<Player>();
                            double friendNum = double.Parse(line.Split(',')[0]);
                            friend.userID = friendNum;
                           // Debug.Log("Friendnum is " + friendNum);
                            if(int.Parse(line.Split(',')[2]) == 1){
                                player.friendsPendingIn.Add(friend);
                            }
                            else if(int.Parse(line.Split(',')[3]) == 1){
                                player.friendsPendingOut.Add(friend);
                            }
                            else if(int.Parse(line.Split(',')[4]) == 1){
                                player.friends.Add(friend);
                            }
                            else if(int.Parse(line.Split(',')[5]) == 1){
                                player.blockedIn.Add(friend);
                            }
                            else if(int.Parse(line.Split(',')[6]) == 1){
                                player.blockedOut.Add(friend);
                            }
                        }
                    }
                    //If you are the friend!
                }
                 wwwRetrieveFriends.Dispose();
                
                RetrieveFriendsUserInformation();
                StopCoroutine(RetrieveFriendsEnum());
        
                
            }
        }
    }

    //Coroutine for retirving the user's pets list from SQL database
    IEnumerator RetrievePetEnum(){
        UnityWebRequest wwwRetrievePet;
        WWWForm form = new WWWForm();
        form.AddField("user_id", player.userID.ToString());

        using(wwwRetrievePet = UnityWebRequest.Post(url + "RetrievePet.php", form)){
            yield return wwwRetrievePet.SendWebRequest();
            if(wwwRetrievePet.isNetworkError){
                //Debug.Log(wwwRetrievePet.error);
                //ShowError(wwwRetrievePet.error);
            }
            else{
                String[] lines = wwwRetrievePet.downloadHandler.text.Split(';');
                AddAnimals(lines, player);
                wwwRetrievePet.Dispose();
                StopCoroutine(RetrievePetEnum());
        
                
            }
        }
    }

    //Coroutine for retrieving pet accessories for each pet from SQL database
    IEnumerator RetrievePetAccessoriesEnum(double passedPlayerID, Animal pet){
        UnityWebRequest wwwRetrievePetAccessory;
        WWWForm form = new WWWForm();
        form.AddField("user_id", passedPlayerID.ToString());
        form.AddField("pet_id", pet.petID);

        using(wwwRetrievePetAccessory = UnityWebRequest.Post(url + "RetrievePetAccessory.php", form)){
            yield return wwwRetrievePetAccessory.SendWebRequest();
            if(wwwRetrievePetAccessory.isNetworkError){
                Debug.Log(wwwRetrievePetAccessory.error);
                ShowError(wwwRetrievePetAccessory.error);
            }
            else{

                String[] lines = wwwRetrievePetAccessory.downloadHandler.text.Split(';');
                foreach(String line in lines){
                    if(!(String.IsNullOrEmpty(line) || String.IsNullOrWhiteSpace(line))){
                        Accessory accessory = ScriptableObject.CreateInstance<Accessory>();
                        accessory.accessoryName = line.Split(',')[0];
                        accessory.accessoryID = int.Parse(line.Split(',')[1]);
                        accessory.userAccessoryID = int.Parse(line.Split(',')[2]);

                        //Go through in game accessory database to retrieve other information via accessory ID

                        pet.accessories.Add(accessory);
                    }
                }
                wwwRetrievePetAccessory.Dispose();
                StopCoroutine(RetrievePetEnum());
        
                
            }
        }
    }

    //Add animals to the player's pet list from the SQL text response
    void AddAnimals(string[] lines, Player passedPlayer){
        foreach(String line in lines){
            if(!string.IsNullOrEmpty(line) || !string.IsNullOrWhiteSpace(line)){
                Animal pet = ScriptableObject.CreateInstance<Animal>();
                pet.petName = line.Split(',')[0];
                pet.petID = int.Parse(line.Split(',')[1]);
                pet.animalType = int.Parse(line.Split(',')[2]);
                
                //Get name of animal and other animal information!
                foreach(Animal animal in animalList.animals){
                    if(animal.animalType == pet.animalType){
                        pet.animalName = animal.animalName;
                        pet.prefab = animal.prefab;
                    }
                }

                pet.color = new AnimalColor();
                pet.color.color = int.Parse(line.Split(',')[3]);
                
                //Get pet color names
                foreach(AnimalColor colors in animalList.colors){
                    if((colors.color == pet.color.color) && (colors.animalID == pet.animalType)){
                        pet.prefab.GetComponent<PetInformation>().body.material = colors.animalMaterial;
                        pet.color.colorName = colors.colorName;
                        pet.color.animalID = colors.animalID;
                        pet.color.animalMaterial = colors.animalMaterial;
                        break;
                    }
                }

                pet.glow = int.Parse(line.Split(',')[4]);
                pet.special = int.Parse(line.Split(',')[5]);
                
                pet.face = new AnimalFace();
                pet.face.faceID = int.Parse(line.Split(',')[6]);


                foreach(AnimalFace animalFace in animalList.faces){
                    if(animalFace.faceID == pet.face.faceID){
                        pet.face.eligibleAnimals = animalFace.eligibleAnimals;
                        pet.face.faceMaterial = animalFace.faceMaterial;
                        break;
                    }
                }

                StartCoroutine(RetrievePetAccessoriesEnum(passedPlayer.userID, pet));

                passedPlayer.pets.Add(pet);
                if(int.Parse(line.Split(',')[8]) > 0){
                    passedPlayer.activePet = passedPlayer.pets[passedPlayer.pets.IndexOf(pet)];
                }

                

                
            }
        }
    }

    //Create a new user ID based on last user ID submitted to database. Submit information to playfab
    IEnumerator CheckUser(){
        UnityWebRequest wwwUserCheck;
        WWWForm form = new WWWForm();

        using(wwwUserCheck = UnityWebRequest.Post(url + "CheckUser.php", form)){
            yield return wwwUserCheck.SendWebRequest();
            if(wwwUserCheck.isNetworkError){
                Debug.Log(wwwUserCheck.error);
                ShowError(wwwUserCheck.error);
            }
            else{
                if(!(String.IsNullOrEmpty(wwwUserCheck.downloadHandler.text) || String.IsNullOrWhiteSpace(wwwUserCheck.downloadHandler.text)))
                    player.userID = double.Parse(wwwUserCheck.downloadHandler.text) + 1;
                else
                    player.userID = 0;
                

                PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest() {
                    Data = new Dictionary<string, string>() {
                        {"user_id", player.userID.ToString()},
                        {"logged_in", bool.FalseString},
                        {"active", "1"}
                    }
                    
                },
                result => Debug.Log("Successfully updated user data"),
                error => {
                    ShowError("Error creating account.");
                    Debug.Log("Error creating account.");
                    Debug.Log(error.GenerateErrorReport());
                });
                

                wwwUserCheck.Dispose();
                StopCoroutine(CheckUser());
        
                
            }
        }
    }

    //Method for showing pop-up errors to the user
    public void ShowError(string errorMessage, string errorTitle = "Error"){
        GameObject errorObject = GameObject.Instantiate(messages.ErrorMessage);
        messages.ErrorMessage.SetActive(true);
        messages.errorTitle.text = errorTitle;
        messages.errorText.text = errorMessage;
    }


    public void CheckConnection(LogIn login){
        StartCoroutine(test(login));
    }
    
    //Check connection to SQL database
    IEnumerator test(LogIn login){
        WWWForm form = new WWWForm();
        using(UnityWebRequest www = UnityWebRequest.Post(url + "TestConnection.php", form)){
            yield return www.SendWebRequest();
            if(www.isNetworkError){
               // Debug.Log(www.error);
               // ShowError(www.error);
            }
            else{
                Debug.Log(www.downloadHandler.text);
                if(www.downloadHandler.text.Contains("Warning")){
                    Debug.Log(www.downloadHandler.text);
                    ShowError("Could not connect to server. Please try again later.");
                    login.mainMenu.SetActive(true);
                }
                else
                    login.SignIn();
            }
        }
    }
}
