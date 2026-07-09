using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GerenciadorPartida : MonoBehaviour
{
    [Header("Interface do Placar")]
    public TextMeshProUGUI textoPlacar;

    [Header("Pontos Iniciais")]
    private int pontosJ1 = 0;
    private int pontosJ2 = 0;

    private Transform pontoSpawnJ1;
    private Transform pontoSpawnJ2;

    void Start()
    {
        // Encontra os pontos de spawn na cena automaticamente para resetar o round
        pontoSpawnJ1 = GameObject.Find("Ponto_J1")?.transform;
        pontoSpawnJ2 = GameObject.Find("Ponto_J2")?.transform;

        AtualizarInterfacePlacar();
    }

    // Chamado pelo DetectorQueda quando uma bolinha cai no Trigger
    public void RegistrarQueda(string nomeBolinhaCaida)
    {
        if (nomeBolinhaCaida == "Teste_J1")
        {
            pontosJ2++;
            Debug.Log("Jogador 1 caiu! Ponto para o Jogador 2.");
        }
        else if (nomeBolinhaCaida == "Teste_J2")
        {
            pontosJ1++;
            Debug.Log("Jogador 2 caiu! Ponto para o Jogador 1.");
        }

        AtualizarInterfacePlacar();

        // Condição de Vitória (Melhor de 3: quem fizer 2 pontos ganha)
        if (pontosJ1 >= 2)
        {
            DeclararVencedorPartida(1);
        }
        else if (pontosJ2 >= 2)
        {
            DeclararVencedorPartida(2);
        }
        else
        {
            StartCoroutine(ResetarRoundCo());
        }
    }

    void AtualizarInterfacePlacar()
    {
        if (textoPlacar != null)
        {
            textoPlacar.text = $"J1: {pontosJ1}  |  J2: {pontosJ2}";
        }
    }

    IEnumerator ResetarRoundCo()
    {
        yield return new WaitForSeconds(1.5f); // Pausa dramática pós-queda

        BolinhaController j1 = GameObject.Find("Teste_J1")?.GetComponent<BolinhaController>();
        BolinhaController j2 = GameObject.Find("Teste_J2")?.GetComponent<BolinhaController>();

        // Coloca de volta nos pontos de spawn e zera a velocidade física
        if (j1 != null && pontoSpawnJ1 != null)
        {
            j1.transform.position = pontoSpawnJ1.position;
            j1.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }

        if (j2 != null && pontoSpawnJ2 != null)
        {
            j2.transform.position = pontoSpawnJ2.position;
            j2.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
    }

    void DeclararVencedorPartida(int jogadorVencedor)
    {
        if (DadosGlobais.Instancia != null)
        {
            DadosGlobais.Instancia.jogadorVencedor = jogadorVencedor;
            
            // Descobre o nome do Scriptable Object usado pelo vencedor para mostrar na tela final
            string tagBolinha = "Desconhecida";
            BolinhaController[] jogadores = FindObjectsByType<BolinhaController>(FindObjectsSortMode.None);
            foreach (var j in jogadores)
            {
                if (j.idJogador == jogadorVencedor && j.dadosBase != null)
                {
                    tagBolinha = j.dadosBase.name;
                }
            }
            DadosGlobais.Instancia.nomeBolinhaVencedora = tagBolinha;
        }

        // Carrega a cena separada de UI que você sugeriu!
        SceneManager.LoadScene("CenaVitoria");
    }
}