using UnityEngine;

public class HookController : MonoBehaviour
{
    public float rotationSpeed = 50f; // ������ ������
    public float moveSpeed = 5f; // ������ ������ ����/�����
    public float maxDistance = 5f; // ����� �������� ������ ����
    private Vector3 initialPosition; // ����� ������ �� ���
    private Quaternion initialRotation; // ������ �������� �� ���
    private bool isSwinging = true; // ��� ��� ������
    private bool isMovingDown = false; // ��� ��� ������ ����
    private bool isMovingUp = false; // ��� ��� ������ �����
    private GameObject carriedItem = null; // �������� �����
    private LineRenderer lineRenderer; // ���� �����
    private int direction = 1; // ����� ������ (1 = �����, -1 = �����)

    void Start()
    {
        // ����� ����� ������ ��������
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // ���� LineRenderer
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2; // ��� ������ ����
    }

    void Update()
    {
        if (isSwinging)
        {
            SwingHook(); // ����� ������
        }
        else if (isMovingDown)
        {
            MoveDown(); // ����� ����
        }
        else if (isMovingUp)
        {
            MoveUp(); // ����� ���� �����
        }

        // ����� ���� ���� �� ��� Space
        if (Input.GetKeyDown(KeyCode.Space) && isSwinging)
        {
            LaunchHook();
        }

        // ����� ����
        UpdateLineRenderer();
    }

    void SwingHook()
    {
        // ����� ��� ���� ����� (��� Y ����)
        transform.RotateAround(transform.parent.position, Vector3.forward, rotationSpeed * direction * Time.deltaTime);

        // ����� ����� ������ ���� ���� ������� �������� ���������
        float angle = transform.localEulerAngles.z;
        if (angle > 180f) angle -= 360f; // ����� ����� ��� 180
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
        // ����� ����
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime, Space.Self);

        // �� ��� ���� ����� ��������
        if (Vector3.Distance(transform.position, initialPosition) > maxDistance)
        {
            isMovingDown = false;
            isMovingUp = true; // ����� ����� �����
        }
    }

    void MoveUp()
    {
        // ����� ���� �����
        transform.position = Vector3.MoveTowards(transform.position, initialPosition, moveSpeed * Time.deltaTime);

        // �� ��� ��� ������ ������
        if (Vector3.Distance(transform.position, initialPosition) < 0.1f)
        {
            if (carriedItem != null)
            {
                // ����� �� �������� �����
                carriedItem.transform.SetParent(null);
                Destroy(carriedItem); // ���� �� ��������
                carriedItem = null;
            }

            // ����� �� ��� ������ ��������
            transform.rotation = initialRotation;
            isMovingUp = false;
            isSwinging = true; // ���� ������ ������
        }
    }

    public void LaunchHook()
    {
        isSwinging = false; // ���� �� �������
        isMovingDown = true; // ����� ����� ����
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isMovingDown) return; // ����� ������� �� �� ��� ������ ����

        if (collision.CompareTag("Item")) // �� ���� �������� �� ���� Item
        {
            carriedItem = collision.gameObject;
            carriedItem.transform.SetParent(transform); // ����� �� �������� ���
            carriedItem.GetComponent<Collider2D>().enabled = false; // ����� �� �-Collider �� ��������

            // ����� �����
            Item item = carriedItem.GetComponent<Item>();
            if (item != null)
            {
                GameManager.Instance.AddScore(item.scoreValue);
            }

            isMovingDown = false; // ���� �� ������
            isMovingUp = true; // ����� ����� �����
        }
    }


    void UpdateLineRenderer()
    {
        // ����� ����� ����� ���� �� ����
        lineRenderer.SetPosition(0, transform.parent.position); // ����� ������ - �����
        lineRenderer.SetPosition(1, transform.position); // ����� ���� - ���
    }
}
