using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EngineAnimator : MonoBehaviour
{
    public ParticleSystem Move;
    public ParticleSystem Hover;
    public ParticleSystem Jump;
    public ParticleSystem Sprint;


    List<ParticleSystem> particles = new List<ParticleSystem>();
    PlayerProperties playerProps;


    private void Start()
    {
        particles.Add(Move);
        particles.Add(Hover);
        particles.Add(Jump);
        particles.Add(Sprint);
        playerProps = GetComponentInParent<PlayerProperties>();
        StartCoroutine(UpdateEmission());
    }

    public void RotateEngine(Vector3 InputVector)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(InputVector.z * 20, 0, -InputVector.x * 20)), 1);
    }

    private void PlayParticle(ParticleSystem particle)
    {
        particles.Where(p => p != particle).ToList().ForEach(p => { p.Stop(); p.Clear(); });
        if(particle.isPlaying == false)
        {
            particle.Play();
        }
    }

    public void StopParticles()
    {
        Move.Stop();
        Hover.Stop();
        Sprint.Stop();
    }

    public void MoveEngine()
    {
        PlayParticle(Move);
        
    }

    public void SprintEngine()
    {
        PlayParticle(Sprint);
    }

    public void HoverEngine()
    {
        PlayParticle(Hover);
    }

    public void JumpEngine()
    {
        Jump.Clear();
        StartCoroutine(ClearParticles());
        PlayParticle(Jump);
    }

    IEnumerator ClearParticles()
    {
        yield return new WaitForSeconds(0.1f);
        Move.Clear();
        Hover.Clear();
        Sprint.Clear();
    }

    IEnumerator UpdateEmission()
    {
        PlayerController.MovementStates lastMoveState = PlayerController.MovementStates.IDLE;

        while (true)
        {
            if(playerProps.MovementState != lastMoveState)
            {
                switch (playerProps.MovementState)
                {
                    case PlayerController.MovementStates.WALKING:
                        MoveEngine();
                    break;

                    case PlayerController.MovementStates.SPRINTING:
                        SprintEngine();
                    break;

                    case PlayerController.MovementStates.HOVERING:
                        HoverEngine();
                        break;

                    case PlayerController.MovementStates.FALLING:
                        StopParticles();
                        break;

                }
            }
            lastMoveState = playerProps.MovementState;
            Debug.Log(playerProps.MovementState);
            yield return new WaitForSeconds(0.01f);


        }
    }
}
