using UnityEngine;
using UnityEngine.Events;

public class NoteMovement_g2 : NoteMovement
{
    public UnityEvent EnterTop,ExitTop;
    public float startSpeedY = 5;
    private float speedY;
    public int dir = 1;
    protected override void Start()
    {
        base.Start();

        speedY = startSpeedY;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Ground"))
        {

            if (!note.isError)
            {
                Calculate.notHit();
                AttentionEventHub.Instance.OnNoteMiss();
            }
            Destroy(gameObject);

        }

        if(other.transform.CompareTag("Top"))
        {
            EnterTop?.Invoke();
            
        }



    }

    void OnTriggerExit(Collider other)
    {
           if(other.transform.CompareTag("Top"))
        {
            ExitTop?.Invoke();
            
        }
    }
    protected override void Update()
    {
        if (!PicoUIController.paused)
        {
            speedY -= 5 * Time.deltaTime; 
            speedY= Mathf.Max(speedY,-10);
            transform.position += speedY * Vector3.up * Time.deltaTime;
            transform.position += speed * dir * Vector3.right * Time.deltaTime;



        }
        if (PicoUIController.Gameover)
        {
            Destroy(gameObject);
        }

    }
}

