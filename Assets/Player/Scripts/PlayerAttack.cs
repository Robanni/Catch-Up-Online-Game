using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class PlayerAttack : NetworkBehaviour
{
    [SerializeField] private float _attackDistance = 1f;
    [SerializeField] private float _attackRadius = 1.5f;
    [SerializeField] private float _damagePuase = 3f;
    [SerializeField] private Material _playerMaterial;

    private Rigidbody _playerRigidbody;
    private Camera _playerCamera;

    private int _localPlayerScore = 0;

    public delegate void UpdateDelegate(uint playerId,int localPlayerScore);
    public event UpdateDelegate UpdatePlayerScoreEvent;

    private bool _wasHit = false;

    private uint _playerID;


    public override void OnStartLocalPlayer()
    {

        _playerID = GetComponent<NetworkIdentity>().netId;

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
            transform.position += attackDiraction* _attackDistance;

            Collider[] hitedPlayers = Physics.OverlapSphere(transform.position, _attackRadius);

            foreach (Collider player in hitedPlayers)
            {
                if (gameObject.GetComponent<Collider>() == player)continue; 
                if (player.gameObject.CompareTag("Player") )
                {
                    player.GetComponent<PlayerAttack>().CommandTakeHitSync();
                    _localPlayerScore++;
                    UpdatePlayerScoreEvent?.Invoke(_playerID,_localPlayerScore);
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
