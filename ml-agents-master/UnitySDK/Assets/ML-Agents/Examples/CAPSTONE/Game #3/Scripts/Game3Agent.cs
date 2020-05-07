﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
public class Game3Agent : Agent
{
    private Vector2 movement;
    private Rigidbody2D rb;
    public float speed = 50f;
    public static float turnSpeed = 5f;

    public Camera cam;
    public RayPerception2D rayPerception;
    public Transform gameArea;
    public GameObject gameAreaObject;
    private PowerUpSpawnGameThree powerUpController;
    private TargetSpawnner targetController;
    public GameControllerGameThree gameController;

    public Shooting shooting;
    public AgentFOV agentFOV;

    private float shootTime;

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        if (shooting.ammoCount == 0)
        {
            AgentReset();
        }
        if (vectorAction[1] == 1f && shootTime <= 0)
        {
            shootTime = 0.5f;
            GetComponent<Shooting>().Shoot();
        }
        float angle = transform.rotation.z;
        //WASD Movement
        //if (vectorAction[0] == 1f)
        //{
        //    //W
        //    movement.y = 1f;
        //}
        //else if (vectorAction[0] == 2f)
        //{
        //    //S
        //    movement.y = -1f;
        //}
        //else
        //{
        //    movement.y = 0f;
        //}
        //if (vectorAction[1] == 1f)
        //{
        //    //A
        //    movement.x = -1f;
        //}
        //else if (vectorAction[1] == 2f)
        //{
        //    //D
        //    movement.x = 1f;
        //}
        //else
        //{
        //    movement.x = 0;
        //}
        if (vectorAction[0] == 1f)
        {
            //Rotate CCW
            rb.rotation += 1 * turnSpeed;
        }
        else if (vectorAction[0] == 2f)
        {
            //Rotate CW
            rb.rotation += -1 * turnSpeed;
        }
        Mathf.Clamp(rb.rotation, 0, 360);
        //rb.AddForce(movement * speed + - new Vector2(Mathf.Log(rb.velocity.x),Mathf.Log(rb.velocity.y)));
        //Vector3 pos = cam.WorldToViewportPoint(transform.position);
        //pos.x = Mathf.Clamp01(pos.x);
        //pos.y = Mathf.Clamp01(pos.y);
        //transform.position = cam.ViewportToWorldPoint(pos);
    }
    public override void InitializeAgent()
    {
        transform.position = transform.parent.transform.position;
        rb = GetComponent<Rigidbody2D>();
        rayPerception = GetComponent<RayPerception2D>();

        gameArea = transform.parent;
        gameAreaObject = gameArea.gameObject;
        powerUpController = gameArea.GetComponent<GameControllerGameThree>().powerUpContainer.GetComponent<PowerUpSpawnGameThree>();
        targetController = gameArea.GetComponent<GameControllerGameThree>().targetContainer.GetComponent<TargetSpawnner>();

        shooting = GetComponent<Shooting>();
        agentFOV = GetComponent<AgentFOV>();
        gameController = this.transform.parent.gameObject.GetComponent<GameControllerGameThree>();


        shootTime = 0f;
    }

    public override void CollectObservations()
    {
        //(transform.pos.x - gameArea.pos.x) / sizeOfGameAreaX;
        //(transform.pos.y - gameArea.pos.y) / sizeOfGameAreaY;
        //(transform.eulerAngles.z / 360f);
        //(rb.velocity) / maxVelocity;
        //AddVectorObs((transform.position.x - gameArea.position.x)/ 8.55f);
        //AddVectorObs((transform.position.y - gameArea.position.y)/ 4.81f);
        AddVectorObs(transform.eulerAngles.z / 360f);

        //powerupsCount / maxPowerUps;
        //targetCount / maxTargets;
        //ammoLeft / maxAmmo;
        AddVectorObs(targetController.activeTargets / targetController.maxNumOfTargetsToSpawn);
        //AddVectorObs((shooting.ammoCount) / (shooting.ammoCount + (powerUpController.numOfPowerUpsToSpawn * 3)));
        for(int i = 0; i < agentFOV.targetTransforms.Length; i++)
        {
            Vector3 target = agentFOV.targetTransforms[i];
            AddVectorObs((target.x - gameArea.position.x) / 8.55f);
            AddVectorObs((target.y - gameArea.position.y) / 4.81f);
        }
 
        
        //foreach (Transform powerup in agentFOV.visiblePowerups)
        //{
        //    AddVectorObs((powerup.position.x - gameArea.position.x) / 8.55f);
        //    AddVectorObs((powerup.position.y - gameArea.position.y) / 4.81f);
        //}
        
        //rayCasts at -45, 0, and 45 degree positions, relative to front of player, for wall detection
        //float distance = 20f;
        //float[] angles = { 0, 45, 315 };
        //string[] detectableObjects = {"wall"};
        //AddVectorObs(rayPerception.Perceive(distance, angles, detectableObjects));
    }

    public void FixedUpdate()
    {
        shootTime -= Time.fixedDeltaTime;
        Mathf.Clamp(shootTime, 0, 0.5f);
    }

    public void Reward(float reward)
    {
        Debug.Log("Adding reward " + reward);
        AddReward(1f);
    }
    public override void AgentReset()
    {
        base.AgentReset();
        gameController.ResetGameArea();
    }
}
