using PuzzleGame.Ads;
using PuzzleGame.Gameplay;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PuzzleGame.UI
{
    [RequireComponent(typeof(Button))]
    public class RestartButton : MonoBehaviour
    {
        [SerializeField] private UnityEvent onClick;
        
        public float interval = 30f;

        static float lastAdsTime;
    
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        public void OnClick()
        {
            UserProgress.Current.GetGameState<GameState>(UserProgress.Current.CurrentGameId).Reset();
            UserProgress.Current.SaveGameState(UserProgress.Current.CurrentGameId);
            UserProgress.Current.Save();
            
            onClick?.Invoke();

            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
            if (UserProgress.Current.IsItemPurchased("no_ads"))
                return;
        
            //if (Time.unscaledTime - lastAdsTime < interval || !UnityAdsController.Instance.IsVideoLoaded)
                //return;

            //lastAdsTime = Time.unscaledTime;
            //UnityAdsController.Instance.ShowVideo();
        }
    }
}