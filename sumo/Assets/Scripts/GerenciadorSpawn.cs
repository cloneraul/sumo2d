using System.Collections;
using UnityEngine;

public class GerenciadorSpawn : MonoBehaviour
{
    [Header("PREFAB BASE DA BOLINHA (Arraste o Jogador_Bolinha aqui)")]
    public GameObject prefabBolinhaBase;

    [Header("ARQUIVOS DE DADOS (ORDEM RIGOROSA DE 1 A 5)")]
    public BolinhaData[] dadosBolinhas;

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

        // Validação rápida caso os IDs venham zerados (por dar play direto na cena de teste)
        if (idJ1 == 0) idJ1 = 1; 
        if (idJ2 == 0) idJ2 = 1;

        // 3. Spawna o Jogador 1
        if (idJ1 < dadosBolinhas.Length && dadosBolinhas[idJ1] != null)
        {
            // Remove instâncias pré-existentes para evitar sobreposição violenta de colisores
            GameObject existenteJ1 = GameObject.Find("Teste_J1");
            if (existenteJ1 != null)
            {
                Debug.LogWarning("[SPAWN] Encontrado objeto 'Teste_J1' existente na cena. Removendo antes de spawnar novo clone para evitar sobreposição.");
                DestroyImmediate(existenteJ1);
            }

            GameObject cloneJ1 = Instantiate(prefabBolinhaBase, pontoSpawnJ1.position, Quaternion.identity);
            cloneJ1.name = "Teste_J1"; // Mantém o nome exato para colisões antigas funcionarem

            // Evita correções físicas violentas no primeiro frame: desativa simulação e reativa logo em seguida
            Rigidbody2D rb1 = cloneJ1.GetComponent<Rigidbody2D>();
            if (rb1 != null)
            {
                rb1.simulated = false;
                StartCoroutine(AtivarFisicaCoroutine(rb1, 0.02f));
            }

            BolinhaController controller = cloneJ1.GetComponent<BolinhaController>();
            if (controller != null)
            {
                controller.InicializarBolinha(dadosBolinhas[idJ1], true);
            }
        }
        else
        {
            Debug.LogError("[SPAWN] ScriptableObject para o ID do Jogador 1 não foi configurado na lista!");
        }

        // 4. Spawna o Jogador 2
        if (idJ2 < dadosBolinhas.Length && dadosBolinhas[idJ2] != null)
        {
            // Remove instâncias pré-existentes para evitar sobreposição violenta de colisores
            GameObject existenteJ2 = GameObject.Find("Teste_J2");
            if (existenteJ2 != null)
            {
                Debug.LogWarning("[SPAWN] Encontrado objeto 'Teste_J2' existente na cena. Removendo antes de spawnar novo clone para evitar sobreposição.");
                DestroyImmediate(existenteJ2);
            }

            GameObject cloneJ2 = Instantiate(prefabBolinhaBase, pontoSpawnJ2.position, Quaternion.identity);
            cloneJ2.name = "Teste_J2"; // Mantém o nome exato para colisões antigas funcionarem

            // Evita correções físicas violentas no primeiro frame: desativa simulação e reativa logo em seguida
            Rigidbody2D rb2 = cloneJ2.GetComponent<Rigidbody2D>();
            if (rb2 != null)
            {
                rb2.simulated = false;
                StartCoroutine(AtivarFisicaCoroutine(rb2, 0.02f));
            }

            BolinhaController controller = cloneJ2.GetComponent<BolinhaController>();
            if (controller != null)
            {
                controller.InicializarBolinha(dadosBolinhas[idJ2], false);
            }
        }
        else
        {
            Debug.LogError("[SPAWN] ScriptableObject para o ID do Jogador 2 não foi configurado na lista!");
        }
    }

    private IEnumerator AtivarFisicaCoroutine(Rigidbody2D rb, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = true;
        }
    }
}