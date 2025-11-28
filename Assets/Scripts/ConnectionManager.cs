using System;
using TMPro;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConnectionManager : MonoBehaviour
{
        [SerializeField] private TMP_InputField _addressField;
        [SerializeField] private TMP_InputField _portField;
        [SerializeField] private TMP_Dropdown _connectionModeDropdown;
        [SerializeField] private Button _connectButton;
        [SerializeField] private TMP_Text _buttonText;

        private ushort Port => ushort.Parse(_portField.text);
        private string Address => _addressField.text;
        
        private void OnEnable()
        {
                _connectionModeDropdown.onValueChanged.AddListener(OnConnectionModeChange);
                _connectButton.onClick.AddListener(OnConnectionClick);
        }

        private void OnConnectionModeChange(int connectionMode)
        {
                string buttonLabel;
                _connectButton.enabled = true;

                switch (connectionMode)
                {
                      case  0:
                              buttonLabel = "Start Host";
                              break;
                      case 1:
                              buttonLabel = "Start Server";
                              break;
                      case 2:
                              buttonLabel = "Start Client";
                              break;
                      default:
                              buttonLabel = "ERROR";
                              _connectButton.enabled = false;
                              break;
                }
                
                _buttonText.text = buttonLabel;
        }

        private void OnConnectionClick()
        {
                DestroyLocalSimulationWorld();
                SceneManager.LoadScene(1);
                
                switch (_connectionModeDropdown.value)
                {
                        case 0:
                                StartServer();
                                StartClient();
                                break;
                        case 1:
                                StartServer();
                                break;
                        case 2:
                                StartClient();
                                break;
                        default:
                                Debug.LogError($"[{nameof(ConnectionManager)}]: Unknown connection mode: {_connectionModeDropdown.value}");
                                break;
                }
        }

        private void DestroyLocalSimulationWorld()
        {
                foreach (var world in World.All)
                {
                        if(world.Flags != WorldFlags.Game)
                                continue;
                        
                        world.Dispose();
                        break;
                }
        }

        private void StartServer()
        {
                var serverWorld = ClientServerBootstrap.CreateServerWorld("ServerWorld");
                var serverEndpoint = NetworkEndpoint.AnyIpv4.WithPort(Port);

                using var networkDriverQuery = serverWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
                networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(serverEndpoint);
                
        }
        
        private void StartClient()
        {
                var clientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");
                var connectionEndpoint = NetworkEndpoint.Parse(Address, Port);
                using var networkDriverQuery = clientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>()); 
                networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(clientWorld.EntityManager, connectionEndpoint);

                World.DefaultGameObjectInjectionWorld = clientWorld;
        }

        private void OnDisable()
        {
                _connectionModeDropdown.onValueChanged.RemoveAllListeners();
                _connectButton.onClick.RemoveAllListeners();
        }
}