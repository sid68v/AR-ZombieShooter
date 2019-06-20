﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
   
    public float speed = 2f;
    public int damage = 10;
    //public float impactPower = 5f;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, 10f);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.transform.GetChild(0).gameObject.SetActive(true);
            //other.gameObject.GetComponent<Rigidbody>().AddForce(-other.transform.forward * impactPower, ForceMode.Impulse);
            other.gameObject.GetComponent<EnemyController>().GetDamage(damage);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}