using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : Enemy
{
    [Header("Enemey_2 Inscribed")]
    public float lifeTime = 10;
    // Enemy 2 uses a Sine wave to modify a 2 point linear interpolation
    [Tooltip("Determines how much the Sine wave will ease the interpolation")]
    public float sinEccentricity = 0.6f;
    public AnimationCurve rotCurve;
    [Header("Enemey_2 Private Fields")]
    [SerializeField] private float birthTime;
    private Quaternion baseRotation;
    [SerializeField] private Vector3 p0, p1;
    // Start is called before the first frame update
    void Start()
    {
        // Pick any point on the left side of the screen
        p0 = Vector3.zero;
        p0.x = -bndCheck.camWidth - bndCheck.radius;
        p0.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        // Pick any point on the right side of the screen
        p1 = Vector3.zero;
        p1.x = bndCheck.camWidth + bndCheck.radius;
        p1.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        // Possibly swap sides
        if (Random.value > 0.5f)
        {
            // Setting the .x of each point to its negative value
            p0.x *= -1;
            p1.x *= -1;
        }

        // set the birthTime to the current time
        birthTime = Time.time;

        // set up the intial ship rotation
        transform.position = p0;
        transform.LookAt(p1, Vector3.back);
        baseRotation = transform.rotation;

    }
    public override void Move()
    {
        // This completely overrides the Move() method in Enemy.cs
        // Determine u based on the lifetime
        float u = (Time.time - birthTime) / lifeTime;

        // If u > 1, then it's time to destroy this Enemy_2
        if (u > 1)
        {
            Destroy(this.gameObject);
            return;
        }

        // Use the AnimationCurve to adjust u
        float shipRot = rotCurve.Evaluate(u) * 360;
       // if (p0.x > p1.x) shipRot = -shipRot; // flip rotation if going left
       // transform.rotation = Quaternion.Euler(0, shipRot, 0);
       transform.rotation = baseRotation * Quaternion.Euler(-shipRot, 0, 0);

        // Adjust u by adding a Sine wave to it
        u += sinEccentricity * (Mathf.Sin(u * Mathf.PI * 2 - Mathf.PI / 2) + 1) / 2;

        pos = (1 - u) * p0 + u * p1;
    }
}
