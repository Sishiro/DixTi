using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class ConectionSystem : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    private string joinCode = "";
    [SerializeField] private TextMeshProUGUI codeText;

    [SerializeField] private MenuSystem menuSystem;

    public PlayerData playerData;
    [SerializeField] private TMP_InputField playerNameIF;

    public void EnterGame()
    {
        if (playerNameIF.text.Length > 0)
        {
            playerData.name = playerNameIF.text;
            menuSystem.ChangeScene(1);
        }
        else
        {
            // Show error UI
        }
    }

    public async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(7);

            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

            codeText.text += joinCode;
        }
        catch (RelayServiceException ex)
        {
            Debug.LogException(ex);
        }
    }

    public async void JoinRelay()
    {
        joinCode = inputField.text;
        Debug.Log(joinCode);
        if (joinCode.Length > 0)
        {
            try
            {
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

                RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                NetworkManager.Singleton.StartClient();

                menuSystem.ChangeScene(2);
            }
            catch (RelayServiceException ex)
            {
                Debug.LogException(ex);
            }
        }
        else
        {
            // Show error UI
        }
    }

    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
    }
}
