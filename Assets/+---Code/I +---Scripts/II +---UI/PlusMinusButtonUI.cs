using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlusMinusButtonUI : MonoBehaviour
{
    [SerializeField] Button minus;
    [SerializeField] Button plus;

    public CurrentStateUI currentState;

    private void Awake()
    {
        if(minus == null)
            minus = transform.Find("Minus").GetComponent<Button>();

        if(plus == null)
            plus = transform.Find("Plus").GetComponent<Button>();

        if(currentState == null)
            currentState = transform.parent.Find("CurrentState").gameObject.GetComponent<CurrentStateUI>();

        minus.onClick.AddListener(Minus);
        plus.onClick.AddListener(Plus);
    }

    private void OnDisable()
    {
        minus.onClick.RemoveListener(Minus);
        plus.onClick.RemoveListener(Plus);
    }

    void Minus()
    {
        currentState.onStatChanged.Invoke(false);
    }

    void Plus()
    {
        currentState.onStatChanged.Invoke(true);
    }
}
