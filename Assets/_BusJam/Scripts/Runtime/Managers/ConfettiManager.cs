using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfettiManager : MonoBehaviour
{
    public static ConfettiManager Instance;
    public List<ParticleSystem> confettiParticles;

    private void Awake()
    {
        MakeSingleton();
    }

    private void MakeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void PlayConfettiParticles()
    {
        foreach (var confettiParticle in confettiParticles)
        {
            confettiParticle.Play();
        }
    }
}