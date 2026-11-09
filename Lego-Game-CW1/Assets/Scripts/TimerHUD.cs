using TMPro;
using UnityEngine;

public class TimerHUD : MonoBehaviour
{
    public TMP_Text timerText;
    public float timeLeft = 60f;

    void Update()
    {
        if (timeLeft > 0f)
            timeLeft -= Time.deltaTime;

        if (timeLeft < 0f)
            timeLeft = 0f;

        int seconds = Mathf.CeilToInt(timeLeft);
        timerText.text = (seconds / 60).ToString("00") + ":" + (seconds % 60).ToString("00");
    }
}
