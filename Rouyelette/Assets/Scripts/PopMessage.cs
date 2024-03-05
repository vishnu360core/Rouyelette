using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopMessage : MonoBehaviour
{
    // Start is called before the first frame update

    static PopMessage instance;

    public static PopMessage Instance { get { return instance; } }

    public enum MessageType {normal,win,lost}


    [Header("Panel")]
    [SerializeField] GameObject panel;

   [Header("MessageText:")]
   [SerializeField] TMP_Text messageText;

    [Header("Delay:")]
    [Range(0, 10f)]
    [SerializeField] float _delay;


    private void Awake()
    {
        if (instance == null)
            instance = this;


        panel.SetActive(false);
    }


    /// <summary>
    /// Action for showing the message
    /// </summary>
    /// <param name="type"></param>
    /// <param name="message"></param>
    public void PopUpMessage(MessageType type, string message)
    {
        StartCoroutine(PopMessageAction(type, message));
    }

    IEnumerator PopMessageAction(MessageType type, string message)
    {
        panel.SetActive(true);

        messageText.text = message;

        switch (type)
        {
            case MessageType.normal:
                messageText.color = Color.white;
                break;

            case MessageType.win:
                messageText.color = Color.green;
                break;

            case MessageType.lost:
                messageText.color = Color.red;
                break;
        }

        yield return new  WaitForSeconds(_delay);
        panel.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
