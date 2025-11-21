using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    private float _speed = 3f;
    

    private Rigidbody2D _rb;
    private Vector2 _playerMove;
    private Animator _animator;


    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>(); 
      
    }

    public void Execute()
    {

    }
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        
        Vector2 moveInput = new Vector2(h, v);

        Vector2 direction = new Vector2(h, v);
        direction.Normalize();

        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W))
        {
            moveY = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveY = -1f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveX = 1f;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            moveX = -1f;
        }

        _animator.SetFloat("MoveX", moveX);
        _animator.SetFloat("MoveY", moveY);

        _playerMove = new Vector2(moveX, moveY);
        transform.Translate(_playerMove * _speed * Time.deltaTime);


         Vector3 movement = new Vector3(h, v, 0f);
         transform.Translate(movement * _speed * Time.deltaTime);

         MakePlayerDistanceField();
    }

    // TODO : 움직일때, 방해물 생성시에만 호출
    void MakePlayerDistanceField()
    {
        BFSManager.Instance.BuildDistanceField(transform.position);
    }

/*    private void FixedUpdate()
    {
        _rb.linearVelocity = new Vector2(0, v) * _speed * Time.deltaTime;
    }*/

}
