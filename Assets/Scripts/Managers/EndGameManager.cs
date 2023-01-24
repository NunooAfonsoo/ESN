using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.UI.EndGame;
using Constants;

namespace Assets.Scripts.Managers
{
    public class EndGameManager : MonoBehaviour
    {
        public static EndGameManager Instance;
        [SerializeField] Texture[] backgroundImages;

        void Awake()
        {
            Instance = this;
        }

        
        void Update()
        {
            if (!GameWonUIManager.Instance.gameObject.activeInHierarchy)
            {
                if (Time.timeSinceLevelLoad >= 10 && PopulationManager.Instance.Infected.Count == 0)
                {
                    if (SceneManager.GetSceneByName(Scenes.UI).isLoaded) SceneManager.UnloadSceneAsync(Scenes.UI);

                    GameWonUIManager.Instance.gameObject.SetActive(true);
                    GameWonUIManager.Instance.VictoryText.text = EndGameTexts.VictoryText;
                    GameWonUIManager.Instance.BackgroundImage.texture = backgroundImages[Random.Range(0, backgroundImages.Length)];
                }
                else if (Time.timeSinceLevelLoad >= 10 && PopulationManager.Instance.AverageApprovalRating < 15)
                {
                    if (SceneManager.GetSceneByName(Scenes.UI).isLoaded) SceneManager.UnloadSceneAsync(Scenes.UI);

                    GameWonUIManager.Instance.gameObject.SetActive(true);
                    GameWonUIManager.Instance.VictoryText.text = EndGameTexts.SupportLossText;
                    GameWonUIManager.Instance.BackgroundImage.texture = backgroundImages[Random.Range(0, backgroundImages.Length)];
                }
                else if (Time.timeSinceLevelLoad >= 10 && EconomyManager.Instance.UpdatedMoney < -300)
                {
                    if (SceneManager.GetSceneByName(Scenes.UI).isLoaded) SceneManager.UnloadSceneAsync(Scenes.UI);

                    GameWonUIManager.Instance.gameObject.SetActive(true);
                    GameWonUIManager.Instance.VictoryText.text = EndGameTexts.MoneyLossText;
                    GameWonUIManager.Instance.BackgroundImage.texture = backgroundImages[Random.Range(0, backgroundImages.Length)];
                }
            }
        }

    }
}
