using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public float bubbleSpeed;
    
    Vector3 targetPos;
    public GameObject ways;
    public Transform[] wayPoints;
    int pointDex;
    int pointCount;
    int direction =1;
    void Awake(){
        wayPoints = new Transform[ways.transform.childCount];
        for(int i =0; i< ways.gameObject.transform.childCount;i++){
            wayPoints[i] = ways.transform.GetChild(i).gameObject.transform;
        }

    }

    private void Start(){
        pointCount = wayPoints.Length;
        pointDex =1;
        targetPos = wayPoints[pointDex].transform.position;
    }

    private void Update(){
        var step = bubbleSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
        if(transform.position == targetPos){
            
            NextPoint();
        }
    }

    public void NextPoint(){
       
        if (pointDex == pointCount - 1){
            direction = -1;
        }

        if (pointDex == 0){
            direction =1;
        }

        pointDex += direction;
        
        targetPos = wayPoints[pointDex].transform.position;

    }
}
