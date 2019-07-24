using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;



public class MainManager : MonoBehaviour
{
    [SerializeField]
    Text mainText;

    [SerializeField]
    Text inputText;

    [SerializeField]
    Text nameText;

    string myMessage;
    string myName;

    DatabaseReference dataRef;

    struct Message 
    {
        public string name;
        public string text;

        public Message(string name, string text)
        {
            this.name = name;
            this.text = text;
        }
    }

void Start()
    {
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://testchat-b3f0a.firebaseio.com/");

        // Get the root reference location of the database.
        dataRef = FirebaseDatabase.DefaultInstance.GetReference("messages");
        dataRef.ValueChanged += HandleValueChanged;
    }



    public void SendMessage()
    {
        
        if (string.IsNullOrEmpty(myName))
        {
            myName = nameText.text;
            print("имя " +myName);
        }
               
        print("сообщение " + inputText.text);
        myMessage = inputText.text;

        if (!string.IsNullOrEmpty(myMessage))
        {
            print("сообщение "+myMessage);
            Message message = new Message(myName, myMessage);
            // string serialized = JsonUtility.ToJson(message, Form)
            //JsonConvert.SerializeObject(data, Formatting.Indented);
            string toSend = JsonUtility.ToJson(message);
            //string toSend = message.name + ": " + message.text;
            PostToDatabase(toSend);
        }

        inputText.text = "";
    }

    /*
     private void writeNewUser(string userId, string name, string email) {
    User user = new User(name, email);
    string json = JsonUtility.ToJson(user);

    mDatabaseRef.Child("users").Child(userId).SetRawJsonValueAsync(json);
}
     */

    void PostToDatabase(string message)
    {
        dataRef.Push().SetRawJsonValueAsync(message);
        print(message);
        // string json = JsonUtility.ToJson(message);
        //dataRef.Push().SetValueAsync(message);
        print("message was sent");
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
       
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Do something with the data in args.Snapshot
        print("вызвали");
        //string receive = args.Snapshot.GetRawJsonValue();
        string receive = args.Snapshot.Value.ToString();
        Message message = JsonUtility.FromJson<Message>(receive);
        print(message.name);
        print(receive);
        //args.Snapshot.GetValue(false).ToString();
        mainText.text += receive;
    }


}
