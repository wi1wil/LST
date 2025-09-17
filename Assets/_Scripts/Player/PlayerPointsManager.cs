using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPointsManager : MonoBehaviour
{
    public Transform[] playerPoints;

    private static PlayerPointsManager instance;
    public static PlayerPointsManager Instance => instance;

    void Awake()
    {
        instance = this;
    }

    public Transform GetRandomPlayerPoint()
    {
        int randomIndex = Random.Range(0, playerPoints.Length);
        return playerPoints[randomIndex];
    }
}
