using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLifeUI : MonoBehaviour
{
    [SerializeField] List<Transform> lifeImages;

    [Tooltip("player에서 가져오는 변수")]
    [SerializeField] int fullLifes = 10;

    int previousLifes;

    [Tooltip("player에서 life 만들면 삭제")]
    public int currentLifes;

    private void Start()
    {
        previousLifes = fullLifes;

        for(int i = 0; i < fullLifes; i++)
        {
            Transform child = transform.GetChild(i);
            lifeImages.Add(child);
            lifeImages[i].gameObject.SetActive(true);
        }
    }

    private void OnEnable()
    {
        // GameManager.instance.player.lifeUpdateUIEvent.AddListener(UpdateLifeImages)

    }

    private void OnDisable()
    {
        // GameManager.instance.player.lifeUpdateUIEvent.RemoveListener(UpdateLifeImages)

    }

    // TODO: Player에서
    // UnityEvent<int> lifeUpdateUIEvent 추가 필요
    // 이후 Level up하면 lifeUpdateUIEvent.Invoke(currentLife: int) 추가 필요
    void UpdateLifeImages(int currentLifes)
    {
        int diff = currentLifes - previousLifes;

        // SetActive(false)
        if(diff < 0)
        {
            for(int i = 0; i < Math.Abs(diff); i++)
            {
                lifeImages[previousLifes - 1 - i].gameObject.SetActive(false);
            }
        }

        // SetActive(true)
        else
        {
            for(int i = 0; i < Math.Abs(diff); i++)
            {
                lifeImages[previousLifes + i].gameObject.SetActive(true);
            }
        }

        previousLifes = currentLifes;
    }

    // TODO: Event 추가 후 삭제
    private void Update()
    {
        UpdateLifeImages(currentLifes);
    }
}
