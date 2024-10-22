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

    [Tooltip("player에서 level 만들면 삭제")]
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

    // TODO: Player에서
    // UnityEvent<int> levelUpUIEvent 추가 필요
    // 이후 Level up하면 levelUpUIEvent.Invoke(level: int) 추가 필요
    void UpdateLevelTextUI(int level)
    {
        levelText.text = level.ToString();
    }

    // TODO: Player에서
    // UnityEvent<int, int> expUpUIEvent 추가 필요
    // 이후 Level up하면 expUpUIEvent.Invoke(totalExp: int, currentExp: int) 추가 필요
    void UpdateExpUI(int totalExp, int currentExp)
    {
        float amount = (float)currentExp / totalExp;
        filledImage.fillAmount = amount;
    }

    // TODO: Event 추가 후 삭제
    private void Update()
    {
        UpdateLevelTextUI(tempLevel);
        UpdateExpUI(tempTotalExp, tempCurrentExp);
    }
}
