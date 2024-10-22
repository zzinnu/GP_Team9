using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelUI : MonoBehaviour
{
    [SerializeField] TMP_Text levelText;
    [SerializeField] Image filledImage;

    [Tooltip("player���� level ����� ����")]
    public int tempLevel = 0;
    public int tempTotalExp = 100;
    public int tempCurrentExp = 10;

    private void Start()
    {
        if(levelText == null)
            levelText = transform.Find("LevelText").GetComponent<TMP_Text>();

        if(filledImage == null)
            filledImage = transform.Find("LevelFilled").GetComponent<Image>();
    }

    
    private void OnEnable()
    {
        // GameManager.instance.player.levelUpUIEvent.AddListener(UpdateLevelTextUI)
        // GameManager.instance.player.expUpUIEvent.AddListener(UpdateExpUI)
    }

    private void OnDisable()
    {
        // GameManager.instance.player.levelUpUIEvent.RemoveListener(UpdateLevelTextUI)
        // GameManager.instance.player.expUpUIEvent.RemoveListener(UpdateExpUI)
    }

    // TODO: Player����
    // UnityEvent<int> levelUpUIEvent �߰� �ʿ�
    // ���� Level up�ϸ� levelUpUIEvent.Invoke(level: int) �߰� �ʿ�
    void UpdateLevelTextUI(int level)
    {
        levelText.text = level.ToString();
    }

    // TODO: Player����
    // UnityEvent<int, int> expUpUIEvent �߰� �ʿ�
    // ���� Level up�ϸ� expUpUIEvent.Invoke(totalExp: int, currentExp: int) �߰� �ʿ�
    void UpdateExpUI(int totalExp, int currentExp)
    {
        float amount = (float)currentExp / totalExp;
        filledImage.fillAmount = amount;
    }

    // TODO: Event �߰� �� ����
    private void Update()
    {
        UpdateLevelTextUI(tempLevel);
        UpdateExpUI(tempTotalExp, tempCurrentExp);
    }
}
