using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Bolt.EntityBehaviour<INPCSTATE> {
    public GameObject Player;
    //static Animator anims;

	void Start () {
        //anims = GetComponent<Animator>();
	}

    void FixedUpdate () {
        Player = GameObject.FindWithTag("Player");
        Vector3 direction = Player.transform.position - this.transform.position;
        float angle = Vector3.Angle(direction, this.transform.forward);
        if(Vector3.Distance(Player.transform.position, this.transform.position) < 10 && angle < 30)
        {
            direction.y = 0;

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), 0.1f);

            if (direction.magnitude > 5)
            {
                this.transform.Translate(0, 0, 0.5f);
            }

        }
	}  
}
