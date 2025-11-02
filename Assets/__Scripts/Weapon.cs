using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eWeaponType
{
    none,
    blaster,
    spread,
    phaser,
    missile,
    laser,
    shield
}

[System.Serializable]
public class WeaponDefinition
{
    public eWeaponType type = eWeaponType.none;
    public string letter;
    public Color powerUpColor = Color.white;
    public GameObject weaponModelPrefab;
    public GameObject projectilePrefab;
    public Color projectileColor = Color.white;
    public float damageOnHit = 0;
    public float damagePerSecond = 0;
    public float delayBetweenShots = 0;
    public float velocity = 50;
}

public class Weapon : MonoBehaviour
{
    public static Transform PROJECTILE_ANCHOR;

    [Header("Dynamic")]
    [SerializeField]
    private eWeaponType _type = eWeaponType.none;

    public WeaponDefinition def;
    public float nextShotTime;

    private GameObject weaponModel;
    private Transform shotPointTrans;

    void Start()
    {
        if (PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        if (transform.childCount > 0)
            shotPointTrans = transform.GetChild(0);
        else
            shotPointTrans = transform;

        if (_type != eWeaponType.none)
            SetType(_type);
    }

    void OnEnable()
    {
        Hero hero = GetComponentInParent<Hero>();
        if (hero != null) hero.fireEvent += Fire;
    }

    void OnDisable()
    {
        Hero hero = GetComponentInParent<Hero>();
        if (hero != null) hero.fireEvent -= Fire;
    }

    public eWeaponType type
    {
        get { return _type; }
        set { SetType(value); }
    }

    public void SetType(eWeaponType wt)
    {
        _type = wt;

        if (_type == eWeaponType.none)
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            gameObject.SetActive(true);
        }

        def = Main.GET_WEAPON_DEFINITION(_type);

        if (_type == eWeaponType.blaster)
        {
            def.delayBetweenShots = 0.09f;
            def.damageOnHit = 1.5f;
        }

        if (weaponModel != null) Destroy(weaponModel);
        weaponModel = Instantiate(def.weaponModelPrefab, transform);
        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localScale = Vector3.one;

        nextShotTime = 0f;
    }

    private void Fire()
    {
        if (!gameObject.activeInHierarchy) return;
        if (Time.time < nextShotTime) return;

        ProjectileHero p;
        Vector3 vel = Vector3.up * def.velocity;

        switch (_type)
        {
            case eWeaponType.blaster:
                p = MakeProjectile();
                p.vel = vel;
                break;

            case eWeaponType.spread:
                float step = 10f;
                for (int i = -2; i <= 2; i++)
                {
                    p = MakeProjectile();
                    p.transform.rotation = Quaternion.AngleAxis(i * step, Vector3.back);
                    p.vel = p.transform.rotation * vel;
                }
                break;

            default:
                break;
        }
    }

    private ProjectileHero MakeProjectile()
    {
        GameObject go = Instantiate(def.projectilePrefab, PROJECTILE_ANCHOR);
        ProjectileHero p = go.GetComponent<ProjectileHero>();

        Vector3 pos = (shotPointTrans != null ? shotPointTrans.position : transform.position);
        pos.z = 0;
        p.transform.position = pos;

        p.type = _type;
        nextShotTime = Time.time + def.delayBetweenShots;
        return p;
    }
}
