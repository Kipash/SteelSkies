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
        motor.OnDisable += Die;
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
        //Debug.Log(motor.OnDisable.GetInvocationList().Length);
        motor.OnDisable = null;

        Timing.KillCoroutines(EnemyEffects.EnemyEffectsTag);
        Timing.KillCoroutines(EnemyWeaponry.EnemyTimingTag);

        effects.DieEffect(transform.position);
        
        GameServices.Instance.ChallengeManager.DeactivateEntity(gameObject);
        SetDefaultHP(defaultHP);
    }
    public override void Hit(int damage)
    {
        base.Hit(damage);
        effects.HitEffect();
    }
}
