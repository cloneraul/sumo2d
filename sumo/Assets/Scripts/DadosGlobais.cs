using UnityEngine;

public class DadosGlobais : MonoBehaviour
{
    public static DadosGlobais Instancia;

    [Header("Escolhas do Menu")]
    public int tipoEscolhidoJ1 = 0;
    public int tipoEscolhidoJ2 = 0;

    [Header("Dados do Fim da Partida")]
    public int jogadorVencedor;         // Guarda se quem venceu foi o Jogador 1 ou 2
    public string nomeBolinhaVencedora;   // Guarda o nome do Scriptable Object do campeão

    private void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}