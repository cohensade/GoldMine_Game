using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton ����� ������� ������
    public int score = 0; // ����� ������

    void Awake()
    {
        // ����� Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ����� �� �������� ��� �����
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int points)
    {
        score += points;
        Debug.Log("Score: " + score); // ����� �� ������ ����� (Console)
    }
}
