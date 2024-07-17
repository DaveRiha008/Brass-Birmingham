using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Constants
{
  //Game parameters
  readonly public static bool instantGameRestart = false;
  readonly public static bool initMainBoardLock = false;
  readonly public static bool initAIFreePlay = true;
  readonly public static int maxGames = 200;

  readonly public static int boatCost = 3;
  readonly public static int train1Cost = 5;
  readonly public static int train2Cost = 10;
  readonly public static int loanMoney = 30;
  readonly public static int loanIncomeCost = 3;
  readonly public static int maxActionsPerRound = 2;

  readonly public static int maxIncome = 99;
  readonly public static int maxVP = 99;

  readonly public static string saveFilesAbsolutePath = Application.dataPath;
  readonly public static string saveFileName = "SaveFile.dat";
  readonly public static string vicFileName = "VictoryData.dat";


  readonly public static Color[] playerColors = { Color.red, Color.yellow, Color.white, Color.magenta };

  //  Player start of game info
  readonly public static int initPlayerMoney = 17;
  readonly public static int initPlayerIncome = 10;
  readonly public static int initPlayerVicPts = 0;


  //Label texts
  //  action state

  //EN
  //readonly public static string actionStateTextChCard = "Please choose a Card";
  //readonly public static string actionStateTextChDeck = "Please choose a Wild Card";
  //readonly public static string actionStateTextChTile = "Please choose a Tile";
  //readonly public static string actionStateTextChSpace = "Please choose a Space";
  //readonly public static string actionStateTextChIron = "Please choose an Iron Source";
  //readonly public static string actionStateTextChCoal = "Please choose a Coal Source";
  //readonly public static string actionStateTextChBarrel = "Please choose a Barrel Source";
  //readonly public static string actionStateTextChNetwork = "Please choose a Network Space";

  //CZ
  readonly public static string actionStateTextChCard = "ProsÌm zvolte Kartu";
  readonly public static string actionStateTextChCardNoAction = "ProsÌm zahoÔte Kartu";
  readonly public static string actionStateTextChDeck = "ProsÌm zvolte Divokou Kartu";
  readonly public static string actionStateTextChTile = "ProsÌm vyberte Budovu";
  readonly public static string actionStateTextChSpace = "ProsÌm vyberte MÌsto pro budovu";
  readonly public static string actionStateTextChIron = "ProsÌm vyberte zdroj éeleza";
  readonly public static string actionStateTextChCoal = "ProsÌm vyberte zdroj UhlÌ";
  readonly public static string actionStateTextChBarrel = "ProsÌm vyberte zdroj Piva";
  readonly public static string actionStateTextChNetwork = "ProsÌm vyberte MÌsto pro SÌù";

  //  missing resource

  //EN
  //readonly public static string misResTextCard = "No Card to be chosen!";
  //readonly public static string misResTextWildCardInHand = "Can't Scout with Wild Card in hand";
  //readonly public static string misResTextTileBuild = "There is no available Tile to be built";
  //readonly public static string misResTextTileSell = "There is no other Tile to be Sold";
  //readonly public static string misResTextTileDevelop = "There is no Tile to be Developed";
  //readonly public static string misResTextSpaceBuild = "There is no available Space to build in";
  //readonly public static string misResTextNetSpace = "There is no available Space for another Network";
  //readonly public static string misResTextMoneyCoal = "Not enough Money to buy Coal";
  //readonly public static string misResTextMoneyIron = "Not enough Money to buy Iron";
  //readonly public static string misResTextMoneyNetwork = "Not enough Money to build another Network";
  //readonly public static string misResTextIncomeLoan = "Not enough Income to take a Loan";
  //readonly public static string misResTextIron = "No available Iron source to take from";
  //readonly public static string misResTextCoal = "No available Coal source to take from";
  //readonly public static string misResTextBarrel = "No available Barrel source to take from";

  //CZ
  readonly public static string misResTextCard = "é·dn· Karta na v˝bÏr!";
  readonly public static string misResTextWildCardInHand = "Nelze konat Pr˘zkum s Divokou Kartou v ruce!";
  readonly public static string misResTextTileBuild = "é·dn· budova, kter· lze postavit!";
  readonly public static string misResTextTileSell = "é·dn· dalöÌ budova nelze prodat!";
  readonly public static string misResTextTileDevelop = "é·dn· dalöÌ budova nelze Vyvinout!";
  readonly public static string misResTextSpaceBuild = "Na û·dnÈ MÌsto nelze stavÏt!";
  readonly public static string misResTextNetSpace = "Na û·dnÈ mÌsto nelze stavÏt SÌù!";
  readonly public static string misResTextMoneyCoal = "Nedostatek pÏnez na n·kup UhlÌ!";
  readonly public static string misResTextMoneyIron = "Nedostatek pÏnez na n·kup éeleza!";
  readonly public static string misResTextMoneyNetwork = "Nedostatek pÏnez pro postavenÌ dalöÌ SÌtÏ!";
  readonly public static string misResTextIncomeLoan = "NedostateËn˝ p¯Ìjem pro P˘jËku!";
  readonly public static string misResTextIron = "é·dn˝ zdroj éeleza nenÌ dostupn˝!";
  readonly public static string misResTextCoal = "é·dn˝ zdroj UhlÌ nenÌ dostupn˝!";
  readonly public static string misResTextBarrel = "é·dn˝ zdroj Piva nenÌ dostupn˝!";



  //  info
  //CZ
  readonly public static string playerChangedText = "ZmÏna hr·Ëe - nynÌ hraje: ";
  readonly public static string actionCanceledText = "Akce zruöena";
  readonly public static string succesfulBuildText = "Stavba probÏhla ˙spÏönÏ";
  readonly public static string succesfulSellText = "Prodej probÏhl ˙spÏönÏ";
  readonly public static string succesfulLoanText = "P˘jËka probÏhla ˙spÏönÏ";
  readonly public static string succesfulScoutText = "Pr˘zkum probÏhl ˙spÏönÏ";
  readonly public static string succesfulDevelopText = "Pr˘zkum probÏhl ˙spÏönÏ";
  readonly public static string succesfulNetworkText = "SÌù probÏhla ˙spÏönÏ";


  //CARDS
  readonly public static string cardsPath = "Cards";

  readonly public static string cardIndustriesPath = cardsPath + "/Industries";
  readonly public static string cardLocationsPath = cardsPath + "/Locations";

  //Industries
  readonly public static string cardBreweryPath = cardIndustriesPath + "/Brewery";
  readonly public static string cardCoalMinePath = cardIndustriesPath + "/CoalMine";
  readonly public static string cardIronWorksPath = cardIndustriesPath + "/IronWorks";
  readonly public static string cardManufacturerCottonMillPath = cardIndustriesPath + "/ManGoodsCottonMill";
  readonly public static string cardWildIndustryPath = cardIndustriesPath + "/WildIndustry";
  readonly public static string cardPotteryPath = cardIndustriesPath + "/Pottery";

  readonly public static string[] allIndustryCards = { cardBreweryPath, cardCoalMinePath, cardIronWorksPath,
    cardManufacturerCottonMillPath, cardPotteryPath };


  //Locations
  readonly public static string locationParentName = "Locations";

  readonly public static string cardBelperPath = cardLocationsPath + "/Belper";
  readonly public static string cardBirminghamPath = cardLocationsPath + "/Birmingham";
  readonly public static string cardCannockPath = cardLocationsPath + "/Cannock";
  readonly public static string cardCoalbrookdalePath = cardLocationsPath + "/Coalbrookdale";
  readonly public static string cardCoventryPath = cardLocationsPath + "/Coventry";
  readonly public static string cardDerbyPath = cardLocationsPath + "/Derby";
  readonly public static string cardDudleyPath = cardLocationsPath + "/Dudley";
  readonly public static string cardKidderminsterPath = cardLocationsPath + "/Kidderminster";
  readonly public static string cardLeekPath = cardLocationsPath + "/Leek";
  readonly public static string cardNuneatonPath = cardLocationsPath + "/Nuneaton";
  readonly public static string cardRedditchPath = cardLocationsPath + "/Redditch";
  readonly public static string cardStaffordPath = cardLocationsPath + "/Stafford";
  readonly public static string cardStokeOnTrentPath = cardLocationsPath + "/StokeOnTrent";
  readonly public static string cardStonePath = cardLocationsPath + "/Stone";
  readonly public static string cardTamworthPath = cardLocationsPath + "/Tamworth";
  readonly public static string cardUttoxeterPath = cardLocationsPath + "/Uttoxeter";
  readonly public static string cardWalsallPath = cardLocationsPath + "/Walsall";
  readonly public static string cardWildLocationPath = cardLocationsPath + "/WildLocation";
  readonly public static string cardWolverhamptonPath = cardLocationsPath + "/Wolverhampton";
  readonly public static string cardWorchesterPath = cardLocationsPath + "/Worchester";

  readonly public static string[] allLocationCards = { cardBelperPath, cardBirminghamPath, cardCannockPath, cardCoalbrookdalePath, cardCoventryPath,
    cardDerbyPath, cardDudleyPath, cardKidderminsterPath, cardLeekPath, cardNuneatonPath, cardRedditchPath, cardStaffordPath, cardStokeOnTrentPath,
    cardStonePath, cardTamworthPath, cardUttoxeterPath, cardWalsallPath, cardWolverhamptonPath, cardWorchesterPath };


  // Card quantities
  readonly public static int[] industryCardsAmount2Players = { 5, 2, 0, 4, 0, 2 };
  readonly public static int[] industryCardsAmount3Players = { 5, 2, 6, 4, 6, 2 };
  readonly public static int[] industryCardsAmount4Players = { 5, 3, 8, 4, 8, 3 };

  readonly public static int[] locationCardsAmount2Players = { 0, 3, 2, 3, 3,
                                                              0, 2, 2, 0, 1, 1, 2, 2, 0,
                                                              0, 1, 0, 1, 2, 2 };
  readonly public static int[] locationCardsAmount3Players = { 0, 3, 2, 3, 3,
                                                              0, 2, 2, 2, 1, 1, 2, 2, 3,
                                                              2, 1, 1, 1, 2, 2 };
  readonly public static int[] locationCardsAmount4Players = { 2, 3, 2, 3, 3,
                                                              3, 2, 2, 2, 1, 1, 2, 2, 3, 
                                                              2, 1, 2, 1, 2, 2 };
  readonly public static int wildLocationCardsAmount = 4;
  readonly public static int wildIndustryCardsAmount = 4;

  



  //Characters
  readonly public static string charactersPath = "Characters";
  readonly public static string redCharPath = charactersPath + "/Red";
  readonly public static string yellowCharPath = charactersPath + "/Yellow";
  readonly public static string whiteCharPath = charactersPath + "/White";
  readonly public static string purpleCharPath = charactersPath + "/Purple";
  readonly public static string[] allCharPaths = { redCharPath, yellowCharPath, whiteCharPath, purpleCharPath };
  readonly public static string charactersParentName = "Character spaces";

  //Money
  readonly public static string moneyPath = "Money";
  readonly public static string money15Path = moneyPath + "/15";
  readonly public static string money10Path = moneyPath + "/10";
  readonly public static string money5Path = moneyPath + "/5";

  //Merchants
  readonly public static string merchantsPath = "Merchants";
  readonly public static string merchantAllPath = merchantsPath + "/All";
  readonly public static string merchantCottonMill4Path = merchantsPath + "/CottonMill4Player";
  readonly public static string merchantCottonMillPath = merchantsPath + "/CottonMillAll";
  readonly public static string merchantEmpty3Path = merchantsPath + "/Empty3Player";
  readonly public static string merchantEmptyPath = merchantsPath + "/EmptyAll";
  readonly public static string merchantManufacturer4Path = merchantsPath + "/Manufacturer4Player";
  readonly public static string merchantManufacturerPath = merchantsPath + "/ManufacturerAll";
  readonly public static string merchantPotteryPath = merchantsPath + "/Pottery";
  readonly public static string[] allMerchantTilesPaths = { merchantAllPath, merchantCottonMill4Path,
    merchantCottonMillPath, merchantEmpty3Path, merchantEmptyPath, merchantEmptyPath, merchantManufacturer4Path,
    merchantManufacturerPath, merchantPotteryPath };

  //Tokens
  readonly public static string tokensPath = "Tokens";
  readonly public static string victoryYellowTokenPath = tokensPath + "/EdgyYellow";
  readonly public static string victoryRedTokenPath = tokensPath + "/EdgyRed";
  readonly public static string victoryWhiteTokenPath = tokensPath + "/EdgyWhite";
  readonly public static string victoryPurpleTokenPath = tokensPath + "/EdgyPurple";
  readonly public static string incomeYellowTokenPath = tokensPath + "/RoundYellow";
  readonly public static string incomeRedTokenPath = tokensPath + "/RoundRed";
  readonly public static string incomeWhiteTokenPath = tokensPath + "/RoundWhite";
  readonly public static string incomePurpleTokenPath = tokensPath + "/RoundPurple";
  readonly public static string[] allVictoryTokes = { victoryRedTokenPath, victoryYellowTokenPath, victoryWhiteTokenPath, victoryPurpleTokenPath };
  readonly public static string[] allIncomeTokens = { incomeRedTokenPath, incomeYellowTokenPath, incomeWhiteTokenPath, incomePurpleTokenPath};

  readonly public static Vector3[] playerTokenOffsets = { new Vector3(-0.03f, 0, 0), new Vector3(-0.01f, 0, 0), new Vector3(0.01f, 0, 0), new Vector3(0.03f, 0, 0) };

  readonly public static Vector3 victoryTokenOffset = new Vector3(0.1f, 0.1f, 0);
  readonly public static Vector3 incomeTokenOffset = new Vector3(0.1f, 0, 0);


  //Resources
  readonly public static string resourcesPath = "";
  readonly public static string barrelPath = resourcesPath + "Barrel";
  readonly public static string coalPath = resourcesPath + "Coal";
  readonly public static string ironPath = resourcesPath + "Iron";
  readonly public static string ironSmallPath = resourcesPath + "Iron small";
  readonly public static string coalSmallPath = resourcesPath + "Coal small";
  readonly public static string ironStorageName = "Iron storage";
  readonly public static string coalStorageName = "Coal storage";



  //PlayerTiles
  readonly public static string tilesPath = "Tiles";

  //playerDifference
  readonly public static string redTilesPath = tilesPath + "/Red";
  readonly public static string redUpgradedTilesPath = tilesPath + "/RedUp";
  readonly public static string yellowTilesPath = tilesPath + "/Yellow";
  readonly public static string yellowUpgradedTilesPath = tilesPath + "/YellowUp";
  readonly public static string whiteTilesPath = tilesPath + "/White";
  readonly public static string whiteUpgradedTilesPath = tilesPath + "/WhiteUp";
  readonly public static string purpleTilesPath = tilesPath + "/Purple";
  readonly public static string purpleUpgradedTilesPath = tilesPath + "/PurpleUp";

  //globalBuildings
  readonly public static string[] breweryNames = { "/Brewery1", "/Brewery2", "/Brewery3", "/Brewery4" };
  readonly public static string[] coalMineNames = { "/CoalMine1", "/CoalMine2", "/CoalMine3", "/CoalMine4" };
  readonly public static string[] cottonMillNames = { "/CottonMill1", "/CottonMill2", "/CottonMill3", "/CottonMill4" };
  readonly public static string[] ironWorksNames = { "/IronWorks1", "/IronWorks2", "/IronWorks3", "/IronWorks4", };
  readonly public static string[] manufacturerNames = { "/Manufacturer1", "/Manufacturer2", "/Manufacturer3", "/Manufacturer4", "/Manufacturer5", "/Manufacturer6", "/Manufacturer7", "/Manufacturer8" };
  readonly public static string[] potteryNames = { "/Pottery1", "/Pottery2", "/Pottery3", "/Pottery4", "/Pottery5" };
  readonly public static string trainName = "/Train";
  readonly public static string boatName = "/Boat";

  //Tile quantities

  readonly public static int[] breweryAmounts = { 2, 2, 2, 1 };

  readonly public static int[] cottonMillAmounts = { 3, 2, 3, 3 };

  readonly public static int[] ironWorksAmounts = { 1, 1, 1, 1 };

  readonly public static int[] coalMineAmounts = { 1, 2, 2, 2 };

  readonly public static int[] potteryAmounts = { 1, 1, 1, 1, 1 };

  readonly public static int[] manufacturerAmounts = { 1, 2, 1, 1, 2, 1, 1, 2 };

  //Tile costs

  readonly public static int[] breweryCosts = { 5, 7, 9, 9 };

  readonly public static int[] cottonMillCosts = { 12, 14, 16, 18 };

  readonly public static int[] ironWorksCosts = { 5, 7, 9, 12 };

  readonly public static int[] coalMineCosts = { 5, 7, 8, 10 };

  readonly public static int[] potteryCosts = { 17, 0, 22, 0, 24 };

  readonly public static int[] manufacturerCosts = { 8, 10, 12, 14, 16, 20, 16, 20 };


  //Tile resource quantities

  readonly public static int[] ironWorksIronCount = { 4,4,5,6 };

  readonly public static int[] coalMineCoalCount = { 2,3,4,5 };

  //Tile build requirements
  //  iron
  readonly public static int[] breweryBuildIronReq = { 1,1,1,1 };

  readonly public static int[] cottonMillBuildIronReq = { 0,0,1,1 };

  readonly public static int[] ironWorksBuildIronReq = { 0,0,0,0 };

  readonly public static int[] coalMineBuildIronReq = { 0,0,1,1 };

  readonly public static int[] potteryBuildIronReq = { 1,0,0,0,0 };

  readonly public static int[] manufacturerBuildIronReq = { 0,1,0,1,0,0,1,2 };

  //  coal
  readonly public static int[] breweryBuildCoalReq = { 0,0,0,0 };

  readonly public static int[] cottonMillBuildCoalReq = { 0,1,1,1 };

  readonly public static int[] ironWorksBuildCoalReq = { 1,1,1,1 };

  readonly public static int[] coalMineBuildCoalReq = { 0,0,0,0 };

  readonly public static int[] potteryBuildCoalReq = { 0,1,2,1,2 };

  readonly public static int[] manufacturerBuildCoalReq = { 1,0,2,0,1,0,1,0 };

  //Tile upgrade rewards

  //income
  readonly public static int[] breweryUpIncomeRewards = { 4, 5, 5, 5 };

  readonly public static int[] cottonMillUpIncomeRewards = { 5,4,3,2 };

  readonly public static int[] ironWorksUpIncomeRewards = { 3,3,2,1 };

  readonly public static int[] coalMineUpIncomeRewards = { 4,7,6,5 };

  readonly public static int[] potteryUpIncomeRewards = { 5,1,5,1,5 };

  readonly public static int[] manufacturerUpIncomeRewards = { 5,1,4,6,2,6,4,1 };

  //victory points
  //  for upgrade itself
  readonly public static int[] breweryUpVicPtsRewards = { 4, 5, 7, 10 };

  readonly public static int[] cottonMillUpVicPtsRewards = { 5,5,9,12 };

  readonly public static int[] ironWorksUpVicPtsRewards = { 3,5,7,9 };

  readonly public static int[] coalMineUpVicPtsRewards = { 1,2,3,4 };

  readonly public static int[] potteryUpVicPtsRewards = { 10,1,11,1,20 };

  readonly public static int[] manufacturerUpVicPtsRewards = { 3,5,4,3,8,7,9,11 };

  //  for being next to network

  readonly public static int[] breweryUpNetVicPtsRewards = { 2,2,2,2 };

  readonly public static int[] cottonMillUpNetVicPtsRewards = { 1,2,1,1 };

  readonly public static int[] ironWorksUpNetVicPtsRewards = { 1,1,1,1 };

  readonly public static int[] coalMineUpNetVicPtsRewards = { 2,1,1,1 };

  readonly public static int[] potteryUpNetVicPtsRewards = { 1,1,1,1,1 };

  readonly public static int[] manufacturerUpNetVicPtsRewards = { 2,1,0,1,2,1,0,1 };


  //Border for object highlighting

  readonly public static string tileBorderPath = "Tile Border";
  readonly public static string cardBorderPath = "Card Border";
  readonly public static string barrelBorderPath = "Merch Barrel Border";
  readonly public static string ironStorageBorderPath = "Iron Storage Border";
  readonly public static string coalStorageBorderPath = "Coal Storage Border";

  readonly public static string HUDPath = "HUD";
  readonly public static string mainInfoLabelPath = "InfoLabel";
  readonly public static string problemLabelName = "CurrentProblem";


  //Scene object names

  //Playing boards
  readonly public static string mainBoardName = "Main Board";
  readonly public static string player1PersonalBoardName = "Personal board - Player 1";
  readonly public static string player2PersonalBoardName = "Personal board - Player 2";
  readonly public static string player3PersonalBoardName = "Personal board - Player 3";
  readonly public static string player4PersonalBoardName = "Personal board - Player 4";
  readonly public static string[] playerPersonalBoardNames = { player1PersonalBoardName, player2PersonalBoardName, player3PersonalBoardName, player4PersonalBoardName };

  readonly public static string helpBoardName = "Help Board";

  readonly public static string midEraScreenName = "EraChangeScreen";

  readonly public static string boardBackgroundName = "Background";


  readonly public static string personalBoardHandName = "Hand";
  readonly public static string personalBoardDiscardName = "Discard";

  readonly public static string personalBoardNameLabelName = "PlayerName";
  readonly public static string personalBoardMoneyLabelName = "Money";
  readonly public static string personalBoardIncomeLabelName = "Income";
  readonly public static string personalBoardIncomePointsLabelName = "IncomePoints";
  readonly public static string personalBoardVictoryPointsLabelName = "VictoryPoints";


  //Industries
  readonly public static string IndustrySpacesOnPersonalBoardParentName = "IndustrySpaces";
  readonly public static string breweriesParentName = "Breweries";
  readonly public static string manufacturersParentName = "Manufacturers";
  readonly public static string cottonMillsParentName = "CottonMills";
  readonly public static string potteriesParentName = "Potteries";
  readonly public static string ironWorksParentName = "IronWorks";
  readonly public static string coalMinesParentName = "CoalMines";

  //Network
  readonly public static string boatNetworkParentName = "Network Boat";
  readonly public static string trainNetworkParentName = "Network Train";


  //Card decks
  readonly public static string drawDeckInMainBoardName = "Draw deck";
  readonly public static string wildIndustryDrawDeckInMainBoardName = "Wild industry deck";
  readonly public static string wildLocationDrawDeckInMainBoardName = "Wild location deck";

  //Cards boards
  readonly public static string player1HandBoardName = "Hand Preview - Player 1";
  readonly public static string player2HandBoardName = "Hand Preview - Player 2";
  readonly public static string player3HandBoardName = "Hand Preview - Player 3";
  readonly public static string player4HandBoardName = "Hand Preview - Player 4";
  readonly public static string[] playerHandBoardNames = { player1HandBoardName, player2HandBoardName, player3HandBoardName, player4HandBoardName };


  readonly public static string player1DiscardBoardName = "Discard Preview - Player 1";
  readonly public static string player2DiscardBoardName = "Discard Preview - Player 2";
  readonly public static string player3DiscardBoardName = "Discard Preview - Player 3";
  readonly public static string player4DiscardBoardName = "Discard Preview - Player 4";
  readonly public static string[] playerDiscardBoardNames = { player1DiscardBoardName, player2DiscardBoardName, player3DiscardBoardName, player4DiscardBoardName };


  //VP spaces
  readonly public static string VPSpacesName = "VP spaces";

  //Buttons

  readonly public static string cancelButtonName = "Cancel";
  readonly public static string changerEraReadyButtonName = "EraChangeReady";

}
