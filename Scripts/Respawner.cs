using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour
{

    Character[] players;
    int size;
    int[] timers;
    [SerializeField] int respawnTimer = 240;
    // Start is called before the first frame update
    void Start()
    {
        players = FindObjectsOfType<Character>();
        size = players.Length;
        timers = new int[size];
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < size; i++)
        {
            if (!players[i].isActiveAndEnabled)
            {
                if (timers[i] == 0)
                    timers[i] = respawnTimer;
                timers[i] -= 1;
                if (timers[i] == 0)
                    players[i].gameObject.SetActive(true);
            }
        }
    }

    public Character[] GetCharacters()
    {
        return players;
    }

}
