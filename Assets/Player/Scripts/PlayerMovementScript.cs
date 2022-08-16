using Mirror;
using UnityEngine;


public class PlayerMovementScript : NetworkBehaviour
{
    [SerializeField] private float _playerMovementSpeed = 50f;
    [SerializeField] private float _playerRotationSpeed = 3f;

    private float _rotationX = 0;

    public override void OnStartLocalPlayer()
    {
        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = new Vector3(0, 1, -3.75f);
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) { return; }
        PlayerMovement();
        PlayerRotation();

    }

    private void PlayerMovement()
    {
        float moveX = Input.GetAxis("Horizontal") * Time.deltaTime * _playerMovementSpeed;
        float moveZ = Input.GetAxis("Vertical") * Time.deltaTime * _playerMovementSpeed;

        transform.Translate(moveX, 0, moveZ);
    }
    private void PlayerRotation() {
        
        _rotationX += Input.GetAxis("Mouse X") * _playerRotationSpeed;

        

        transform.rotation = Quaternion.Euler(0, _rotationX, 0);
    }

}
