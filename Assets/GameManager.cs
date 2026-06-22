using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;


[System.Serializable]
public class GameConfig
{
    public float moneyMultiplierFromGems = 0.2f;
    public float potionMoneyBoost = 2f;
    public float potionDuration = 30f;
    public int rebirthGemReward = 5;
    public float upgradeCostMultiplier = 1.5f;
    public float rebirthCostMultiplier = 2f;
    public float gemUpgradeBonus = 1f;
    public float potionDropChance = 0.1f;
}



[System.Serializable]
public class SaveData
{
    public double money;
    public double moneyPerSecond;
    public double upgradeCost;
    public double boostCost;

    public int gem;
    public int level;

    public int gemUpgradeCost;

    public float exp;
    public float expToNextLevel;
}




public class GameManager : MonoBehaviour
{

    public Text moneyText;
    public Text upgradeCostText;
    public Text gemText;
    public Text gemUpgradeCostText;
    public Text levelText;
    public Text expText;
    public Text boostCostText;



    public double money = 0;
    public double moneyPerSecond = 1;

    public double upgradeCost = 10;

    public double boostCost = 1000;



    public int gem = 0;

    int gemUpgradeCost = 50;



    public int level = 1;

    public float exp = 0;

    public float expToNextLevel = 100;


    public float GemUpgradeCost = 50;



    public GameObject coinPanel;

    public GameObject gemPanel;



    GameConfig config = new GameConfig();



    float potionTimer = 0;

    bool isPotionActive = false;




    string SavePath()
    {
        return Application.persistentDataPath + "/save.json";
    }





    string FormatNumber(double num)
    {

        if (num < 1000)
            return num.ToString("0");


        string[] suffix =
        {
            "",
            "K","M","B","T",
            "Qa","Qi","Sx","Sp","Oc","No",
            "Dc","Ud","Dd","Td",
            "Qad","Qid","Sxd","Spd",
            "Ocd","Nod",
            "Ce"
        };



        int index = 0;



        while (num >= 1000 && index < suffix.Length - 1)
        {
            num /= 1000;
            index++;
        }



        return num.ToString("0.###") + suffix[index];

    }






    void Start()
    {

        DontDestroyOnLoad(gameObject);


        LoadGameData();



        TextAsset json =
        Resources.Load<TextAsset>("gameConfig");



        if (json != null)
        {
            config =
            JsonUtility.FromJson<GameConfig>(json.text);
        }




        if (coinPanel != null)
            coinPanel.SetActive(true);



        if (gemPanel != null)
            gemPanel.SetActive(false);

    }







    void Update()
    {


        float gemBonus =
        1 + gem * config.moneyMultiplierFromGems;



        float potionBonus =
        isPotionActive ? config.potionMoneyBoost : 1f;




        money += moneyPerSecond *
        gemBonus *
        potionBonus *
        Time.deltaTime;



        exp += (float)(moneyPerSecond * Time.deltaTime);




        if (exp >= expToNextLevel)
        {
            LevelUp();
        }





        if (moneyText != null)
            moneyText.text =
            "お金: " + FormatNumber(money);



        if (upgradeCostText != null)
            upgradeCostText.text =
            "強化: " + FormatNumber(upgradeCost);



        if (boostCostText != null)
            boostCostText.text =
            "倍率アップ: " + FormatNumber(boostCost);



        if (gemText != null)
            gemText.text =
            "ジェム: " + gem;



        if (gemUpgradeCostText != null)
            gemUpgradeCostText.text =
            "ジェム強化: " + gemUpgradeCost;



        if (levelText != null)
            levelText.text =
            "Lv: " + level;



        if (expText != null)
            expText.text =
            "EXP: " + (int)exp +
            "/" +
            (int)expToNextLevel;




        if (isPotionActive)
        {

            potionTimer -= Time.deltaTime;


            if (potionTimer <= 0)
            {
                isPotionActive = false;
            }

        }

    }






    void LevelUp()
    {

        exp -= expToNextLevel;

        level++;

        moneyPerSecond += 2;

        expToNextLevel *= 1.5f;

    }







    public void AddMoney()
    {
        money += 1;
    }






    public void Upgrade()
    {

        if (money >= upgradeCost)
        {

            money -= upgradeCost;

            moneyPerSecond += 1;


            upgradeCost *=
            config.upgradeCostMultiplier;

        }

    }







    public void BoostIncome()
    {

        if (money >= boostCost)
        {

            money -= boostCost;


            moneyPerSecond *= 1.5;


            boostCost *= 1.8;

        }

    }







    // ジェム強化
    // 50→100→200→400
    // お金/秒 ×1.5

    public void GemUpgrade2()
    {

        if (gem >= gemUpgradeCost)
        {

            gem -= gemUpgradeCost;


            moneyPerSecond *= 1.5;


            gemUpgradeCost *= 2;
            
            gem -= gemUpgradeCost;


        }

    }







    public void Rebirth()
    {

        if (money >= 500)
        {

            int getGem =
            Mathf.RoundToInt((float)(money / 100));



            gem += getGem;



            money = 0;

            moneyPerSecond = 1;

            upgradeCost = 10;

            boostCost = 1000;

        }

    }







    public void UsePotion()
    {

        isPotionActive = true;

        potionTimer =
        config.potionDuration;

    }







    public void OnClickChangeGemPanel()
    {

        coinPanel.SetActive(false);

        gemPanel.SetActive(true);

    }







    public void OnClickChangeCoinPanel()
    {

        coinPanel.SetActive(true);

        gemPanel.SetActive(false);

    }







    public void LoadGeimu()
    {
        SceneManager.LoadScene("geimu");
    }




    public void LoadSample()
    {
        SceneManager.LoadScene("SampleScene");
    }








    public void SaveGame()
    {

        SaveData data =
        new SaveData();



        data.money = money;

        data.moneyPerSecond =
        moneyPerSecond;


        data.upgradeCost =
        upgradeCost;


        data.boostCost =
        boostCost;



        data.gem = gem;

        data.level = level;



        data.gemUpgradeCost =
        gemUpgradeCost;



        data.exp = exp;

        data.expToNextLevel =
        expToNextLevel;




        string json =
        JsonUtility.ToJson(data, true);



        File.WriteAllText(
        SavePath(),
        json);

    }








    public void LoadGameData()
    {

        if (!File.Exists(SavePath()))
            return;



        string json =
        File.ReadAllText(SavePath());



        SaveData data =
        JsonUtility.FromJson<SaveData>(json);



        money = data.money;

        moneyPerSecond =
        data.moneyPerSecond;



        upgradeCost =
        data.upgradeCost;



        boostCost =
        data.boostCost;



        gem =
        data.gem;



        level =
        data.level;



        gemUpgradeCost =
        data.gemUpgradeCost;



        exp =
        data.exp;



        expToNextLevel =
        data.expToNextLevel;


    }






    private void OnApplicationQuit()
    {
        SaveGame();
    }




    private void OnApplicationPause(bool pause)
    {

        if (pause)
            SaveGame();

    }





    public void SaveButton()
    {
        SaveGame();
    }

}