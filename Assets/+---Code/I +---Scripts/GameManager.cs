using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject player;  // TODO : �ӽ÷� GameObject ���ٰ�, Script�� ����

    public int availablePoints;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("GameManager alread exists");
    }
}
