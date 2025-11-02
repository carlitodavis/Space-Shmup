using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Enemy
{
    [Header("Enemy_1 Inscribed Fields")]
    [Tooltip("# of seconds for a full sine wave")]
    public float waveFrequency = 2;
    [Tooltip("Sine wave width in meters")]
    public float waveWidth = 4;
    [Tooltip("Amount the ship will roll left and right with the sine wave")]
    public float waveRotY = 45;

    private float x0;
    private float birthTime;
    // Start is called before the first frame update
    void Start()
    {
        // Set x0 to the starting x position of the Enemy_1
        x0 = pos.x;
        birthTime = Time.time;
    }
    public override void Move()
    {
        // Because pos is a property, we have to get it and set it like this
        Vector3 tempPos = pos;
        // Adjust the x position based on a Sine wave
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        // Update the position
        pos = tempPos;

        // rotate the ship to face the direction of travel
        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);

        // base.Move() will handle the downward movement
        base.Move();

        // print (bndCheck.isOnScreen);
    }
}
