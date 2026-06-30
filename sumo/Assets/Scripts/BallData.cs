using UnityEngine;

[CreateAssetMenu(fileName = "NovaBolinha", menuName = "Bolinha/BallData")]
public class BallData : ScriptableObject
{
    public string nome;

    public float velocidade = 5f;

    public float forca = 10f;

    public float peso = 1f;

    public float tamanho = 1f;

    public Sprite sprite;
}