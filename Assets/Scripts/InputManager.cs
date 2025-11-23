using GodinhoNelsonBennett.Lab3;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private InputSystem_Actions inputScheme;

    [SerializeField] private MovementControl movementController;
    [SerializeField] private Respawner playerRespawner;
    [SerializeField] private CameraRotater cameraRotater;
    private RespawnHandler respawnHandler;


    void Awake()
    {
        inputScheme = new InputSystem_Actions();
        inputScheme.CameraRotation.Enable();
        cameraRotater.Initialize(inputScheme.CameraRotation.Aim);
        movementController.Initialize(inputScheme.Player.Move, inputScheme.Player.SpeedUp);
    }

    void Start()
    {
        this.respawnHandler = new RespawnHandler(inputScheme.Player.Reset, playerRespawner);
    }


}
