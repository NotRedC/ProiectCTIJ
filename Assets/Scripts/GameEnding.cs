using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameEnding : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadeCurtain;
    [SerializeField] private string mainMenuSceneName;

    [SerializeField][TextArea] private string endingMessage;
    [SerializeField] private float textDuration;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(PlayEndingSequence(other.gameObject));
        }
    }

    IEnumerator PlayEndingSequence(GameObject player)
    {
        
        player.GetComponent<PlayerMovement>().enabled = false;

     
        Rigidbody2D body = player.GetComponent<Rigidbody2D>();
        body.linearVelocity = Vector2.zero;

        TauntManager.Instance.ShowMessage(endingMessage, TauntType.Story);
        yield return new WaitForSeconds(textDuration);
        float fadeTimer = 0f;
        while (fadeTimer < 2f)
        {
            fadeCurtain.alpha = fadeTimer / 2f;
            fadeTimer += Time.deltaTime;
            yield return null;
        }
        fadeCurtain.alpha = 1f; 
        PlayerPrefs.DeleteAll(); 
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
