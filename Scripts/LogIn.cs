using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;


//Log In/Sign Up script. Handles account creation and logging in.
public class LogIn : MonoBehaviour
{
    public Player player;
    public GameObject displayNameContainer, errorContainer, confirmContainer, mainMenu;
    public InputField dnInput;
    public Text errorText;
    Database database;
    public TemporaryAccount tempAccount;

    // Start is called before the first frame update
    void Start()
    {

        database = GameObject.FindGameObjectWithTag("database").GetComponent<Database>();

        player = database.player;

        //Create a random username and password for user, store in playerprefs
        if(String.IsNullOrEmpty(PlayerPrefs.GetString("username"))){
            System.Random random = new System.Random();
            String availchars = "abcdefghijklmnopqrstuvwxyz0123456789";
            int length = 14;
            String username = "";
            String password = "";
            for(int i = 0; i < length; i++){
                username += (availchars[random.Next(0,availchars.Length)]);
            }
            for(int i = 0; i < length; i++){
                password += (availchars[random.Next(0,availchars.Length)]);
            }
            PlayerPrefs.SetString("username", username);
            PlayerPrefs.SetString("password", password);
            tempAccount = new TemporaryAccount();
            tempAccount.tempUsername = username;
            tempAccount.tempPassword = password;
        }
        else{
            tempAccount = ScriptableObject.CreateInstance("TemporaryAccount") as TemporaryAccount;
            tempAccount.tempUsername = PlayerPrefs.GetString("username");
            tempAccount.tempPassword = PlayerPrefs.GetString("password");
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Check to make sure we can connect to the SQL database
    public void TestDatabaseConnection(){
        database.CheckConnection(this);
    }

    //After checking the database, redirect to this sign in
    public void SignIn(){
        
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId)){
            PlayFabSettings.TitleId = "DD12B"; 
        }
        
        //Try to register player to see if player is new
        var request = new RegisterPlayFabUserRequest{Username = tempAccount.tempUsername, Password = tempAccount.tempPassword, RequireBothUsernameAndEmail = false};
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
        
    }

    
    //Register player
    private void OnRegisterSuccess(RegisterPlayFabUserResult result){
        Debug.Log("Account successfully created.");
        database.CreateUser();
        displayNameContainer.SetActive(true);
        player.displayName = "";
        player.loggedIn = false;
        player.activePet = new Animal();
        player.pets = new List<Animal>();
        player.friends = new List<Player>();
        //player.items = new List<Item>();
    }

    //Try to login user on failure to register
    private void OnRegisterFailure(PlayFabError error){

        var request = new LoginWithPlayFabRequest{TitleId = PlayFabSettings.TitleId, Username = tempAccount.tempUsername, Password = tempAccount.tempPassword};
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFailure);
    }

    //Successfully logged in user
    private void OnLoginSuccess(LoginResult loginResult)
    {
        string ID = loginResult.PlayFabId;

        PlayFabClientAPI.GetUserData(new GetUserDataRequest() {
            PlayFabId = ID,
            Keys = null
        }, result => {
            if (result.Data == null || !result.Data.ContainsKey("user_id")) {
                //Error occurring when the user created their username and password, but user id was not recorded.
                errorText.text = "Account does not exist.";
                errorContainer.SetActive(true);
            }
            else {
                if(!bool.Parse(result.Data["active"].Value)){
                    //Error occurring when user's account has been deactivated
                    errorText.text = "Account not active. Please contact support.";
                    errorContainer.SetActive(true);
                }
                else{
                    //Get the user id and whether they have completed the tutorial and logged in before
                    player.userID = double.Parse(result.Data["user_id"].Value);
                    player.loggedIn = bool.Parse(result.Data["logged_in"].Value);
                    
                    //Create new player information
                    if(!player.loggedIn){

                        displayNameContainer.SetActive(true);
                        player.displayName = "";
                        player.loggedIn = false;
                        player.activePet = new Animal();
                        player.pets = new List<Animal>();
                        player.friends = new List<Player>();
                        //player.items = new List<Item>();
                    }
                    else if(player.loggedIn){
                        
                        database.RetrieveUser();
                        database.RetrievePet();
                        database.RetriveRooms();
                        //database.RetriveFriends();
                        SceneManager.LoadSceneAsync("MainScene");
                    }
                }
            }
        }, (error) => {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });
        
    }

    private void OnLoginFailure(PlayFabError error)
    {
        errorText.text = "There was an error trying to login. Please try again later.";
        errorContainer.SetActive(true);
    }

    //Method to submit display name
    public void SubmitName(){
        if(!System.Text.RegularExpressions.Regex.IsMatch(dnInput.text, @"^[a-zA-Z]+$")){
            errorText.text = "Display names must use letters only.";
            errorContainer.SetActive(true);
        }
        else{
            confirmContainer.SetActive(true);
        }
    }

    //Method to confirm submission of display name and move to main scene
    public void ConfirmName(){
        player.displayName = dnInput.text;
        SceneManager.LoadSceneAsync("MainScene");
    }

}
