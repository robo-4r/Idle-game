using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    int gemUpgradeCost = 5;

    public int level = 1;
    public float exp = 0;
    public float expToNextLevel = 100;

    public GameObject coinPanel;
    public GameObject gemPanel;

    GameConfig config = new GameConfig();

    float potionTimer = 0f;
    bool isPotionActive = false;

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        TextAsset json = Resources.Load<TextAsset>("gameConfig");

        if (json != null)
        {
            config = JsonUtility.FromJson<GameConfig>(json.text);
        }
        else
        {
            Debug.LogWarning("gameConfig.json が見つかりません。デフォルト設定を使用します");
        }

        if (coinPanel != null) coinPanel.SetActive(true);
        if (gemPanel != null) gemPanel.SetActive(false);
    }

    void Update()
    {
        float gemBonus = 1 + (gem * config.moneyMultiplierFromGems);
        float potionBonus = isPotionActive ? config.potionMoneyBoost : 1f;

        // ★ 1000円以上で1.5倍
        double bonusMultiplier = 1;
        if (money >= 1000)
        {
            bonusMultiplier = 1.5;
        }

        money += moneyPerSecond * gemBonus * potionBonus * bonusMultiplier * Time.deltaTime;
        exp += (float)(moneyPerSecond * Time.deltaTime);

        if (exp >= expToNextLevel)
        {
            LevelUp();
        }

        // UI表示
        if (moneyText != null)
            moneyText.text = "お金: " + ((int)money);

        if (upgradeCostText != null)
            upgradeCostText.text = "強化: " + ((int)upgradeCost);

        if (boostCostText != null)
            boostCostText.text = "倍率アップ: " + ((int)boostCost);

        if (gemText != null)
            gemText.text = "ジェム: " + gem;

        if (gemUpgradeCostText != null)
            gemUpgradeCostText.text = "ジェム強化: " + gemUpgradeCost;

        if (levelText != null)
            levelText.text = "Lv: " + level;

        if (expText != null)
            expText.text = "EXP: " + ((int)exp) + " / " + ((int)expToNextLevel);

        // ポーション処理
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
            upgradeCost *= config.upgradeCostMultiplier;
        }
    }

    // ★ ボタンで倍率アップ
    public void BoostIncome()
    {
        if (money >= boostCost)
        {
            money -= boostCost;
            moneyPerSecond *= 1.5;
            boostCost *= 1.8;
        }
    }

    public void GemUpgrade()
    {
        if (gem >= gemUpgradeCost)
        {
            gem -= gemUpgradeCost;
            moneyPerSecond += config.gemUpgradeBonus;
            gemUpgradeCost += 5;
        }
    }

    // ★ 転生（500円以上で可能・ジェム計算変更）
    public void Rebirth()
    {
        if (money >= 500)
        {
            int getGem = Mathf.RoundToInt((float)(money / 100));
            gem += getGem;

            money = 0;
            moneyPerSecond = 1;
            upgradeCost = 10;
        }
    }

    public void UsePotion()
    {
        isPotionActive = true;
        potionTimer = config.potionDuration;
    }

    public void OnClickChangeGemPanel()
    {
        if (coinPanel != null) coinPanel.SetActive(false);
        if (gemPanel != null) gemPanel.SetActive(true);
    }

    public void OnClickChangeCoinPanel()
    {
        if (coinPanel != null) coinPanel.SetActive(true);
        if (gemPanel != null) gemPanel.SetActive(false);
    }

    public void LoadGeimu()
    {
        SceneManager.LoadScene("geimu");
    }

    public void LoadSample()
    {
        SceneManager.LoadScene("SampleScene");
    }



}