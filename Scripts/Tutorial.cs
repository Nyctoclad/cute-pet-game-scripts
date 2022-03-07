using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class Tutorial : MonoBehaviour
{
    public bool pause;
    public string[] tutorialWords;
    public Text words,namewords;
    public GameObject confirmMessage, namingMessage, chosenObject, mainMenu, tutorialObject, tutorialPetObject;
    GameObject pet;
    Player player;
    Database database;
    
    Vector3 petPosition = new Vector3(8.9691f,4.041695f,7.391274f), defaultRoomPosition = new Vector3(9.196397f, 5.672013f, 7.483993f);

    int i = 0;
    public int[] pauseNumbers;
    // Start is called before the first frame update
    void Start()
    {
        database = GameObject.FindGameObjectWithTag("database").GetComponent<Database>();
        player = database.player;
        if(!player.loggedIn){
            foreach(GameObject pet in GameObject.FindGameObjectsWithTag("pet")){
                pet.GetComponent<TempPickScripts>().onlyonce = true;
            }
            


            if(tutorialWords.Length > 0)
                words.text = tutorialWords[i];
            //Create a new room for this new player
            
        }
        else{
            mainMenu.SetActive(true);
            tutorialObject.SetActive(false);
            tutorialPetObject.SetActive(false);
            Tutorial tutorial = this;
            tutorial.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        NextLine();
    }

    //Decide on the kind of pet the player will have first
    public void SubmitAnimalChoice(){
        chosenObject.transform.position = petPosition;
        
        confirmMessage.SetActive(false);
        foreach(GameObject pet in GameObject.FindGameObjectsWithTag("pet")){
            if(pet != chosenObject)
                pet.SetActive(false);
        }
        
        namingMessage.SetActive(true);
    }

    //Submit the chosen animal name for the pet, submit user information to the database, instantiate the new room for the user
    public void SubmitAnimalName(){

        //Make sure the pet name adheres to the naming rules
        if(System.Text.RegularExpressions.Regex.IsMatch(namewords.text, @"^[a-zA-Z]+$")){
            chosenObject.GetComponent<TempPickScripts>().onlyonce = true;
            pause = false;
            if(tutorialWords.Length > 0 && i < tutorialWords.Length){
                i++;
                Debug.Log(tutorialWords[i]);
                words.text = tutorialWords[i];
                pause = true;
                StartCoroutine(Wait(.5f));
                
            }

            PetInformation petInfo = chosenObject.GetComponent<PetInformation>();
            database.SubmitNewUserInfo();
            database.AddNewPet(petInfo.animal,namewords.text);

            //If the player did part of the tutorial before enough to get a room
            if(player.rooms.Count > 0){
                //Instantiate room
                Room room = player.rooms[0];
                GameObject instantiatedRoom = GameObject.Instantiate(room.prefab, new Vector3(0,0,0), new Quaternion(0,0,0,0));
                instantiatedRoom.transform.localPosition = room.location;
                instantiatedRoom.transform.localRotation = room.rotation;
                instantiatedRoom.transform.Find("Floor").gameObject.GetComponent<MeshRenderer>().material = room.floor.material;
                instantiatedRoom.transform.Find("LeftWall").gameObject.GetComponent<MeshRenderer>().material = room.wall.material;
                instantiatedRoom.transform.Find("BackWall").gameObject.GetComponent<MeshRenderer>().material = room.wall.material;
                instantiatedRoom.transform.Find("RightWall").gameObject.GetComponent<MeshRenderer>().material = room.wall.material;
            }

            //If the player does not have a room registered
            if(player.rooms.Count < 1){
                SurfaceMaterials surfaceMaterials = database.surfaceMaterials;
                Room room = new Room();
                room.active = true;
                room.defaultRoom = true;
                room.roomID = 0;
                foreach(Mat mat in surfaceMaterials.materials){
                    if(mat.num == 0){
                        room.floor = surfaceMaterials.materials[0];
                        room.floorMaterialID = 0;
                    }
                    else if(mat.num == 1){
                        room.wallMaterialID = 1;
                        room.wall = surfaceMaterials.materials[1];
                    }
                }
                
                room.roomSize = 4;
                room.rotation = Quaternion.Euler(0,0,0);
                room.location = defaultRoomPosition;
                foreach(RoomSize roomSize in database.roomSizes){
                    if(roomSize.size == room.roomSize){
                        room.prefab = roomSize.prefab;
                    }
                }
                player.rooms.Add(room);
                

                //Instantiate room
                GameObject instantiatedRoom = GameObject.Instantiate(room.prefab, new Vector3(0,0,0), new Quaternion(0,0,0,0));
                instantiatedRoom.transform.localPosition = room.location;
                instantiatedRoom.transform.localRotation = room.rotation;

                instantiatedRoom.transform.Find("Floor").gameObject.GetComponent<MeshRenderer>().material = room.floor.material;
                instantiatedRoom.transform.Find("LeftWall").gameObject.GetComponent<MeshRenderer>().material = room.wall.material;
                instantiatedRoom.transform.Find("BackWall").gameObject.GetComponent<MeshRenderer>().material = room.wall.material;
                instantiatedRoom.transform.Find("RightWall").gameObject.GetComponent<MeshRenderer>().material = room.wall.material;
                
                String locationString = "" + room.location.x + ":" + room.location.y + ":" + room.location.z;
                String rotationString = "" + room.rotation.eulerAngles.x + ":" + room.rotation.eulerAngles.y + ":" + room.rotation.eulerAngles.z;

                database.SubmitRooms(player.userID, room.roomSize, Convert.ToInt32(room.defaultRoom), room.floorMaterialID, room.wallMaterialID, locationString, rotationString, room.roomID);

            }
            
            //Submit new user information to playfab
            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest() {
                Data = new Dictionary<string, string>() {
                    {"logged_in", bool.TrueString},
                    {"user_id", player.userID.ToString()},
                    {"active", bool.TrueString},
                }
            },
            result => Debug.Log("Successfully updated user data"),
            error => {
                Debug.Log(error.GenerateErrorReport());
            });
            namingMessage.SetActive(false);
        }
        else{
            database.messages.ErrorMessage.SetActive(true);
            database.messages.errorText.text = "Pet names must only use letters.";
        }
        
    }

    //Cancel and choose a different animal
    public void Exit(){
        chosenObject.GetComponent<TempPickScripts>().onlyonce = false;
    }

    //Cycle to the next words of the tutorial
    public void NextLine(){
        if(((Input.touchCount > 0) || Input.GetMouseButton(0)) && !pause){
            if(tutorialWords.Length > 0 && i < tutorialWords.Length){
                i++;
                Debug.Log(tutorialWords[i]);
                words.text = tutorialWords[i];
                pause = true;
                StartCoroutine(Wait(.5f));
                
            }
            else if(i > 0){
                mainMenu.SetActive(true);
                tutorialObject.SetActive(false);
                tutorialPetObject.SetActive(false);
                pet = database.InstantiatePet(petPosition, player.pets.IndexOf(player.activePet));
                GameObject.FindGameObjectWithTag("land_information").GetComponent<MainLandController>().currentPet = pet;
                Tutorial tutorial = this;
                tutorial.enabled = false;
            }

        }
    }

    IEnumerator Wait(float seconds){
        yield return new WaitForSeconds(seconds);
        foreach(int num in pauseNumbers){
            if(i == num){
                if(i == 2)
                    foreach(GameObject pet in GameObject.FindGameObjectsWithTag("pet")){
                        pet.GetComponent<TempPickScripts>().onlyonce = false;
                    }
                break;
            }
            else pause = false;
        }
    }
}
