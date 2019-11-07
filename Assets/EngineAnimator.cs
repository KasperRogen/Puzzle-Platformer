using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EngineAnimator : MonoBehaviour
{
    public ParticleSystem Move;
    public ParticleSystem Hover;
    public ParticleSystem Jump;

    List<ParticleSystem> particles = new List<ParticleSystem>();

    private void Start()
    {
        particles.Add(Move);
        particles.Add(Hover);
        particles.Add(Jump);
    }

    private void PlayParticle(ParticleSystem particle)
    {
        particles.Where(p => p != particle).ToList().ForEach(p => p.Stop());
        if(particle.isPlaying == false)
        {
            particle.Play();
        }
    }

    public void StopParticles()
    {
        Move.Stop();
        Hover.Stop();
        Jump.Stop();
    }

    public void MoveEngine(Vector3 InputVector)
    {
        PlayParticle(Move);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(InputVector.z * 20, 0, -InputVector.x * 20)), 5);
    }

    public void HoverEngine(Vector3 InputVector)
    {
        PlayParticle(Hover);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(InputVector.z * 20, 0, -InputVector.x * 20)), 5);
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
    }
}
