using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WeaponRepository
{
    [Header("Generic")]
    [SerializeField] Transform weaponHolder;
    

    public Weapon Primary { get; private set; }
    public Weapon Secundart { get; private set; }
    public Weapon CurrentWeapon { get; private set; }

    List<Weapon> allWeapons = new List<Weapon>();
    List<Weapon> primaryWeapons = new List<Weapon>();
    List<Weapon> secundaryWeapons = new List<Weapon>();

    public void Start()
    {
        allWeapons = weaponHolder.GetComponents<Weapon>().ToList();
        var grouped = allWeapons.GroupBy(x => x.Data.Type);

        primaryWeapons = grouped.Where(x => x.Key == WeaponType.Primary).SelectMany(x => x).ToList();
        secundaryWeapons = grouped.Where(x => x.Key == WeaponType.Secundary).SelectMany(x => x).ToList();
    }

    public void SwapWeapons()
    {
        
    }

    void ChangeWeapon(Weapon wep)
    {
        CurrentWeapon = wep;

        allWeapons
            .Except(new Weapon[] { wep })
            .Select((x) =>
            {
                x.gameObject.SetActive(false);
                return x;
            });

        wep.gameObject.SetActive(true);        
    }

}
