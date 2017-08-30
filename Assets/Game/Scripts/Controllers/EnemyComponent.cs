using UnityEngine;
using System.Collections;

public class EnemyComponent : Entity
{
    [SerializeField] EnemyMotor motor;
    [Header("Health")]
    [SerializeField] int defaultHP;


    private void OnEnable()
    {
        SetDefaultHP(defaultHP);

        motor.OnDisable += Die;
        motor.Start();
    }
    private void Update()
    {
        motor.Update();
    }
    public override void Hit(int damage)
    {
        print("hit");
        base.Hit(damage);
    }
    public override void Die()
    {
        AppServices.Instance.AudioManager.SoundEffectsManager.PlaySound(SoundEffects.Explosion);

        GameServices.Instance.ChallengeManager.DeactivateEntity(gameObject);
        SetDefaultHP(defaultHP);
    }
}
