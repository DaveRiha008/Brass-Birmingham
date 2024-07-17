using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
  [System.Serializable]
  public struct PlayerData
  {
    public int index;
    public bool AIReplaced;
    public int victoryPoints;
    public int income;
    public int money;
    public int moneySpentThisTurn;
    public string name;
  }

  [System.Serializable]
  public struct GameData
  {
    public List<int> playerTurns;
    public ERA currentEra;
    public int activePlayerIndex;
    public int activePlayerTurnIndex;
    public int numOfPlayers;
    public int numOfAI;
    public int randomSeed;
    public int actionPlayed;
    public bool firstEverRound;
  }

  [System.Serializable]
  public struct CardData
  {
    public int id;
    public CARD_TYPE type;
    public CARD_STATE state;

    public List<INDUSTRY_TYPE> myIndustries;
    public string myLocation;
  }

  [System.Serializable]
  public struct CardManagerData
  {
    public List<int> player1HandIds;
    public List<int> player2HandIds;
    public List<int> player3HandIds;
    public List<int> player4HandIds;

    public List<int> player1DiscardIds;
    public List<int> player2DiscardIds;
    public List<int> player3DiscardIds;
    public List<int> player4DiscardIds;
  }

  [System.Serializable]
  public struct ObjectManagerData
  {
    public List<int> firstUnbuiltBreweryIndeces;
    public List<int> firstUnbuiltIronWorksIndeces;
    public List<int> firstUnbuiltCoalMineIndeces;
    public List<int> firstUnbuiltCottonMillIndeces;
    public List<int> firstUnbuiltPotteryIndeces;
    public List<int> firstUnbuiltManufacturerIndeces;
  }

  [System.Serializable]
  public struct TileScriptData
  {
    public int id;
    public int builtOnSpaceID;
    public bool alreadyBuilt;
    public bool isUpgraded;
    public bool isDeveloped;
    public int ownerPlayerIndex;

    //For breweries, ironworks and coalmines
    public int resourceCount;

  }

  [System.Serializable]
  public struct ResourceStorageData
  {
    public int countInStorage;

  }

  [System.Serializable]
  public struct NetworkSpaceData
  {
    public int id;
    public ERA myEra;
    public bool occupied;
    public int vehicleOwnerIndex;
  }

  [System.Serializable]
  public struct MerchantTileData
  {
    public int id;
    public int myTileID;
    public bool hasBarrel;
  }

  public List<PlayerData> playerData = new List<PlayerData>();
  public GameData gameData;
  public List<CardData> cardData = new();
  public CardManagerData cardManagerData = new();
  public ObjectManagerData objectManagerData = new();
  public List<TileScriptData> tileScriptData = new();
  public ResourceStorageData coalStorageData = new();
  public ResourceStorageData ironStorageData = new();
  public List<NetworkSpaceData> networkData = new();
  public List<MerchantTileData> merchantData = new();


  public string ToJson()
  {
    return JsonUtility.ToJson(this);
  }

  public void LoadFromJson(string a_Json)
  {
    JsonUtility.FromJsonOverwrite(a_Json, this);
  }
}

public interface ISaveable
{
  /// <summary>
  /// Add information to correct struct in given saveData
  /// </summary>
  void PopulateSaveData(SaveData a_SaveData);
  /// <summary>
  /// Load information from correct struct in given saveData
  /// </summary>
  void LoadFromSaveData(SaveData a_SaveData);
}