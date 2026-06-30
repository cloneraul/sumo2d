using UnityEngine;

// Essa linha faz o Unity criar uma opção no menu de clique direito para gerarmos os arquivos de dados
[CreateAssetMenu(fileName = "NovaBolinha", menuName = "SumoBolinhas/DadosBolinha")]
public class BolinhaData : ScriptableObject
{
    [Header("Identificação")]
    public string nomeBolinha;

    [Header("Atributos Base")]
    [Tooltip("Velocidade de movimento inicial da bolinha.")]
    public float velocidadeInicial = 8f;
    
    [Tooltip("Força do empurrão que ela aplica no inimigo.")]
    public float forcaEmpurraoBase = 12f;
    
    [Tooltip("Massa do Rigidbody2D (quanto maior, mais difícil de ser empurrada).")]
    public float massaBase = 1f;
    
    [Tooltip("Tamanho visual da bolinha (Escala).")]
    public float tamanhoEscala = 1f;

    [Header("Visual (Cores)")]
    [Tooltip("Cor da bolinha se for controlada pelo Jogador 1.")]
    public Color corJogador1 = Color.blue;
    
    [Tooltip("Cor da bolinha se for controlada pelo Jogador 2.")]
    public Color corJogador2 = Color.red;
}