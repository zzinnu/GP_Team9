using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLifeUI : MonoBehaviour
{
    [SerializeField] List<Transform> lifeImages;

    [Tooltip("player���� �������� ����")]
    [SerializeField] int fullLifes = 10;

    int previousLifes;

    [Tooltip("player���� life ����� ����")]
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

    // TODO: Player����
    // UnityEvent<int> lifeUpdateUIEvent �߰� �ʿ�
    // ���� Level up�ϸ� lifeUpdateUIEvent.Invoke(currentLife: int) �߰� �ʿ�
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

    // TODO: Event �߰� �� ����
    private void Update()
    {
        UpdateLifeImages(currentLifes);
    }
}
