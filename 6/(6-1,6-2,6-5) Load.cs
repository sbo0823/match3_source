using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms; 



public class Load : MonoBehaviour
{
    public string[] Text;
    public Text loadText;
    public Text googleLoginText;
    public int percentage;
    public Slider slider; 

    void Start()
    {
        AuthenticateUser();
        StartCoroutine(LoadScene());
        
    }


    void AuthenticateUser()
    {

        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        GPGSSignin();
        /*
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.Activate();

        if (!PlayGamesPlatform.Instance.IsAuthenticated())
        {
            Social.localUser.Authenticate((bool success) =>
            {
                if (success == true)
                {
                    loginText.text = "Login to Google Play Games Services succesfully";
                    Debug.Log("Logged in to Google Play Games Service.");


                }
                else
                {
                    Debug.LogError("Unable to sign in to Google Play Games Services");
                    loginText.text = "Could not login to Google Play Games Services";
                    loginText.color = Color.red;

                }

            });
        }
        else
        {
            loginText.text = "Already Logged in";
        }

*/
    }

    private void GPGSSignin()
    {

        Social.localUser.Authenticate((bool success) =>
        {
            if (success == true)
            {
                Debug.Log("Logged in to Google Play Games Services");
            }
            else
            {
                Debug.LogError("Unable to sign in to Google Play Games Services");
                //    loginText.text = "Could not login to Google Play Games Services";
                //   loginText.color = Color.red;
            }
        });

    }

    IEnumerator LoadScene()
    {
        yield return null;

        //Begin to load the Scene you specify
         
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        //Don't let the Scene activate until you allow it to
        asyncOperation.allowSceneActivation = false;

        float timer = 0.0f;


        int n = 0;
        int percentageUnit = (int)(100 / Text.Length);

        //When the load is still in progress, output the Text and progress bar
        while (!asyncOperation.isDone)
        {
            yield return null;

            timer += Time.deltaTime;

            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                slider.value = Mathf.Lerp(slider.value, 1f, timer);

                percentage = (int)(Mathf.Lerp(slider.value, 1f, timer) * 100);

                //               Debug.Log(percentage);
                 

                if (n >= 0 && (percentage > (percentageUnit * n)) && (percentage > (percentageUnit * (n + 1))))
                {
                    ChangeloadText(n); 
                    n++;

                    yield return new WaitForSeconds(0.5f);
                }


                //Change the Text to show the Scene is ready
                //Wait till unity activate the Scene
                if (slider.value == 1.0f)
                {    //Activate the Scene
                    loadText.text = Text[Text.Length - 1];
                    yield return new WaitForSeconds(2f);
                    asyncOperation.allowSceneActivation = true;
                }

            }
            else
            {
                slider.value = Mathf.Lerp(slider.value, asyncOperation.progress, timer);
                if (slider.value >= asyncOperation.progress)
                {
                    timer = 0f;
                }

            }

            yield return null;
        }
    }

    private void ChangeloadText(int n)
    {
        loadText.text = Text[n];
 //       Debug.Log(Text[n]);
        n++; 
    }
}