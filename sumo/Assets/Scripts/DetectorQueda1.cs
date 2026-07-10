using UnityEngine;

public class DetectorQueda : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // TEXTO ESPIÃO: Isso vai printar no console o nome exato do que está ativando o sensor!
        Debug.LogWarning($"[ZONA DE QUEDA] Algo encostou em mim! Nome do objeto: '{collision.name}' | Tag: '{collision.tag}'");

        // Verifica se quem encostou foi o Jogador 1 ou Jogador 2
        bool ehJogador1 = collision.name.StartsWith("Teste_J1");
        bool ehJogador2 = collision.name.StartsWith("Teste_J2");

        if (!ehJogador1 && !ehJogador2)
        {
            // Se o console avisar que a Arena ou outro objeto apareceu aqui, o problema é a posição do colisor!
            Debug.Log($"[ZONA DE QUEDA] Objeto '{collision.name}' ignorado porque não é um jogador válido.");
            return; 
        }

        // Se for um jogador válido, computa o ponto
        if (ehJogador1)
        {
            Debug.Log("[ZONA DE QUEDA] DETECTADO: Jogador 1 caiu!");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegistrarPonto(2); // J1 caiu, ponto do J2
            }
        }
        else if (ehJogador2)
        {
            Debug.Log("[ZONA DE QUEDA] DETECTADO: Jogador 2 caiu!");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegistrarPonto(1); // J2 caiu, ponto do J1
            }
        }
    }
}