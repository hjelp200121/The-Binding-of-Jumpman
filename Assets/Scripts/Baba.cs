using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BabaState {
    Wander,
    Follow,
    Die,
};

public class Baba : MonoBehaviour
{
    GameObject player;
    public BabaState currentState = BabaState.Wander;

    public float range;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState) {
            case (BabaState.Wander):
                Wander();
                break;

            case (BabaState.Follow):
                Follow();
                break;

            case (BabaState.Die):
                //Die();
                break;

        }
    }

    private bool IsPlayerInRange(float range) {
        return Vector2.Distance(transform.position, player.transform.position) <= range;
    }

    void Wander() {

    }

    void Follow() {

    }

}
