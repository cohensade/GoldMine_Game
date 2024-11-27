using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton לגישה גלובלית לניקוד
    public int score = 0; // משתנה לניקוד

    void Awake()
    {
        // יצירת Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // שמירה על האובייקט בין סצנות
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int points)
    {
        score += points;
        Debug.Log("Score: " + score); // מדפיס את הניקוד למסוף (Console)
    }
}
