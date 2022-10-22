using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour
{
    public static PlayerScore S;

    [SerializeField]
    private TextMeshProUGUI scoreUI;
    private int score;
    public int Score
    {
        get { return score; }
        set 
        {
            score = value;
            scoreUI.text = score.ToString(); 
        }
    }
    [SerializeField]
    private TextMeshProUGUI recordUI;
    private int record;
    public int Record
    {
        get { return record; }
        set
        {
            record = value;
            recordUI.text = record.ToString();
        }
    }

    private void Awake()
    {
        S = this;
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey(Game.S.Map.MapSettings.ToString()))
            Record = PlayerPrefs.GetInt(Game.S.Map.MapSettings.ToString());
        else
            Record = 0;
    }

    public void UpdateRecord()
    {
        if (Record < Score)
        {
            Record = Score;
            PlayerPrefs.SetInt(Game.S.Map.MapSettings.ToString(), Record);
        }
    }
}
