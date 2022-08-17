using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class EngGameUi : MonoBehaviour
{
    private Dictionary<uint,int> _playerScores;

    private void Start()
    {
        _playerScores = new Dictionary<uint, int>();
        StartCoroutine(CheckForNewPlayers());
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void DictionaryAddPlayerScoreEventHandler(uint playerId,int playerScore)
    {
        if(!_playerScores.ContainsKey(playerId))
        {
            _playerScores.Add(playerId, playerScore);
            return; 
        }
        
        _playerScores[playerId] = playerScore;
    }

    private IEnumerator CheckForNewPlayers()//temporary solution
    {
        while (true)
        {
            foreach (PlayerAttack player in FindObjectsOfType<PlayerAttack>())
            {
                player.UpdatePlayerScoreEvent += DictionaryAddPlayerScoreEventHandler;
            }
            yield return new WaitForSeconds(5);
        }
    }
}
