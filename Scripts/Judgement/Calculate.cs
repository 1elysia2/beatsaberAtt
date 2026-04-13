using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calculate : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    [SerializeField] private Text comboText;
    [SerializeField] private Text rateText;

    public static Text Score { get; private set; }
    public static Text Combo { get; private set; }
    public static int score;
    public static int combo;

    public static Text Rate { get; private set; }

    public static int maxCombo;

    void Start()
    {
        Score = scoreText;
        Combo = comboText;
        Rate = rateText;
        score = 0;
        combo = 0;
    }

    public static void OnHit(Judgment judgment)
    {

        switch (judgment)
        {
            case Judgment.Perfect:
                Debug.Log("Perfect!");
                Rate.text = "PERFECT!";
                ClickManager.Instance.PlayHitSound();
                score += 3;
                combo += 1;
                break;
            case Judgment.HoldPerfect:
                Debug.Log("Perfect!");
                Rate.text = "PERFECT!";
                ClickManager.Instance.PlayHitSound();
                score += 3;
                combo += 1;
                break;
            case Judgment.Good:
                Debug.Log("Good!");
                Rate.text = "GOOD!";
                ClickManager.Instance.PlayHitSound();
                score += 1;
                combo += 1;


                break;
            case Judgment.Bad:
                Debug.Log("Bad!");
                Rate.text = "BAD";
                combo = 0;
                break;
            case Judgment.HoldBad:
                Debug.Log("Bad!");
                Rate.text = "BAD";
                combo = 0;
                break;
            case Judgment.ErrorCube:
                Rate.text = "Error";
                combo = 0;
                break;
        }
        string scoreNum;
        int scoreTemp = score;

        if (combo > maxCombo) maxCombo = combo;


        if (scoreTemp / 10000 != 0)
        {//��λ
            scoreNum = score.ToString();
        }
        else if (scoreTemp / 1000 != 0)
        {
            scoreNum = "0" + score.ToString();
        }
        else if (scoreTemp / 100 != 0)
        {
            scoreNum = "00" + score.ToString();
        }
        else if (scoreTemp / 10 != 0)
        {
            scoreNum = "000" + score.ToString();
        }
        else
        {
            scoreNum = "0000" + score.ToString();
        }

        Score.text = "SCORE:" + scoreNum;
        Combo.text = "Combo x" + combo.ToString();
        //Destroy(gameObject);
    }

    public static void notHit()
    {
        Debug.Log("Bad!");
        Rate.text = "BAD";
        combo = 0;
        string scoreNum;
        int scoreTemp = score;


        if (scoreTemp / 10000 != 0)
        {//��λ
            scoreNum = score.ToString();
        }
        else if (scoreTemp / 1000 != 0)
        {
            scoreNum = "0" + score.ToString();
        }
        else if (scoreTemp / 100 != 0)
        {
            scoreNum = "00" + score.ToString();
        }
        else if (scoreTemp / 10 != 0)
        {
            scoreNum = "000" + score.ToString();
        }
        else
        {
            scoreNum = "0000" + score.ToString();
        }

        Score.text = "SCORE:" + scoreNum;
        Combo.text = "Combo x" + combo.ToString();
        //Destroy(gameObject);
    }
}