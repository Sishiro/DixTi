using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class ConnectionSystem : Singleton<ConnectionSystem>
{
    public async Task<bool> CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(7);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

            RoomManager.Instance.startGameButton.interactable = true;

            RoomManager.Instance.joinCodeTextUI.text = "Código: " + joinCode;

            return true;
        }
        catch (RelayServiceException ex)
        {
            Debug.LogException(ex);
        }
        return false;
    }

    public async Task<bool> JoinRelay()
    {
        string joinCode = JoinManager.Instance.joinCodeInputField.text;
        Debug.Log(joinCode);
        if (joinCode.Length > 0)
        {
            try
            {
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

                RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                NetworkManager.Singleton.StartClient();

                return true;
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
        return false;
    }

    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
    }
}
