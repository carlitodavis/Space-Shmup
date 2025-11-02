using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public static Hero S { get; private set; }

    [Header("Inscribed")]
    public float speed = 30f;
    public float rollMult = -45f;
    public float pitchMult = 30f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40f;
    public Weapon[] weapons;

    [Header("Dynamic")]
    [Range(0, 4)]
    [SerializeField] private float _shieldLevel = 4f;

    private GameObject lastTriggerGo = null;

    public delegate void WeaponFireDelegate();
    public event WeaponFireDelegate fireEvent;

    void Awake()
    {
        if (S == null) S = this;
        else { Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!"); Destroy(gameObject); return; }

        ClearWeapons();
        if (weapons != null && weapons.Length > 0 && weapons[0] != null)
        {
            weapons[0].SetType(eWeaponType.spread);
        }
    }

    void Update()
    {
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");

        Vector3 pos = transform.position;
        pos.x += hAxis * speed * Time.deltaTime;
        pos.y += vAxis * speed * Time.deltaTime;
        transform.position = pos;

        transform.rotation = Quaternion.Euler(vAxis * pitchMult, hAxis * rollMult, 0f);

        if ((Input.GetAxis("Jump") == 1f || Input.GetButton("Fire1")) && fireEvent != null)
        {
            fireEvent();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;

        if (go == lastTriggerGo) return;
        lastTriggerGo = go;

        Enemy enemy = go.GetComponent<Enemy>();
        PowerUp pUp = go.GetComponent<PowerUp>();

        if (enemy != null)
{
    TakeDamage(Mathf.Max(0f, enemy.contactDamage));

    if (enemy.destroyOnContact)
        Destroy(go);

    lastTriggerGo = null;
    return;
}


        if (pUp != null)
        {
            AbsorbPowerUp(pUp);
            lastTriggerGo = null;
            return;
        }

        Debug.LogWarning("Hero trigger hit by non-Enemy/PowerUp: " + go.name);
        lastTriggerGo = null;
    }

    public void AbsorbPowerUp(PowerUp pUp)
    {
        switch (pUp.type)
        {
            case eWeaponType.shield:
                shieldLevel++;
                break;

            case eWeaponType.none:
                Debug.LogWarning("PowerUp with type = none");
                break;

            default:
                ClearWeapons();
                if (weapons != null && weapons.Length > 0 && weapons[0] != null)
                {
                    weapons[0].SetType(pUp.type);
                }
                else
                {
                    Debug.LogWarning("No weapon slot 0 assigned on Hero.");
                }
                break;
        }
        pUp.AbsorbedBy(gameObject);
    }

    public void TakeDamage(float dmg)
{
    shieldLevel -= Mathf.Abs(dmg);
}


    public float shieldLevel
    {
        get { return _shieldLevel; }
        private set
        {
            _shieldLevel = Mathf.Min(value, 4f);
            if (_shieldLevel < 0f)
            {
                Destroy(gameObject);
                Main.HERO_DIED();
            }
        }
    }

    Weapon GetEmptyWeaponSlot()
    {
        if (weapons == null) return null;
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null && weapons[i].type == eWeaponType.none)
            {
                return weapons[i];
            }
        }
        return null;
    }

    void ClearWeapons()
    {
        if (weapons == null) return;
        foreach (Weapon w in weapons)
        {
            if (w != null) w.SetType(eWeaponType.none);
        }
    }
}
