using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [Header("Configurações do Prefab")]
    public GameObject prefabMoeda;

    [Header("Configurações de Tempo (Segundos)")]
    public float tempoInicialSpawn = 2f;
    public float intervaloSpawn = 4f;

    [Header("Limites da Arena (Área de Spawn)")]
    // Como a sua Arena tem escala 10x10, um limite de -4 a 4 garante 
    // que as moedas nasçam sempre em cima do quadrado cinza.
    public float limiteX = 4f;
    public float limiteY = 4f;

    private void Start()
    {
        // O InvokeRepeating chama a função de criar moeda repetidamente de forma automática
        InvokeRepeating("SpawnarMoedaAleatoria", tempoInicialSpawn, intervaloSpawn);
    }

    private void SpawnarMoedaAleatoria()
    {
        if (prefabMoeda == null) return;

        // Calcula uma posição X e Y aleatória dentro dos limites definidos
        float posX = Random.Range(-limiteX, limiteX);
        float posY = Random.Range(-limiteY, limiteY);
        Vector2 posicaoSpawn = new Vector2(posX, posY);

        // Cria a moeda na cena
        Instantiate(prefabMoeda, posicaoSpawn, Quaternion.identity);
    }
}