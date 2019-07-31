using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;



public class FirebaseChat : MonoBehaviour
{
    [SerializeField]
    InterLayer interLayer;

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
    
    public void ConnectToDB(string room)
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://testchat-b3f0a.firebaseio.com/");
        if (string.IsNullOrEmpty(room)) room = "global";
        dataRef = FirebaseDatabase.DefaultInstance.GetReference(room);
        dataRef.ValueChanged += HandleValueChanged;
    }
    
    public void SendMessage(string myMessage, string myName)
    {
        Message message = new Message(myName, myMessage);
        string toSend = JsonUtility.ToJson(message);
        dataRef.Push().SetRawJsonValueAsync(toSend);
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
                    Message message = JsonUtility.FromJson<Message>("{" + rezult[i] + "}");
                    interLayer.WriteToChat(message.name, message.text);
                }
            }
            firstRequest = false;
        }

        else
        {
            Message message = JsonUtility.FromJson<Message>("{" + rezult[rezult.Length - 3] + "}");
            interLayer.WriteToChat(message.name, message.text);      //т.к. минимальная длина одного сообщения 4
        }
    }

    int GetStartIndex(int length, int countOfMessage)
    {
        if (length - 3 > countOfMessage * 2) return length - 1 - (countOfMessage * 2);
        else return 2;
    }


}
