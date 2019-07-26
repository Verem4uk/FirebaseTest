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
    InputField inputText;

    [SerializeField]
    Text nameText;

    string myMessage;
    string myName;

    DatabaseReference dataRef;
    bool firstRequest = true;

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
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://testchat-b3f0a.firebaseio.com/");
        dataRef = FirebaseDatabase.DefaultInstance.GetReference("messages");
        dataRef.ValueChanged += HandleValueChanged;
    }

    public void ChangeMyName()
    {
        myName = nameText.text;
        print("имя изменено на " + myName);
    }

    public void SendMessage()
    {
        if (string.IsNullOrEmpty(myName)) ChangeMyName();
        myMessage = inputText.text;
        if (!string.IsNullOrEmpty(myMessage))
        {            
            Message message = new Message(myName, myMessage);
            string toSend = JsonUtility.ToJson(message);
            PostToDatabase(toSend);
        }

        inputText.text = "";
    }

    void PostToDatabase(string message)
    {
        dataRef.Push().SetRawJsonValueAsync(message);       
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {       
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
       
        string receive = args.Snapshot.GetRawJsonValue();   
        if(!string.IsNullOrEmpty(receive)) PrepairMessages(receive);        
    }

    void PrepairMessages (string Json)
    {
        string[] rezult = Json.Split('{', '}');
        foreach(string str in rezult)
        {
            print(str);
        }
        
        if (firstRequest)
        {
            
            for (int i = GetStartIndex(rezult.Length, 6); i < rezult.Length; i += 2)
            {
                if (!string.IsNullOrEmpty(rezult[i]))
                {
                    WriteMessage(JsonUtility.FromJson<Message>("{" + rezult[i] + "}"));
                }
            }

            firstRequest = false;
        }               
        
        else WriteMessage(JsonUtility.FromJson<Message>("{" + rezult[rezult.Length - 3] + "}"));      //т.к. минимальная длина одного сообщения 4

    }

    int GetStartIndex(int length, int countOfMessage)
    {
        if (length - 3 > countOfMessage * 2) return length - 1 - (countOfMessage * 2);
        else return 2;
    }

    void WriteMessage(Message message)
    {
        mainText.text += message.name + ": " + message.text + "\n\n";
    }

    public void Exit()
    {
        Application.Quit();
    }
}
