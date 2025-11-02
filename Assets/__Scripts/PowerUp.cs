using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoundsCheck))]
[RequireComponent(typeof(Rigidbody))]
public class PowerUp : MonoBehaviour
{
    [Header("Inscribed")]
    public Vector2 rotMinMax = new Vector2(15, 90);
    public Vector2 driftMinMax = new Vector2(0.25f, 2);
    public float lifeTime = 10f;
    public float fadeTime = 4f;

    [Header("Dynamic")]
    [SerializeField] private eWeaponType _type = eWeaponType.none;
    public GameObject cube;
    public TextMesh letter;
    public Vector3 rotPerSecond;
    public float birthTime;

    private Rigidbody rb;
    private Collider col;
    private BoundsCheck bndCheck;
    private Material cubeMat;

    void Awake()
    {
        // Ensure Collider exists (3D) and is a trigger
        col = GetComponent<Collider>();
        if (col == null) col = gameObject.AddComponent<BoxCollider>();
        col.isTrigger = true;

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        bndCheck = GetComponent<BoundsCheck>();

        // Find visuals
        cube = (transform.childCount > 0) ? transform.GetChild(0).gameObject : gameObject;
        var rend = cube.GetComponent<Renderer>();
        if (rend != null) cubeMat = rend.material;  // runtime instance
        letter = GetComponentInChildren<TextMesh>();

        // Random drift
        Vector3 vel = Random.onUnitSphere; vel.z = 0f; vel.Normalize();
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rb.velocity = vel;

        transform.rotation = Quaternion.identity;

        rotPerSecond = new Vector3(
            Random.Range(rotMinMax.x, rotMinMax.y),
            Random.Range(rotMinMax.x, rotMinMax.y),
            Random.Range(rotMinMax.x, rotMinMax.y)
        );

        birthTime = Time.time;
    }

    void Update()
    {
        if (cube != null)
            cube.transform.rotation *= Quaternion.Euler(rotPerSecond * fadeTime * Time.deltaTime);

        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;
        if (u >= 1f) { Destroy(gameObject); return; }

        if (u > 0f)
        {
            if (cubeMat != null)
            {
                var c = cubeMat.color; c.a = 1f - u; cubeMat.color = c;
            }
            if (letter != null)
            {
                var c2 = letter.color; c2.a = 1f - (u * 0.5f); letter.color = c2;
            }
        }

        if (bndCheck != null && !bndCheck.isOnScreen)
        {
            Destroy(gameObject);
        }
    }

    public eWeaponType type
    {
        get { return _type; }
        set { Setype(value); }
    }

    public void Setype(eWeaponType wt)
    {
        _type = wt;
        var def = Main.GET_WEAPON_DEFINITION(_type);
        if (cubeMat != null) cubeMat.color = def.powerUpColor;
        if (letter != null) letter.text = def.letter;
    }

    public void AbsorbedBy(GameObject target)
    {
        Destroy(gameObject);
    }
}
