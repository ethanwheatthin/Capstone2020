﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using MLAgents;

public class Game2Agent : Agent
{
    private RayPerception2D rayPerception;

    
    public Text scoreText;
    public int score = 0;
    public GameObject goal;

    public GameObject asteroidControllerObject;
    private AsteroidController asteroidController;
    public GameObject spawnPointGameObject;
    private PowerUpSpawnner spawnPointScript;

    public GameObject bulletPrefab;
    public Transform point;
    private Rigidbody2D rb;
    public float bulletSpeed = 20f;

    private Vector2 movement;
    private Vector2 mousePosition;
    public Camera cam;
    public static float speed = 5f;
    public static float turnSpeed = 10f;
    private float shootTime;

    public override void AgentAction(float [] vectorAction, string textAction)
    {
        if (vectorAction[3] == 1f && shootTime <= 0)
        {
            shootTime = 0.5f;
            Shoot();
        }
        float angle = transform.rotation.z;
        //WASD Movement
        if (vectorAction[0] == 1f)
        {
            //W
            movement.y = 1f;
        }
        else if (vectorAction[0] == 2f)
        {
            //S
            movement.y = -1f;
        }
        else
        {
            movement.y = 0f;
        }
        if (vectorAction[1] == 1f)
        {
            //A
            movement.x = -1f;
        }
        else if (vectorAction[1] == 2f)
        {
            //D
            movement.x = 1f;
        }
        else
        {
            movement.x = 0;
        }
        if(vectorAction[2] == 1f)
        {
            //Rotate CCW
            rb.rotation += 1 * turnSpeed;
        }
        else if(vectorAction[2] == 2f)
        {
            //Rotate CW
            rb.rotation += -1 * turnSpeed;
        }
        Mathf.Clamp(rb.rotation, 0, 360);
        rb.MovePosition(rb.position + movement * speed * Time.deltaTime);

        Vector3 pos = cam.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);
        transform.position = cam.ViewportToWorldPoint(pos);
    }

    public override void CollectObservations()
    {
        AddVectorObs(this.transform.position);
        AddVectorObs(rb.velocity);

        //Position of goal
        AddVectorObs(spawnPointScript.currentPosition);

        //float[] rayAngles = { 0f, 22.5f, 45f, 67.5f, 90f, 112.5f, 135f, 157.5f, 180f, 202.5f, 225f, 247.5f, 270f, 292.5f, 315f, 337.5f };
        float[] angles = { 0f, 45f, 90f, 135f, 180f, 225f, 270f, 315f };
        string[] tags = { "asteroid", "wall" };
        AddVectorObs(rayPerception.Perceive(7f, angles, tags));
    }

    public override void AgentReset()
    {
        asteroidController.ResetAsteroids();
        this.transform.position = transform.parent.transform.position;
        score = 0;
    }

    public override void InitializeAgent()
    {
        transform.position = transform.parent.transform.position;
        Debug.Log("Agent Position " + transform.position);
        spawnPointScript = spawnPointGameObject.GetComponent<PowerUpSpawnner>();
        rb = GetComponent<Rigidbody2D>();
        shootTime = 0f;
        rayPerception = GetComponent<RayPerception2D>();
        asteroidController = asteroidControllerObject.GetComponent<AsteroidController>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("goal"))
        {
            Debug.Log("Goal hit!");
            this.score++;
            AddReward(1f);
            spawnPointScript.DestoryPowerUp();
            //spawnPointScript.activePowerups--;
        }

        if (collider.gameObject.CompareTag("asteroid"))
        {
            Destroy(collider.gameObject);
            Done();
            Debug.Log("Asteroid hit!");
            this.score--;
            //Either kill player here or decrement his score
        }
    }

    public void FixedUpdate()
    {
        shootTime -= Time.fixedDeltaTime;
        Mathf.Clamp(shootTime, 0, 0.5f);

    }

    public void Shoot()
    {
        GameObject bulletTemp = Instantiate(bulletPrefab, point.position, point.rotation);
        Rigidbody2D rb = bulletTemp.GetComponent<Rigidbody2D>();
        rb.AddForce(point.up * bulletSpeed, ForceMode2D.Impulse);
    }
}