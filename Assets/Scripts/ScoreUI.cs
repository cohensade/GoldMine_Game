using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public TMP_Text scoreText; // ����� ����� �� TextMeshPro

    void Update()
    {
        scoreText.text = "Score: " + GameManager.Instance.score;
    }
}
