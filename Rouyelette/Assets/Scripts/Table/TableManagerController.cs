using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Xml;
using System.Runtime.InteropServices;

public class TableManagerController : MonoBehaviour
{
    [DllImport("__Internal")]
    public static extern void CopyPasteReader(string gObj, string vName);


    [Header("UI Settings:")]
    [SerializeField] TMP_InputField _tableIDInput;
    [SerializeField] GameObject _joinPanel;


    private void Start()
    {
        Actions.TableStatus += TableStatusAction;
    }

    /// <summary>
    /// CallBACK from the network by table socket
    /// </summary>
    /// <param name="message"></param>
    private void TableStatusAction(string message)
    {
        PopMessage.Instance.PopUpMessage(PopMessage.MessageType.normal, message);

        Invoke("LoadGame", 2.0f);
    }

    void LoadGame()
    {
        SceneController.Instance.LoadLoadingScene();
    }


    #region CREATE_TABLE

    /// <summary>
    /// Action implemented  when the user clicked on create button
    /// </summary>
    public void CreateAction()
    {
        var ticks = DateTime.Now.Ticks;
        var guid = Guid.NewGuid().ToString();
        var uniqueId = ticks.ToString();

        Debug.Log("New Table Created >>" + uniqueId);

        PopMessage.Instance.PopUpMessage(PopMessage.MessageType.normal,"Table Created :" + uniqueId);

        Network.Instance.PushTableId(uniqueId);

        Invoke("LoadGame", 2.0f);
    }
    #endregion

    #region JOIN_TABLE

    /// <summary>
    /// Enable the join panel
    /// </summary>
    /// <param name="enable"></param>
    public void EnableJoinPanel(bool enable)
    {
        _joinPanel.SetActive(enable);   
    }

    /// <summary>
    /// Action implemented when the user is clicked the join team
    /// </summary>
    public void JoinTeam()
    {
        if(_tableIDInput.text == "")
        {
            PopMessage.Instance.PopUpMessage(PopMessage.MessageType.normal, "Please enter the id ");
            return;
        }

        string id = _tableIDInput.text;

        Network.Instance.SearchID(id);
    }

    #endregion


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        {
            CopyPasteReader(gameObject.name, "Paste");
        }
    }

    private void Paste(string pasteValue)
    {
        Debug.Log(pasteValue + " pasteValue");
        // Assuming 'inputField' is your InputField component
        _tableIDInput.text = pasteValue;
    }
}
