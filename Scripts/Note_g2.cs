using UnityEngine;
using UnityEngine.UI;
public class Note_g2 : Note
{
    public float spawnTime;
    public void Initialize(string direction, float moveSpeed, int dir)
    {
        noteDirection = direction;
        if(noteDirection ==  "Down")
        {
            noteDirection= "Up";
        }
        if (movement == null)
        {
            movement = GetComponent<NoteMovement>();
        }
        movement.speed = moveSpeed;
        UpdateVisual();
        (movement as NoteMovement_g2).dir = dir;
        spawnTime = Time.time;

    }



}