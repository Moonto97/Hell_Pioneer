using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    private float _speed = 3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector2 direction = new Vector2(h, v);
        direction.Normalize();

        Vector3 movement = new Vector3(h, v, 0f);
        transform.Translate(movement * _speed * Time.deltaTime);
    }
}
