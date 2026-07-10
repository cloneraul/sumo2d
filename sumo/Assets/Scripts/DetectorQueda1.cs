using UnityEngine;

public class DetectorQueda : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Só faz alguma coisa se o objeto que colidir for REALMENTE o Jogador 1 ou Jogador 2
        bool ehJogador1 = collision.name.StartsWith("Teste_J1");
        bool ehJogador2 = collision.name.StartsWith("Teste_J2");

        // Se NÃO for nenhum dos dois (ex: se for a Arena que acabou de nascer), ignora completamente!
        if (!ehJogador1 && !ehJogador2)
        {
            return; 
        }

        // Se chegou até aqui, com certeza é um dos jogadores!
        if (ehJogador1)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegistrarPonto(2); // J1 caiu, ponto do J2
            }
        }
        else if (ehJogador2)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegistrarPonto(1); // J2 caiu, ponto do J1
            }
        }
    }
}