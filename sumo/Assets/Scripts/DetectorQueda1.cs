using UnityEngine;

public class DetectorQueda : MonoBehaviour
{
    private Collider2D meuColisor;

    private void Awake()
    {
        // Pega o BoxCollider2D da própria Zona de Queda
        meuColisor = GetComponent<Collider2D>();
    }

    // Mudamos para Stay2D: o Unity fica checando enquanto a bolinha estiver encostando
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Verifica se quem encostou foi o Jogador 1 ou Jogador 2
        bool ehJogador1 = collision.name.StartsWith("Teste_J1");
        bool ehJogador2 = collision.name.StartsWith("Teste_J2");

        if (!ehJogador1 && !ehJogador2) return;

        // PEGA O CENTRO DA BOLINHA
        Vector2 centroDoJogador = collision.bounds.center;

        // O TRUQUE: Verifica se o CENTRO da bolinha está totalmente dentro do nosso retângulo de queda
        bool centroEstaDentro = meuColisor.OverlapPoint(centroDoJogador);

        // Se a pontinha encostou mas o centro ainda está fora, ignora! (Salvou-se na borda)
        if (!centroEstaDentro) return;

        // Se o centro entrou na zona, aí sim ela caiu por completo!
        if (ehJogador1)
        {
            Debug.LogWarning("[ZONA DE QUEDA] DETECTADO: Jogador 1 caiu por COMPLETO!");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegistrarPonto(2); // J1 caiu, ponto do J2
            }
        }
        else if (ehJogador2)
        {
            Debug.LogWarning("[ZONA DE QUEDA] DETECTADO: Jogador 2 caiu por COMPLETO!");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegistrarPonto(1); // J2 caiu, ponto do J1
            }
        }
    }
}