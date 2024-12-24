using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SnakeController : MonoBehaviour
{
    [SerializeField]public float moveSpeed;  // Tốc độ di chuyển của rắn
    [SerializeField]public float steerSpeed; // Tốc độ quay hướng của rắn
    [SerializeField]public int   Gap;        // Khoảng cách giữa các phần thân rắn

    public int Score;

    public TMP_Text scoreText; // Dùng TextMeshPro

    public GameObject bodyPrefab;            // Prefab cho các phần thân rắn

    List<GameObject>BodyParts       = new List<GameObject>(); // Danh sách các phần thân rắn
    List<Vector3>   PositionHistory = new List<Vector3>();    // Lịch sử vị trí của đầu rắn

    void Update()
    {
        // Code di chuyển rắn
            // Di chuyển đầu rắn về phía trước theo hướng nó đang quay
            transform.position += transform.forward * moveSpeed * Time.deltaTime;

            // Nhận input từ phím mũi tên trái/phải để xoay đầu rắn
            float steerDirection = Input.GetAxis("Horizontal");
            transform.Rotate(Vector3.up * steerDirection * steerSpeed * Time.deltaTime);

            // Lưu vị trí hiện tại của đầu rắn vào danh sách PositionHistory
            PositionHistory.Insert(0, transform.position);

        // Code cập nhật thân rắn
            // Cập nhật vị trí cho từng phần thân rắn dựa trên lịch sử vị trí
            int index = 0;
            foreach (var body in BodyParts)
            {
                // Tìm điểm tiếp theo mà phần thân cần bám theo
                Vector3 point = PositionHistory[Mathf.Clamp(index * Gap, 0, PositionHistory.Count - 1)];

                // Tính hướng di chuyển và cập nhật vị trí
                Vector3 moveDirection = point - body.transform.position;
                body.transform.position += moveDirection * moveSpeed * Time.deltaTime;

                // Xoay phần thân để luôn hướng về điểm tiếp theo
                body.transform.LookAt(point);

                index++;
        }
            scoreText.text = Score.ToString();
    }

    void GrowUpSnake()
    {
        // Nếu rắn đã có thân, đặt phần thân mới ở vị trí của phần thân cuối cùng
        Vector3 spawnPosition = BodyParts.Count > 0
            ? BodyParts[BodyParts.Count - 1].transform.position
            : transform.position - transform.forward * Gap; // Nếu chưa có thân, đặt ở vị trí cách đầu rắn một khoảng Gap

        // Tạo phần thân mới từ prefab
        GameObject body = Instantiate(bodyPrefab, spawnPosition, Quaternion.identity);

        // Xoay phần thân mới theo hướng phần thân cuối cùng hoặc đầu rắn
        if (BodyParts.Count > 0)
        {
            body.transform.rotation = BodyParts[BodyParts.Count - 1].transform.rotation;
        }
        else
        {
            body.transform.rotation = transform.rotation;
        }

        // Thêm phần thân vào danh sách BodyParts
        BodyParts.Add(body);
        AudioManager.Instance.PlaySFX("Eat");
    }


    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra nếu rắn va chạm với "food"
        if (other.gameObject.CompareTag("food"))
        {
            Destroy(other.gameObject); // Phá hủy food
            Score++;                   // Cộng điểm
            GrowUpSnake();             // Tăng kích thước rắn
        }
    }

}
