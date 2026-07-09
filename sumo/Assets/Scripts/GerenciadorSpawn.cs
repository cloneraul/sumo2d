using UnityEngine;

public class GerenciadorSpawn : MonoBehaviour
{
    [Header("ORDEM RIGOROSA DE ÍNDICES (1 a 5)")]
    public GameObject[] prefabsBolinhas;

    [Header("Pontos de Spawn na Arena")]
    public Transform pontoSpawnJ1;
    public Transform pontoSpawnJ2;

    void Start()
    {
        // 1. Verifica se a memória global existe
        if (DadosGlobais.Instancia == null)
        {
            Debug.LogError("[SPAWN] DadosGlobais não encontrado! Certifique-se de iniciar o jogo pela cena _Boot.");
            return;
        }

        // 2. Pega os IDs selecionados no menu
        int idJ1 = DadosGlobais.Instancia.tipoEscolhidoJ1;
        int idJ2 = DadosGlobais.Instancia.tipoEscolhidoJ2;

        // Validação rápida caso os IDs venham zerados (por dar play direto na cena)
        if (idJ1 == 0) idJ1 = 1; // Padrão Veloz se der erro
        if (idJ2 == 0) idJ2 = 1;

        // 3. Spawna o jogador 1
        if (idJ1 < prefabsBolinhas.Length && prefabsBolinhas[idJ1] != null)
        {
            GameObject cloneJ1 = Instantiate(prefabsBolinhas[idJ1], pontoSpawnJ1.position, Quaternion.identity);
            cloneJ1.name = "Teste_J1"; // Mantém o nome idêntico para a colisão da moeda funcionar!
            
            // Tenta configurar o input do jogador 1 no script dele (se houver uma variável pra isso)
            BolinhaController controller = cloneJ1.GetComponent<BolinhaController>();
            if (controller != null)
            {
                // Se o seu script de bolinha usar uma variável para diferenciar controle, ative aqui.
                // Exemplo comum: controller.ehJogador1 = true;
            }
        }
        else
        {
            Debug.LogError("[SPAWN] Prefab para o ID do Jogador 1 não foi configurado na lista!");
        }

        // 4. Spawna o jogador 2
        if (idJ2 < prefabsBolinhas.Length && prefabsBolinhas[idJ2] != null)
        {
            GameObject cloneJ2 = Instantiate(prefabsBolinhas[idJ2], pontoSpawnJ2.position, Quaternion.identity);
            cloneJ2.name = "Teste_J2"; // Mantém o nome idêntico para a colisão da moeda funcionar!
            
            BolinhaController controller = cloneJ2.GetComponent<BolinhaController>();
            if (controller != null)
            {
                // Exemplo comum: controller.ehJogador1 = false;
            }
        }
        else
        {
            Debug.LogError("[SPAWN] Prefab para o ID do Jogador 2 não foi configurado na lista!");
        }
    }
}