using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.SceneManagement;
using LevelManagement;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms; 


[RequireComponent(typeof(LevelGoal))]
    public class GameManager : Singleton<GameManager>

    {   Board m_board;

        public bool m_isReadyToBegin = false;
        bool m_isGameOver = false;
        public bool IsComboTextActivated = false;
        public int numofCombo;
        public string levelName;
        public int levelIndex;
        Tile[,] m_allTiles;
        private DataManager m_dataManager;
        private SoundManager m_soundManager;

    [SerializeField]
    private int mainMenuIndex = 1;


 


    void Update()
        {
            if (Input.GetKey("escape"))
            {
                QuitGame(); 
            }
        }

        public void QuitGame()
        {
            //execute function shows exit message window through UI Manager. UI Manager has the refernece of every UI in our game
            Application.Quit();
    }
     

    public bool IsGameOver //public version of m_isgameover.
        {
            get
            {
                return m_isGameOver;
            }
            set
            {
                m_isGameOver = value;
            }
        }

        bool m_isWinner = false;
        bool m_isReadyToReload = false;




        LevelGoal m_levelGoal;
        // LevelGoalTimed m_levelGoalTimed;
        LevelGoalCollected m_levelGoalCollected;

        public LevelGoal LevelGoal { get { return m_levelGoal; } } // other classes access Levelgoal through gamemanager. 



    public override void Awake() //already defined awake in levelgoal singleton, we have to override it. tile.cs and rectxformmover.
    {

        base.Awake();  //get all the logic from singleton base class. awake is earlier than stat.
        m_dataManager = Object.FindObjectOfType<DataManager>();
        m_soundManager = Object.FindObjectOfType<SoundManager>();

        if (SceneManager.GetActiveScene().buildIndex > 1)
        {

            m_levelGoal = GetComponent<LevelGoal>();
            //   m_levelGoalTimed = GetComponent<LevelGoalTimed>(); // omitted because levelgoal class now has time related components.
            m_levelGoalCollected = GetComponent<LevelGoalCollected>();

            
            m_board = GameObject.FindObjectOfType<Board>().GetComponent<Board>();
        }
    }

    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
     //   m_soundManager.PlayRandomMusic();


        if (SceneManager.GetActiveScene().buildIndex > 1)
        { 
            UIManager.Instance.MainMenuButtonOff();
        }


        if (SceneManager.GetActiveScene().buildIndex > 1)
            {
            GameMenu.Open();

            if (UIManager.Instance != null)
                {
                    if (UIManager.Instance.scoreMeter != null)
                    {
                        UIManager.Instance.scoreMeter.SetupStars(m_levelGoal);
                    }

                    //use the s cene name as the level  name
                    if (UIManager.Instance.levelNameText != null)
                    {
                        //get a reference to the current scene
                        Scene scene = SceneManager.GetActiveScene();
                        UIManager.Instance.levelNameText.text = scene.name;
                    }


                    if (m_levelGoalCollected != null)
                    {
                        UIManager.Instance.EnableCollectionGoalLayout(true);
                        UIManager.Instance.SetupCollectionGoalLayout(m_levelGoalCollected.collectionGoals);
                    }
                    else
                    {
                        UIManager.Instance.EnableCollectionGoalLayout(false);
                    }

                    bool useTimer = (m_levelGoal.levelCounter == LevelCounter.Timer);

                    UIManager.Instance.EnableTimer(useTimer);
                    UIManager.Instance.EnableMovesCounter(!useTimer);
                }




                m_levelGoal.movesLeft++; //add 1
                UpdateMoves();
                StartCoroutine("ExecuteGameLoop");

            }
        }

        // update the Text component that shows our moves left
        public void UpdateMoves()
        {
            // if the LevelGoal is not timed (e.g. LevelGoalScored)...
            if (m_levelGoal.levelCounter == LevelCounter.Moves)
            {
                // decrement a move
                m_levelGoal.movesLeft--;

                // update the UI
                if (UIManager.Instance != null && UIManager.Instance.movesLeftText != null)
                {
                    UIManager.Instance.movesLeftText.text = m_levelGoal.movesLeft.ToString();
                }
            }
        }

        IEnumerator ExecuteGameLoop()
        {
            yield return StartCoroutine("StartGameRoutine");


            yield return StartCoroutine("PlayGameRoutine"); 

            //wait for board to refill
 
            yield return StartCoroutine("WaitForBoardRoutine", 0.5f);
         

            yield return StartCoroutine("EndGameRoutine");

        }
  

        public void BeginGame()
        {
            m_isReadyToBegin = true;

        }

        IEnumerator StartGameRoutine()
        {
            if (UIManager.Instance != null)
            {
                if (UIManager.Instance.messageWindow != null)
                {
                    UIManager.Instance.messageWindow.GetComponent<RectXformMover>().MoveOn();
                    int maxGoal = m_levelGoal.scoreGoals.Length - 1;
                    UIManager.Instance.messageWindow.ShowScoreMessage(m_levelGoal.scoreGoals[maxGoal]);
                }

                if (m_levelGoal.levelCounter == LevelCounter.Timer)
                {
                    UIManager.Instance.messageWindow.ShowTimedGoal(m_levelGoal.timeLeft);
                    
                }
                else
                {
                    UIManager.Instance.messageWindow.ShowMovesGoal(m_levelGoal.movesLeft);
                }

                if (m_levelGoalCollected != null) //if m_levelGoal Collected is exists
                {
                    UIManager.Instance.messageWindow.ShowCollectionGoal(true); //execute show collectional goal on messageWindow

                    GameObject goalLayout = UIManager.Instance.messageWindow.collectionGoalLayout;

                    if (goalLayout != null)
                    {
                        UIManager.Instance.SetupCollectionGoalLayout(m_levelGoalCollected.collectionGoals, goalLayout, 80); //each unit is 70 across, so gives extra spacing(10) 
                    }
                }
            }



            while (!m_isReadyToBegin)
            {
                yield return null;
            }

            //if it is ready to begin, (  if button clicked, m_isReadyToBegin = true;

            if (UIManager.Instance != null && UIManager.Instance.screenFader != null)
            {
                UIManager.Instance.screenFader.FadeOff();
            }

            yield return new WaitForSeconds(0.5f);

            if (m_board != null)
            {
                m_board.SetupBoard();
            }
        }

        IEnumerator PlayGameRoutine()
        {
            if (m_levelGoal.levelCounter == LevelCounter.Timer)
            {
                m_levelGoal.StartCountdown(); //it runs every level uses timer.
            }


            while (!m_isGameOver)
            { 
                m_isGameOver = m_levelGoal.IsGameOver();
                m_isWinner = m_levelGoal.IsWinner();
            
                //wait one frame
                yield return null;
            }
        }

        IEnumerator WaitForBoardRoutine(float delay = 0f)
        {
            if (m_levelGoal.levelCounter == LevelCounter.Timer && UIManager.Instance != null
                && UIManager.Instance.timer != null)
            {
                UIManager.Instance.timer.FadeOff();
                UIManager.Instance.timer.paused = true;
            }
            if (m_board != null)
            {
                yield return new WaitForSeconds(m_board.swapTime);
                while (m_board.isRefilling)
                {
                    yield return null;
                }
            }

            yield return new WaitForSeconds(delay);
        }

        IEnumerator EndGameRoutine()
        {
            m_isReadyToReload = false;

            
            if (m_isWinner) //winner
            {

            if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
                {
                    UIManager.Instance.MainMenuButtonOff();
                }
                else
                {
                    UIManager.Instance.MainMenuButtonOn();
                }
                    
                ShowWinScreen();
            }
            else //game failed
            {
                UIManager.Instance.MainMenuButtonOn();
                ShowLoseScreen();
            }
           
           yield return new WaitForSeconds(1f);

            if (UIManager.Instance != null && UIManager.Instance.screenFader != null)
            {
                UIManager.Instance.screenFader.FadeOn();
            }

            // Disable GameMenu 
           GameMenu.Instance.gameObject.SetActive(false);
           // GameMenu.Close();
 
            while (!m_isReadyToReload)
            {
                yield return null;
            }

       

    }

        void ShowLoseScreen()
        {
            if (UIManager.Instance != null && UIManager.Instance.messageWindow != null)
            {
                UIManager.Instance.messageWindow.GetComponent<RectXformMover>().MoveOn();
                UIManager.Instance.messageWindow.ShowLoseMessage();
                UIManager.Instance.messageWindow.ShowCollectionGoal(false);

                string caption = "";
                if (m_levelGoal.levelCounter == LevelCounter.Timer)
                {
                    caption = "Out of time!";
                }
                else
                {
                    caption = "Out of moves!";
                }

                UIManager.Instance.messageWindow.ShowGoalCaption(caption, 0, 70);

                if (UIManager.Instance.messageWindow.goalFailedIcon != null)
                {
                    UIManager.Instance.messageWindow.ShowGoalImage(UIManager.Instance.messageWindow.goalFailedIcon);
                }


            }




            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayLoseSound();
            }
        }

        void ShowWinScreen()
        {
            if (UIManager.Instance != null && UIManager.Instance.messageWindow != null)
            {
                UIManager.Instance.messageWindow.GetComponent<RectXformMover>().MoveOn();

      
            
            UIManager.Instance.messageWindow.ShowWinMessage();
            UIManager.Instance.messageWindow.ShowCollectionGoal(false);
                

            if (ScoreManager.Instance != null)
                {
                    string scoreStr = "you scored\n" + ScoreManager.Instance.CurrentScore.ToString() + "points!";


                    // decide whether to save data or not

                    if(m_dataManager != null) 
                    {
                         

                            if (m_dataManager.score[SceneManager.GetActiveScene().buildIndex - 3] < ScoreManager.Instance.CurrentScore) //if score in savedata less than current score
                            {
                                m_dataManager.score[SceneManager.GetActiveScene().buildIndex - 3] = ScoreManager.Instance.CurrentScore; //update score
                             
                            }

                            if (SceneManager.GetActiveScene().buildIndex != SceneManager.sceneCountInBuildSettings - 1) // is current level last stage?
                            { 
                                m_dataManager.unlocked[SceneManager.GetActiveScene().buildIndex - 2] = true; //unlock next level
                            }
                         
                            m_dataManager.Save();

                    
                    }
                    UIManager.Instance.messageWindow.ShowGoalCaption(scoreStr, 0, 70);
                    
                    //Submit Leaderboard scores, if authenticated
                    if (PlayGamesPlatform.Instance.localUser.authenticated)
                    {
                      
                        PlayGamesPlatform.Instance.ReportScore(ScoreManager.Instance.CurrentScore, GPGSIds.leaderboardLevels[SceneManager.GetActiveScene().buildIndex - 3], (bool success) =>
                        { 
                            Debug.Log("Leaderboard update success : " + success);
                        });


                        if(ScoreManager.Instance.CurrentScore > 3000)
                        {
                            PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_scoring_over_3000, 100.0F, (bool success) =>
                            {
                                Debug.Log("achivement done");
                            });
                        }
                         
                    }


                }

                if (UIManager.Instance.messageWindow.goalCompleteIcon != null)
                {
                    UIManager.Instance.messageWindow.ShowGoalImage(UIManager.Instance.messageWindow.goalCompleteIcon);
                }
            }

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayWinSound();
            }
        }

    

        public void ReloadScene()
        {
            m_isReadyToReload = true;

            if (m_isWinner && SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
            {
                Debug.Log(SceneManager.GetActiveScene().buildIndex + "번째 씬");
                Debug.Log("Cleared whole level!");
                 BackToMain();//main menu
            }

            if (m_isWinner && SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings - 1)
            {
                Debug.Log("test" + SceneManager.sceneCountInBuildSettings);
                LoadNextLevel(); //next stage
            }

            if (!m_isWinner && m_isGameOver)
            {
                 ReloadLevel(); 
            }


        }


    public void BackToMain()
    {
        SceneManager.LoadScene("Main");
        MainMenu.Open();
    }

    public void LoadLevel(string levelName)
        {
            SceneManager.LoadScene(levelName);
        }


    public void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings)
        {
            if (levelIndex == mainMenuIndex)
            {
                MainMenu.Open();
            }

            SceneManager.LoadScene(levelIndex);
        }
        else
        {
            Debug.LogWarning("GAMEMANAGER LoadLevel Error: invalid scene specified!");
        }

    }
        private void ReloadLevel()
        {

            LoadLevel(SceneManager.GetActiveScene().buildIndex);
        }


     


    public void LoadNextLevel()
        {
            int nextSceneIndex = (SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings;
            LoadLevel(nextSceneIndex);
        }


        public void ScorePoints(GamePiece piece, int multiplier = 1, int bonus = 0)
        {
            if (piece != null)
            {
                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.AddScore(piece.scoreValue * multiplier + bonus);
                    m_levelGoal.UpdateScoreStars(ScoreManager.Instance.CurrentScore);
                    //update meter everytime we rechead scorepoint.
                    if (UIManager.Instance != null && UIManager.Instance.scoreMeter != null)
                    {
                        UIManager.Instance.scoreMeter.UpdateScoreMeter(ScoreManager.Instance.CurrentScore, m_levelGoal.scoreStars);
                    }
                }

                if (SoundManager.Instance != null && piece.clearSound != null)
                {
                    SoundManager.Instance.PlayClipAtPoint(piece.clearSound, Vector3.zero, SoundManager.Instance.fxVolume);
                }
            }
        }

        public void AddTime(int timeValue)
        {
            if (m_levelGoal.levelCounter == LevelCounter.Timer)
            {
                m_levelGoal.AddTime(timeValue);
            }
        }

        public void UpdateCollectionGoals(GamePiece pieceToCheck)
        {
            if (m_levelGoalCollected != null)
            {
                m_levelGoalCollected.UpdateGoals(pieceToCheck);
            }
        }

        public void Combo(int numofCombo)
        {

            StartCoroutine(ShowCombo(numofCombo));
        }


        IEnumerator ShowCombo(int numofCombo)
        {

            UIManager.Instance.comboWindow.SetActive(true);

            yield return new WaitForSeconds(0.3f);

            Debug.Log(numofCombo + "Combo!");

            UIManager.Instance.comboText.text = numofCombo.ToString() + "Combo!";

            yield return new WaitForSeconds(0.3f);

            IsComboTextActivated = false;
            UIManager.Instance.comboWindow.SetActive(false);

            yield return null;

            UIManager.Instance.comboText.text = "";
        }

    public bool CheckRemainedBrakeable()
    {
        int countbreakableValue = 0;

        for(int i=0; i<m_board.width; i++)
        {
            for(int j=0; j<m_board.height; j++)
            {
                if (m_board.m_allTiles[i,j].breakableValue > 0)
                {
                    countbreakableValue++;
                }
                
            }
        }

        if (countbreakableValue > 0)
        {
//            Debug.Log(countbreakableValue + "남은 타일이 있습니다." ); //if breakable remained
            return true;
        }
        else
        {
  //          Debug.Log(countbreakableValue + "남은 타일이 없습니다." ); //if there isn't breakable remained
            return false;
        }
    }

}
