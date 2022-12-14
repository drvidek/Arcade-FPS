using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] private GunType[] gunType;
    [SerializeField] private GameObject model;
    [SerializeField] private ParticleSystem _PSysBirth;
    [SerializeField] private ParticleSystem _PSysDeath;
    [SerializeField] private Light _lightTop;
    [SerializeField] private MeshRenderer _fresnel;
    [SerializeField] private AudioSource _sfxBirth;
    [SerializeField] private AudioSource _sfxDeath;
    public GunType Gun { get; private set; }
    public bool Collected { get; private set; }



    private void Start()
    {
        int i = Random.Range(0, gunType.Length);
        Gun = gunType[i];
        _lightTop.color = Gun.Colour;
        _fresnel.material.SetColor("_Color", Gun.Colour);
        model.SetActive(false);
        _sfxBirth.Play();
    }

    private void Update()
    {
        if (model.activeInHierarchy || Collected)
            return;

        if (!_PSysBirth.isPlaying)
        {
            StartLife();
        }
    }

    private void StartLife()
    {
        model.SetActive(true);
        model.GetComponentInChildren<Animator>().SetTrigger("Create");
        
    }

    public IEnumerator EndOfLife()
    {
        Collected = true;
        model.SetActive(false);
        _PSysDeath.Play();
        _sfxDeath.Play();
        while (_PSysDeath.isPlaying)
            yield return null;
        Destroy(gameObject);
    }
}
