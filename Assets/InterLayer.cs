using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterLayer : MonoBehaviour
{
    [SerializeField]
    FirebaseChat firebaseChat;

    [SerializeField]
    Text mainText;

    [SerializeField]
    InputField inputText;

    [SerializeField]
    Text nameText;

    string myMessage;
    string myName;
    string room = "roomID";

    private void Start()
    {
        //firebaseChat.ConnectToDB(null);
        firebaseChat.ConnectToDB(room);
    }

    public void PrepareMessage()
    {
        if (string.IsNullOrEmpty(myName)) ChangeMyName();
        myMessage = inputText.text;
        if (!string.IsNullOrEmpty(myMessage)) firebaseChat.SendMessage(myMessage, myName);
        inputText.text = "";
    }

    public void WriteToChat(string name, string message)
    {
        mainText.text += name + ": " + message + "\n\n";
    }

    public void ChangeMyName()
    {
        myName = nameText.text;        
    }

    public void Exit()
    {
        Application.Quit();
    }

}
