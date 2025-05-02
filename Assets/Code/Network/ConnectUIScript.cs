using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class ConnectUIScript : MonoBehaviour
{
    //[SerializeField] private Button hostButton;
    //[SerializeField] private Button clientButton;
    //private void Start()
    //{
    //    hostButton.onClick.AddListener(HostButtonOnClick);
    //    clientButton.onClick.AddListener(ClientButtonOnClick);
    //}
    public void HostButtonOnClick()
    {
        NetworkManager.Singleton.StartHost();
    }
    public void ClientButtonOnClick()
    {
        NetworkManager.Singleton.StartClient();
    }
}