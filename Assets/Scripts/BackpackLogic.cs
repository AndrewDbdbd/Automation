using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BackpackLogic : MonoBehaviour
{
    [Range(0f, 100f)]
    [SerializeField] private float capacity = 100f;
    [SerializeField] private float flowOutTime = 5f;
    [SerializeField] private TextMeshProUGUI textObj;
    [Range(0f, 100f)]
    [SerializeField] private float nowCapacity = 100f;

    void Update()
    {
        textObj.text = ((int)capacity).ToString();
        if (capacity != nowCapacity)
        {
            capacity = Mathf.MoveTowards(capacity, nowCapacity, flowOutTime * Time.deltaTime);
        }
    }

    
    public void Decrease(float value) 
    {
        nowCapacity -= value;
    }

}
