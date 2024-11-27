using UnityEngine;

public class HookController : MonoBehaviour
{
    public float rotationSpeed = 50f; // מהירות הסיבוב
    public float moveSpeed = 5f; // מהירות התנועה למטה/למעלה
    public float maxDistance = 5f; // המרחק המקסימלי לתנועה למטה
    private Vector3 initialPosition; // מיקום התחלתי של הוו
    private Quaternion initialRotation; // הזווית ההתחלתית של הוו
    private bool isSwinging = true; // האם הוו מתנדנד
    private bool isMovingDown = false; // האם הוו בתנועה למטה
    private bool isMovingUp = false; // האם הוו בתנועה למעלה
    private GameObject carriedItem = null; // האובייקט שנאסף
    private LineRenderer lineRenderer; // החוט המחבר
    private int direction = 1; // כיוון הסיבוב (1 = ימינה, -1 = שמאלה)

    void Start()
    {
        // שמירת מיקום וזווית התחלתיים
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // קבלת LineRenderer
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2; // שני נקודות לחוט
    }

    void Update()
    {
        if (isSwinging)
        {
            SwingHook(); // תנועת מטוטלת
        }
        else if (isMovingDown)
        {
            MoveDown(); // תנועה למטה
        }
        else if (isMovingUp)
        {
            MoveUp(); // תנועה חזרה למעלה
        }

        // שליחת הקרס למטה עם מקש Space
        if (Input.GetKeyDown(KeyCode.Space) && isSwinging)
        {
            LaunchHook();
        }

        // עדכון החוט
        UpdateLineRenderer();
    }

    void SwingHook()
    {
        // סיבוב הוו סביב השחקן (ציר Y למטה)
        transform.RotateAround(transform.parent.position, Vector3.forward, rotationSpeed * direction * Time.deltaTime);

        // החלפת כיוון הסיבוב כאשר מגיע לזוויות המקסימום והמינימום
        float angle = transform.localEulerAngles.z;
        if (angle > 180f) angle -= 360f; // תיקון זווית מעל 180
        if (angle >= 70f && direction == 1)
        {
            direction = -1;
        }
        else if (angle <= -70f && direction == -1)
        {
            direction = 1;
        }
    }

    void MoveDown()
    {
        // תנועה למטה
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime, Space.Self);

        // אם הוו הגיע למרחק המקסימלי
        if (Vector3.Distance(transform.position, initialPosition) > maxDistance)
        {
            isMovingDown = false;
            isMovingUp = true; // מתחיל לחזור למעלה
        }
    }

    void MoveUp()
    {
        // תנועה חזרה למעלה
        transform.position = Vector3.MoveTowards(transform.position, initialPosition, moveSpeed * Time.deltaTime);

        // אם הוו חזר לנקודת ההתחלה
        if (Vector3.Distance(transform.position, initialPosition) < 0.1f)
        {
            if (carriedItem != null)
            {
                // משחרר את האובייקט שנאסף
                carriedItem.transform.SetParent(null);
                Destroy(carriedItem); // הורס את האובייקט
                carriedItem = null;
            }

            // מחזיר את הוו לזווית ההתחלתית
            transform.rotation = initialRotation;
            isMovingUp = false;
            isSwinging = true; // חוזר לתנועת מטוטלת
        }
    }

    public void LaunchHook()
    {
        isSwinging = false; // עוצר את המטוטלת
        isMovingDown = true; // מתחיל תנועה למטה
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isMovingDown) return; // זיהוי התנגשות רק אם הוו בתנועה למטה

        if (collision.CompareTag("Item")) // אם פוגע באובייקט עם תגית Item
        {
            carriedItem = collision.gameObject;
            carriedItem.transform.SetParent(transform); // מצמיד את האובייקט לוו
            carriedItem.GetComponent<Collider2D>().enabled = false; // מנטרל את ה-Collider של האובייקט

            // הוספת ניקוד
            Item item = carriedItem.GetComponent<Item>();
            if (item != null)
            {
                GameManager.Instance.AddScore(item.scoreValue);
            }

            isMovingDown = false; // עוצר את הירידה
            isMovingUp = true; // מתחיל תנועה למעלה
        }
    }


    void UpdateLineRenderer()
    {
        // עדכון נקודת התחלה וסוף של החוט
        lineRenderer.SetPosition(0, transform.parent.position); // נקודת ההתחלה - השחקן
        lineRenderer.SetPosition(1, transform.position); // נקודת הסוף - הוו
    }
}
