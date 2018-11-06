using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//[RequireComponent(typeof(Text))]
public class CountdownText : MonoBehaviour
{

    public delegate void CountdownFinished(); // Create an event for the game manager to listen to
    public static event CountdownFinished OnCountdownFinished; // hard to manage events when static, but easier (if just public have to have reference to game object)

    Text countdown;

    private void OnEnable()
    {
        countdown = GetComponent<Text>();
        countdown.text = "3";
        StartCoroutine("Countdown");
    }

    IEnumerator Countdown() // can wait a couple seconds or 1 second
    {
        int count = 3;
        for (int i = 0; i < count; i++)
        {
            countdown.text = (count - i).ToString();
            yield return new WaitForSeconds(1);
        }

        OnCountdownFinished();
    }
}
