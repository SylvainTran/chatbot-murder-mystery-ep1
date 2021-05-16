using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageObject : MonoBehaviour
{
    private string sender;
    public string Sender { get { return sender; }}
    private string date;
    private string text;
    public string Text { get { return text; } set { text = value; }} 
}
