using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using static UnityEngine.GraphicsBuffer;

public class PlayerAttack : NetworkBehaviour
{
    [SerializeField] private float _attackImpuls = 1f;
    [SerializeField] private float _damagePuase = 3f;
    [SerializeField] private Material _playerMaterial;

    private Rigidbody _playerRigidbody;
    private Camera _playerCamera;
    

    private bool _wasHit = false;


    public override void OnStartLocalPlayer()
    {
        _playerRigidbody = GetComponent<Rigidbody>();
        _playerCamera = GetComponentInChildren<Camera>();
        _playerMaterial = new Material(_playerMaterial);    
        GetComponent<Renderer>().material = _playerMaterial;
    }

    private void Update()
    {
        Attack();
    }

    private void Attack()
    {
        if(!_playerCamera) _playerCamera = GetComponentInChildren<Camera>();
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 attackDiraction = _playerCamera.transform.forward;
            _playerRigidbody.AddForce(attackDiraction * _attackImpuls, ForceMode.Impulse);

            Collider[] hitedPlayers = Physics.OverlapSphere(transform.position, 0.6f);

            foreach (Collider player in hitedPlayers)
            {
                if (gameObject.GetComponent<Collider>() == player)continue; 
                if (player.gameObject.CompareTag("Player") )
                {
                    player.GetComponent<PlayerAttack>().CommandTakeHitSync();
                }
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void CommandTakeHitSync()
    {
        TakeHit();
    }

    [ClientRpc]
    private void TakeHit()
    {
        if (_wasHit) return;
        _wasHit = true;
        StartCoroutine(ChangeColorTimer(_damagePuase));
    }
    
    private IEnumerator ChangeColorTimer(float waitTime)
    {
        _playerMaterial.color = Color.red;
        yield return new WaitForSeconds(waitTime);
        _playerMaterial.color = Color.white;
        _wasHit = false;
    }
}
