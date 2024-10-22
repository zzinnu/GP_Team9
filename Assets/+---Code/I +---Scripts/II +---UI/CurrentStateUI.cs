using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CurrentStateUI : MonoBehaviour
{
    public UnityEvent<bool> onStatChanged;

    [SerializeField] TMP_Text changesText;
    [SerializeField] TMP_Text currentStateText;

    int previousValue;
    int currentValue;

    // TODO: Ability 찍는거 제한 및 Save기능
    private void Awake()
    {
        if(changesText == null)
            changesText = transform.Find("Changes").GetComponent<TMP_Text>();

        if(currentStateText == null)
            currentStateText = GetComponent<TMP_Text>();

        if(!int.TryParse(currentStateText.text, out currentValue))
        {
            Debug.LogError("Failed Get currenValue to int");
        }
        
        previousValue = currentValue;
        changesText.enabled = false;
    }

    private void OnEnable()
    {
        onStatChanged.AddListener(StatChange);
    }

    private void OnDisable()
    {
        onStatChanged.RemoveListener(StatChange);
    }

    void StatChange(bool selectedButton)
    {
        currentValue = selectedButton ? (currentValue + 1) : (currentValue - 1);

        // 기존보다 낮게 되면 값 원래대로 복구하고 return
        if(currentValue <= previousValue)
        {
            currentValue = previousValue;
            changesText.enabled = false;
        }

        int diff = currentValue - previousValue;

        // Changes text 바꾸기
        if(!changesText.enabled && diff > 0)
            changesText.enabled = true;

        changesText.text = "(+" + diff.ToString() + ")";

        // Current text 바꾸기
        currentStateText.text = currentValue.ToString();
    }
}
