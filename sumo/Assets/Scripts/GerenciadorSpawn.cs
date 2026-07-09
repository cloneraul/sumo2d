using UnityEngine;

public class GerenciadorSpawn : MonoBehaviour
{
    [Header("Prefabs das Bolinhas (ORDEM RIGOROSA DE ÍNDICES)")]
    [Tooltip("Índice 0: Vazio/Padrão (não usado)\nÍndice 1: Bolinha Veloz\nÍndice 2: Bolinha Tanque\nÍndice 3: Bolinha Demolidora\nÍndice 4: Bolinha Planta\nÍndice 5: Bolinha Equilibrada")]
    public GameObject[] prefabsBolinhas = new GameObject[6];

    [Header("Pontos de Spawn na Arena")]
    public Transform pontoSpawnJ1;
    public Transform pontoSpawnJ2;

    private BolinhaController bolinhaj1;
    private BolinhaController bolinhaj2;

    private void Start()
    {
        ValidarConfiguracao();

        int tipoJ1 = DadosGlobais.Instancia.tipoEscolhidoJ1;
        int tipoJ2 = DadosGlobais.Instancia.tipoEscolhidoJ2;

        Debug.Log($"[GerenciadorSpawn] Lendo tipos do menu: J1={tipoJ1}, J2={tipoJ2}");

        SpawnarJogador(tipoJ1, pontoSpawnJ1, 1);
        SpawnarJogador(tipoJ2, pontoSpawnJ2, 2);

        ConectarJogadores();
    }

    private void ValidarConfiguracao()
    {
        if (prefabsBolinhas == null || prefabsBolinhas.Length < 6)
        {
            Debug.LogError("[GerenciadorSpawn] Array prefabsBolinhas deve ter pelo menos 6 elementos!");
            return;
        }

        if (pontoSpawnJ1 == null || pontoSpawnJ2 == null)
        {
            Debug.LogError("[GerenciadorSpawn] Pontos de Spawn (pontoSpawnJ1 e pontoSpawnJ2) não configurados!");
            return;
        }
    }

    private void SpawnarJogador(int idTipo, Transform pontoSpawn, int idJogador)
    {
        if (idTipo < 1 || idTipo >= prefabsBolinhas.Length)
        {
            Debug.LogError($"[GerenciadorSpawn] ID de tipo {idTipo} inválido! Deve estar entre 1 e {prefabsBolinhas.Length - 1}");
            return;
        }

        GameObject prefab = prefabsBolinhas[idTipo];
        if (prefab == null)
        {
            Debug.LogError($"[GerenciadorSpawn] Prefab no índice {idTipo} está vazio!");
            return;
        }

        GameObject instancia = Instantiate(prefab, pontoSpawn.position, Quaternion.identity);
        BolinhaController bolinha = instancia.GetComponent<BolinhaController>();

        if (bolinha != null)
        {
            bolinha.idJogador = idJogador;

            if (idJogador == 1)
            {
                bolinhaj1 = bolinha;
            }
            else if (idJogador == 2)
            {
                bolinhaj2 = bolinha;
            }

            Debug.Log($"[Spawn] Jogador {idJogador} (Tipo {idTipo}) instanciado em {pontoSpawn.position}");
        }
        else
        {
            Debug.LogError($"[GerenciadorSpawn] Prefab no índice {idTipo} não possui componente BolinhaController!");
        }
    }

    private void ConectarJogadores()
    {
        if (bolinhaj1 != null && bolinhaj2 != null)
        {
            bolinhaj1.inimigo = bolinhaj2;
            bolinhaj2.inimigo = bolinhaj1;
            Debug.Log("[GerenciadorSpawn] Jogadores conectados como inimigos um do outro!");
        }
        else
        {
            Debug.LogWarning("[GerenciadorSpawn] Falha ao conectar jogadores - verifique os prefabs!");
        }
    }
}
