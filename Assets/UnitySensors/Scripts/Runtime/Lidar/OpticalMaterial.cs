using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpticalMaterial : MonoBehaviour
{
    [Header("Parameters")]

    [Range(0.0f, 1.0f)]
    [SerializeField] private float _roughness;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float _surfaceReflectance;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float _specularReflectance;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float _retroReflectance;

    [Header("Informations (No need to input)")]

    [SerializeField] private float _f_r;
    [SerializeField] private float _f_d;

    [SerializeField] private float _d;
    [SerializeField] private float _lambda;
    [SerializeField] private float _g;

    private float _alpha2;

    public float roughness { get => this._roughness; }
    public float surfaceReflactance { get => this._surfaceReflectance; }
    public float specularReflectance { get => this._specularReflectance; }
    public float retroReflectance { get => this._retroReflectance; }


    // Start is called before the first frame update
    void Start()
    {
        this._alpha2 = this._roughness * this._roughness * this._roughness * this._roughness;
        this._f_r = (1 - this._specularReflectance) * this._retroReflectance/(Mathf.PI * this._alpha2);
        this._f_d = (1 - this._specularReflectance) * (1 - this._retroReflectance)*this.surfaceReflactance/(Mathf.PI);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetReflectance(float theta)
    {
        float f_s = this.calc_f_s(theta);
        float reflectance = f_s + this._f_r + this._f_d;
        return reflectance;
    }

    private float calc_f_s(float theta)
    {
        this._d = this._alpha2/Mathf.PI/ this.square(this.square(Mathf.Cos(theta))*(this._alpha2 - 1.0f)+1.0f);
        this._lambda = -1.0f+Mathf.Sqrt((1.0f/this.square(Mathf.Cos(theta))-1.0f)*this._alpha2+1.0f);
        this._g = 1 / (1 + this._lambda);
        float f_s = this._d * this._g * this._specularReflectance / square(Mathf.Cos(theta)) * 0.25f;
        return f_s;
    }

    private float square(float a)
    {
        return a * a;
    }
}
