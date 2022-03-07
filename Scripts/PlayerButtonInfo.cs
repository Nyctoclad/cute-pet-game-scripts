using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

//Information about individual friends stored on the button of that friend
public class PlayerButtonInfo : MonoBehaviour
{
    public Text username, petName, petType, petColor;
    public Image userIcon;
    public GameObject online, offline, confirmBox, infoBox, friendsPetInfo;
    
    public Player friend;
    public string playerName;
    Database database;
    List<string[]> changeList = new List<string[]>();
    Transform original;
    // Start is called before the first frame update
    void Start()
    {
        database = GameObject.FindGameObjectWithTag("database").GetComponent<Database>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //Show if they are online or offline
    public void Online(){
        online.SetActive(true);
        offline.SetActive(false);
    }

    public void Offline(){
        offline.SetActive(true);
        offline.SetActive(false);
    }

    public void Info(){
        string playerType = this.gameObject.tag;
        
        if(playerType == "fpin"){
            if(confirmBox != null){
                ConfirmBoxInfo cbi = confirmBox.GetComponent<ConfirmBoxInfo>();
                cbi.title.text = "Add Friend";
                cbi.descriptionText.text = "Add " + playerName + " as a friend?";
                cbi.confirm.onClick.AddListener(()=> AddFriend());
                confirmBox.SetActive(true);
            }
           
        }
        else if(playerType == "fpout"){
            infoBox.SetActive(true);

        }
        else if(playerType == "friends"){
            
        }
        else if(playerType == "bout"){
            if(confirmBox != null){
                ConfirmBoxInfo cbi = confirmBox.GetComponent<ConfirmBoxInfo>();
                cbi.title.text = "Unblock";
                cbi.descriptionText.text = "Unblock " + playerName + "?";
                cbi.confirm.onClick.AddListener(()=> Unblock());
            }
        }
    }

    public void ShowFriendsPetInfo(){
        GameObject petholder = new GameObject();

        petName.text = friend.activePet.petName;
        petType.text = friend.activePet.animalName;
        petColor.text = friend.activePet.color.colorName;

        foreach(Transform child in friendsPetInfo.transform){
            if(child.gameObject.tag == "petholder"){
                petholder = child.gameObject;
                break;
            }
        }

        GameObject pet = GameObject.Instantiate(friend.activePet.prefab,new Vector3(0,0,0), friend.activePet.prefab.transform.rotation, petholder.transform);
        SkinnedMeshRenderer body = pet.GetComponent<PetInformation>().body, face = pet.GetComponent<PetInformation>().face;
        pet.transform.localPosition = new Vector3(0,0,0);
        pet.transform.localScale = new Vector3(730.4f, 730.4f, 2f);
        body.material = friend.activePet.color.animalMaterial;
        face.material = friend.activePet.face.faceMaterial;
        GameObject.FindGameObjectWithTag("land_information").GetComponent<MainLandController>().SendBackward();
        friendsPetInfo.SetActive(true);
    }

    //Change user to friend and reload the friend's list
    public void AddFriend(){
        database.UpdateFriends(friend.userID, 0, 0, 1, 0, 0);
        MainLandController mlc = GameObject.FindGameObjectWithTag("land_information").GetComponent<MainLandController>();
        mlc.pbuserID = friend.userID;
        mlc.CreateFriendsList();
        confirmBox.SetActive(false);
    }

    //Unblock someone and reload the friends list
    public void Unblock(){
        database.UpdateFriends(friend.userID, 0,0,0,0,0);
        MainLandController mlc = GameObject.FindGameObjectWithTag("land_information").GetComponent<MainLandController>();
        mlc.pbuserID = friend.userID;
        mlc.CreateFriendsList();
        confirmBox.SetActive(false);
    }
}
