using UnityEngine;

public class DetectorQueda : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se o objeto que caiu foi o Jogador 1
        if (collision.name == "Teste_J1")
        {
            // Se o J1 caiu, o ponto vai para o Jogador 2!
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegistrarPonto(2);
            }
        }
        // Verifica se o objeto que caiu foi o Jogador 2
        else if (collision.name == "Teste_J2")
        {
            // Se o J2 caiu, o ponto vai para o Jogador 1!
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegistrarPonto(1);
            }
        }
    }
}