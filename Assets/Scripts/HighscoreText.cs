using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class HighscoreText : MonoBehaviour
{


    Text highscore;

    void OnEnable() // text needs to be refreshed evertime the start page is activated
    {
        highscore = GetComponent<Text>();
        highscore.text = "High Score:" + PlayerPrefs.GetInt("HighScore").ToString(); // To String (returns integer) can't implicitly convert int to string

    }

}
