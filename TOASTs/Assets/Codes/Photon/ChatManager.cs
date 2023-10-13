using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ChatManager : MonoBehaviour
{

    // chat
    public GameObject chatPanel;
    public GameObject scroll_view;
    public GameObject m_Content;
    public TMP_InputField m_inputField;
    PhotonView photonview;
    GameObject m_ContentText;
    ScrollRect scroll_rect;
    static bool chk = false;

    public static ChatManager instance;


    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // chat
        m_ContentText = m_Content.transform.GetChild(0).gameObject;
        photonview = GetComponent<PhotonView>();
        scroll_rect = GameObject.FindObjectOfType<ScrollRect>();
        //Debug.Log(">>> chatManager에서 start() 불림");
        //Debug.Log(">>> Connected? : " + PhotonNetwork.IsConnected);
        photonview.RPC("RPC_Chat", RpcTarget.All, ">"+ PhotonNetwork.LocalPlayer.NickName+"님이 방에 입장했습니다. ");

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(">>> Input.GetKeyDown(KeyCode.Return)" + Input.GetKey(KeyCode.Return));

        /*Debug.Log(">>> chk : " + chk);
        Debug.Log(">>> Input.GetKeyDown(KeyCode.Return) : " + Input.GetKeyDown(KeyCode.Return));
        Debug.Log(">>> m_inputField.isFocused : " + m_inputField.isFocused);*/
        if (!chk && Input.GetKeyDown(KeyCode.Return) && !m_inputField.isFocused)
        {
            //Debug.Log(">>> 채팅창 찾아가기");
            m_inputField.ActivateInputField();
        }

        if (!m_inputField.isFocused && scroll_view.GetComponent<RectTransform>().rect.height != 80)
        {
            ShrinkChat();
        }

        if (m_inputField.isFocused && scroll_view.GetComponent<RectTransform>().rect.height != 330)
        {
            ExpandChat();
        }
    }

    public void OnEndEditEvent()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //Debug.Log(">>> 메시지 보내기");
            chk = true;
            string strMessage = PhotonNetwork.NickName + " : " + m_inputField.text;

            photonview.RPC("RPC_Chat", RpcTarget.All, strMessage);
            m_inputField.text = "";
        }
    }
    /*public void ChangeChkToFalse()
    {
        chk = false;
    }*/

    void AddChatMessage(string message)
    {
        GameObject goText = Instantiate(m_ContentText, m_Content.transform);

        goText.GetComponent<TextMeshProUGUI>().text = message; // 
        m_Content.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        scroll_rect.verticalNormalizedPosition = 0.0f;
    }

    public void ScrollDown()
    {
        scroll_rect.verticalNormalizedPosition = 0.0f;
        
    }

    public void ShrinkChat()
    {
        //Debug.Log(">>> 채팅창 줄이기");
        scroll_view.GetComponent<RectTransform>().sizeDelta = new Vector2(280, 80);
        /*Color color = scroll_view.GetComponent<UnityEngine.UI.Image>().color;
        color.a = 0.02f;
        scroll_view.GetComponent<UnityEngine.UI.Image>().color = color;*/
        chatPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 150);

        chk = false;
        Invoke("ScrollDown", 0.02f);
    }

    public void ExpandChat()
    {
        //Debug.Log(">>> 채팅창 늘리기 ");
        scroll_view.GetComponent<RectTransform>().sizeDelta = new Vector2(280, 330);
        chatPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 400);
        Invoke("ScrollDown", 0.02f);
    }

    [PunRPC]
    void RPC_Chat(string message)
    {
        Debug.Log(">>> RPC_Chat 불림");
        AddChatMessage(message);
    }
    
}
