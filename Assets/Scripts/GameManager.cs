using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Syn.Bot.Oscova;
using Syn.Bot.Oscova.Attributes;
using TMPro;

public class Message
{
    public string Text;
    public TMP_Text TextObject;
    public MessageType MessageType;
}

public enum MessageType
{
    User, Bot
}
public class GameManager : MonoBehaviour
{
    OscovaBot MainBot;
    List<Message> Messages = new List<Message>();

    public GameObject chatPanel, textObject;
    public GameObject wetsonCanvasGO;
    private Canvas wetsonCanvas;
    public TMP_InputField chatBox;
    public Color UserColor, BotColor;

    private void Awake()
    {
        wetsonCanvas = wetsonCanvasGO.GetComponent<Canvas>();
    }

    public void ToggleChat()
    {
        wetsonCanvas.enabled = !wetsonCanvas.enabled;
    }

    public void AddMessage(string messageText, MessageType messageType)
    {
        if(Messages.Count >= 25)
        {
            Destroy(Messages[0].TextObject.gameObject);
            Messages.Remove(Messages[0]);
        }

        var newMessage = new Message { Text = messageText };

        var newText = Instantiate(textObject, chatPanel.transform);

        newMessage.TextObject = newText.GetComponent<TMP_Text>();
        newMessage.TextObject.text = messageText;
        newMessage.TextObject.color = messageType == MessageType.User ? UserColor : BotColor;

        Messages.Add(newMessage);
    }

    public void SendMessageToBot()
    {
        var userMessage = chatBox.text;

        if(!string.IsNullOrEmpty(userMessage))
        {
            Debug.Log($"OscovaBot: [USER] {userMessage}");
            AddMessage($"Detective K: {userMessage}", MessageType.User);
            var request = MainBot.MainUser.CreateRequest(userMessage);
            var evaluationResult = MainBot.Evaluate(request);
            evaluationResult.Invoke();

            chatBox.Select();
            chatBox.text = "";
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            SendMessageToBot();
        }
    }

    private void Start()
    {
        try
        {
            MainBot = new OscovaBot();
            OscovaBot.Logger.LogReceived += (s, o) =>
            {
                Debug.Log($"OscovaBot: {o.Log}");
            };

            MainBot.Dialogs.Add(new BotDialog());
            MainBot.Trainer.StartTraining();

            MainBot.MainUser.ResponseReceived += (sender, evt) =>
            {
                AddMessage($"Bot: {evt.Response.Text}", MessageType.Bot);
            };
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }
}

public class BotDialog : Dialog
{
    [Expression("Hello Bot")]
    public void Hello(Context context, Result result)
    {
        result.SendResponse("Well hello, Detective K!");
    }
}
