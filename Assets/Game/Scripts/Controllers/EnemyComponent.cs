using UnityEngine;
using System.Collections;
using MovementEffects;

public class EnemyComponent : Entity
{
    [SerializeField] EnemyMotor motor;
    [SerializeField] EnemyWeaponry weaponry;
    [SerializeField] EnemyEffects effects;


    [Header("Misc")]
    [SerializeField] bool Disable;

    [Header("Health")]
    [SerializeField] int defaultHP;

    public PathSettings PathSettings
    {
        get { return motor.pathSettings; }
        set { motor.pathSettings = value; motor.GetPath(); }
    }

    private void OnEnable()
    {
        SetDefaultHP(defaultHP);

        motor.OnDisable = null;
        motor.OnDisable += () => { Deactivate(false); } ;
        motor.Start();

        effects.Start();

        if (GameServices.Instance != null && GameServices.IsMainManagerPresent)
            weaponry.Target = GameServices.Instance.GameManager.Player.transform;

        weaponry.Start();
    }
    private void OnDisable()
    {
        Timing.KillCoroutines(EnemyEffects.EnemyEffectsTag);
        Timing.KillCoroutines(EnemyWeaponry.EnemyTimingTag);
    }
    private void Update()
    {
        motor.Update();
        weaponry.RotateTowers();
    }
    public override void Die()
    {
        effects.DieEffect(transform.position);
        Deactivate(true);
    }
    void Deactivate(bool natural)
    {
        motor.OnDisable = null;
        GameServices.Instance.ChallengeManager.DeactivateEntity(gameObject, natural);
        SetDefaultHP(defaultHP);

        Timing.KillCoroutines(EnemyEffects.EnemyEffectsTag);
        Timing.KillCoroutines(EnemyWeaponry.EnemyTimingTag);
    }
    public override void Hit(int damage)
    {
        base.Hit(damage);
        effects.HitEffect();
    }
}
