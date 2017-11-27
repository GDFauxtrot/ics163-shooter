using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour {
    // dumb little generic things to manage databases and make life just a bit easier
    private class ItemPrice<T1, T2> {
        public T1 level;
        public T2 price;

        public ItemPrice() {
            level = default(T1);
            price = default(T2);
        }
        public ItemPrice(T1 level, T2 price) {
            this.level = level;
            this.price = price;
        }
    }
    private static ItemPrice<T1, T2> FindItemPriceForLevel<T1, T2>(T1 level, List<ItemPrice<T1, T2>> list) {
        foreach(ItemPrice<T1, T2> ip in list) {
            if (ip.level.Equals(level))
                return ip;
        }
        return null;
    }


    public GameObject playerMoneyText,
        healthUpgradeButton, healthUpgradeText, healthUpgradeCostText,
        damageUpgradeButton, damageUpgradeText, damageUpgradeCostText,
        chargeTimeUpgradeButton, chargeTimeUpgradeText, chargeTimeUpgradeCostText;

    PersistentData persistentData;
    
    List<ItemPrice<int, int>> healthUpgradeCosts;
    List<ItemPrice<int, int>> damageUpgradeCosts;
    List<ItemPrice<float, int>> chargeTimeUpgradeCosts;

    void Start () {
        persistentData = GameObject.Find("PersistentDataObject").GetComponent<PersistentData>();

        SetupUpgradeCosts();

        playerMoneyText.GetComponent<Text>().text = "$: " + persistentData.playerMoney.ToString();

        UpdateGUIValues();
        //healthUpgradeText.GetComponent<Text>().text = persistentData.playerHealth.ToString() + "/7";
        //damageUpgradeText.GetComponent<Text>().text = persistentData.playerDamage.ToString() + "/7";
        //chargeTimeUpgradeText.GetComponent<Text>().text = persistentData.playerChargeTime.ToString() + "/0.5";

        //healthUpgradeCostText.GetComponent<Text>().text = "$: " + FindItemPriceForLevel(persistentData.playerHealth, healthUpgradeCosts).price.ToString();
        //damageUpgradeCostText.GetComponent<Text>().text = "$: " + FindItemPriceForLevel(persistentData.playerDamage, damageUpgradeCosts).price.ToString();
        //chargeTimeUpgradeCostText.GetComponent<Text>().text = "$: " + FindItemPriceForLevel(persistentData.playerChargeTime, chargeTimeUpgradeCosts).price.ToString();
    }

    public void HealthPressed() {
        ItemPrice<int, int> newItemPrice = PerformUpgradeAction(persistentData.playerHealth, healthUpgradeCosts);

        persistentData.playerHealth = newItemPrice.level;

        UpdateGUIValues();

        // if this "next index" == next "next index" -- we just hit the last upgrade
        if (newItemPrice == healthUpgradeCosts[healthUpgradeCosts.Count - 1]) {
            healthUpgradeButton.GetComponent<Button>().enabled = false;
        }
    }

    public void DamagePressed() {
        ItemPrice<int, int> newItemPrice = PerformUpgradeAction(persistentData.playerDamage, damageUpgradeCosts);

        persistentData.playerDamage = newItemPrice.level;

        UpdateGUIValues();

        // if this "next index" == next "next index" -- we just hit the last upgrade
        if (newItemPrice == damageUpgradeCosts[damageUpgradeCosts.Count - 1]) {
            damageUpgradeButton.GetComponent<Button>().enabled = false;
        }
    }

    public void ChargeTimePressed() {
        ItemPrice<float, int> newItemPrice = PerformUpgradeAction(persistentData.playerChargeTime, chargeTimeUpgradeCosts);

        persistentData.playerChargeTime = newItemPrice.level;

        UpdateGUIValues();

        // if this "next index" == next "next index" -- we just hit the last upgrade
        if (newItemPrice == chargeTimeUpgradeCosts[chargeTimeUpgradeCosts.Count - 1]) {
            chargeTimeUpgradeButton.GetComponent<Button>().enabled = false;
        }
    }

    void UpdateGUIValues() {
        ItemPrice<int, int> health = FindItemPriceForLevel(persistentData.playerHealth, healthUpgradeCosts);
        ItemPrice<int, int> damage = FindItemPriceForLevel(persistentData.playerDamage, damageUpgradeCosts);
        ItemPrice<float, int> chargeTime = FindItemPriceForLevel(persistentData.playerChargeTime, chargeTimeUpgradeCosts);

        healthUpgradeText.GetComponent<Text>().text = persistentData.playerHealth.ToString() + "/" + healthUpgradeCosts[healthUpgradeCosts.Count - 1].level.ToString();
        healthUpgradeCostText.GetComponent<Text>().text = (health.price == 0 ? "" : "$: " + health.price.ToString());
        
        damageUpgradeText.GetComponent<Text>().text = persistentData.playerDamage.ToString() + "/" + damageUpgradeCosts[damageUpgradeCosts.Count - 1].level.ToString();
        damageUpgradeCostText.GetComponent<Text>().text = (damage.price == 0 ? "" : "$: " + damage.price.ToString());

        chargeTimeUpgradeText.GetComponent<Text>().text = persistentData.playerChargeTime.ToString() + "/" + chargeTimeUpgradeCosts[chargeTimeUpgradeCosts.Count - 1].level.ToString();
        chargeTimeUpgradeCostText.GetComponent<Text>().text = (chargeTime.price == 0 ? "" : "$: " + chargeTime.price.ToString());
    }

    ItemPrice<T, int> PerformUpgradeAction<T>(T level, List<ItemPrice<T, int>> db) {
        // get current entry in list
        ItemPrice<T, int> itemPrice = FindItemPriceForLevel(level, db);

        if (persistentData.playerMoney < itemPrice.price || itemPrice == db[db.Count - 1])
            return itemPrice;

        // next index guaranteed to be there (we're not last, just checked for that)
        ItemPrice<T, int> nextItemPrice = db[db.IndexOf(itemPrice) + 1];

        persistentData.playerMoney -= itemPrice.price;
        playerMoneyText.GetComponent<Text>().text = "$: " + persistentData.playerMoney.ToString();

        return nextItemPrice;
    }

    void SetupUpgradeCosts() {
        healthUpgradeCosts = new List<ItemPrice<int, int>>();
        damageUpgradeCosts = new List<ItemPrice<int, int>>();
        chargeTimeUpgradeCosts = new List<ItemPrice<float, int>>();

        // WHEN IS UNITY UPGRADING TO NET 4.0 SO WE CAN USE TUPLES

        healthUpgradeCosts.Add(new ItemPrice<int, int>(2, 100));
        healthUpgradeCosts.Add(new ItemPrice<int, int>(3, 200));
        healthUpgradeCosts.Add(new ItemPrice<int, int>(4, 300));
        healthUpgradeCosts.Add(new ItemPrice<int, int>(5, 400));
        healthUpgradeCosts.Add(new ItemPrice<int, int>(6, 500));
        healthUpgradeCosts.Add(new ItemPrice<int, int>(7, 0));

        damageUpgradeCosts.Add(new ItemPrice<int, int>(1, 150));
        damageUpgradeCosts.Add(new ItemPrice<int, int>(2, 200));
        damageUpgradeCosts.Add(new ItemPrice<int, int>(3, 300));
        damageUpgradeCosts.Add(new ItemPrice<int, int>(4, 350));
        damageUpgradeCosts.Add(new ItemPrice<int, int>(5, 450));
        damageUpgradeCosts.Add(new ItemPrice<int, int>(6, 600));
        damageUpgradeCosts.Add(new ItemPrice<int, int>(7, 0));

        chargeTimeUpgradeCosts.Add(new ItemPrice<float, int>(3.0f, 100));
        chargeTimeUpgradeCosts.Add(new ItemPrice<float, int>(2.5f, 150));
        chargeTimeUpgradeCosts.Add(new ItemPrice<float, int>(2.0f, 200));
        chargeTimeUpgradeCosts.Add(new ItemPrice<float, int>(1.5f, 250));
        chargeTimeUpgradeCosts.Add(new ItemPrice<float, int>(1.0f, 300));
        chargeTimeUpgradeCosts.Add(new ItemPrice<float, int>(0.5f, 0));
    }
}
