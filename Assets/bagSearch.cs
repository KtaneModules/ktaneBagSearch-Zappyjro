//Sorry about this horrible spaghetti mess XD

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using Newtonsoft.Json;
using KModkit;

public class bagSearch : MonoBehaviour {
	public KMAudio Audio;
	public KMBombModule Module;
	public KMBombInfo Info;
	public KMModSettings modSettings;
	public KMSelectable moduleSelectable;
	public KMSelectable[] itemButtons;
	public KMSelectable detainButton, boardButton, xrayButton;
	public TextMesh airlineScreen, flightRouteScreen, weightScreen, numberScreen;
	public GameObject xrayMachine;
	public MeshRenderer[] itemButtonRenderers;
	public MeshRenderer[] suitcase;
	public Material[] itemMaterials;
	public Material[] suitcaseMaterials;
	public KMColorblindMode ColourBlindMode;
	public TextMesh colourText;
	public GameObject colourObject;

	private static int _moduleIdCounter = 1;
	private int _moduleId = 0;

	private string[] airlines = new string[3];
	private int[] destinations = new int[6];
	private int[] itemsBagOne = new int[6];
	private int[] itemsBagTwo = new int[6];
	private int[] itemsBagThree = new int[6];
	private int[] currentBagArray;
	private bool[] airlinesFake = new bool[3];
	private int currentBag = 0;
	private int[] weights = new int[3];
	private int[] flightLengths = new int[3];
	private int[] colours = new int[3];
	private bool colourBlindMode;

	private string[] realAirlines = {
		"SparkJet Airways",
		"BlastAir",
		"InfernoFly",
		"WhizBang Airlines",
		"DynamitePlus Airways",
		"PopAir Travel",
		"BoomSky Express",
		"FRQandU"
	};
	private string[] fakeAirlines = {
		"SparkJets Airway",
		"BlastAir Flights",
		"InfernoFlight",
		"WhizzBang Airline",
		"Dynamight+ Airways",
		"PopAir Travelling",
		"BoomSky Direct",
		"FRKandThey"
	};
	private string[] destinationNames = { "ALG", "VJK", "JFR", "RPL", "CKD", "DPO", "KFL", "DJC" };
	private string[] itemNames = {
		"Empty",
		"Passport",
		"Star Badge",
		"Ball",
		"Bone",
		"Flashlight",
		"Game Controller",
		"Gun",
		"Handcuffs",
		"Knife",
		"Money",
		"Nail Polish",
		"Scales",
		"Shirt",
		"Tie",
		"Wine Bottle",
		"Wire"
	};

	private bool _isSolved = false, oneDetained = false, twoDetained = false, threeDetained = false, xrayOn = false;

	private bool[] shouldBeConfiscatedOne = { false, false, false, false, false, false };
	private bool[] shouldBeConfiscatedTwo = { false, false, false, false, false, false };
	private bool[] shouldBeConfiscatedThree = { false, false, false, false, false, false };

	// Use this for initialization
	void Start () {


		xrayRemoveChildren ();

		_moduleId = _moduleIdCounter++;

		if (ColourBlindMode.ColorblindModeActive) {
			colourBlindMode = true;
		}


		//Random bag weights and colours
		for (int i = 0; i < 3; i++) {
			int j = i;
			weights [j] = Random.Range (1, 40);
			colours [j] = Random.Range (0, 6);
		}

		colourObject.gameObject.SetActive (false);
		if (colourBlindMode) {
			string[] coloursBlind = { "Red", "Blue", "Yellow", "Pink", "Green", "Orange", "Brown" };
			colourObject.gameObject.SetActive (true);
			colourText.text = coloursBlind [colours [0]];
		}

		//Choosing the airlines
		for (int i = 0; i < 3; i++) {
			int j = i;
			int fakeChance = Random.Range (0, 100);
			if (fakeChance < 85) {
				airlinesFake [j] = false;
				airlines [j] = realAirlines [Random.Range (0, realAirlines.Length)];
			} else {
				airlinesFake [j] = true;
				airlines [j] = fakeAirlines [Random.Range (0, fakeAirlines.Length)];
			}
		}

		//Choosing the destinations
		destinations[0] = Random.Range(0,7);
		for (int i = 1; i < 6; i++) {
			int j = i;
			destinations [j] = Random.Range (0, 7);
			while (destinations [j] == destinations [(j - 1)]) {
				destinations [j] = Random.Range (0, 7);
			}
		}

		//Flight Distances
		for (int i = 1; i < 4; i++) {
			int j = i;
			if (destinations [(2 * j) - 1] == 0 || destinations [(2 * j) - 2] == 0) {
				if (destinations [(2 * j) - 1] == 1 || destinations [(2 * j) - 2] == 1) {
					flightLengths [j - 1] = 9;
				} else if (destinations [(2 * j) - 1] == 2 || destinations [(2 * j) - 2] == 2) {
					flightLengths [j - 1] = 8;
				} else if (destinations [(2 * j) - 1] == 3 || destinations [(2 * j) - 2] == 3) {
					flightLengths [j - 1] = 3;
				} else if (destinations [(2 * j) - 1] == 4 || destinations [(2 * j) - 2] == 4) {
					flightLengths [j - 1] = 11;
				} else if (destinations [(2 * j) - 1] == 5 || destinations [(2 * j) - 2] == 5) {
					flightLengths [j - 1] = 9;
				} else if (destinations [(2 * j) - 1] == 6 || destinations [(2 * j) - 2] == 6) {
					flightLengths [j - 1] = 10;
				} else if (destinations [(2 * j) - 1] == 7 || destinations [(2 * j) - 2] == 7) {
					flightLengths [j - 1] = 1;
				}
			} else if (destinations [(2 * j) - 1] == 1 || destinations [(2 * j) - 2] == 1) {
				if (destinations [(2 * j) - 1] == 2 || destinations [(2 * j) - 2] == 2) {
					flightLengths [j - 1] = 4;
				} else if (destinations [(2 * j) - 1] == 3 || destinations [(2 * j) - 2] == 3) {
					flightLengths [j - 1] = 21;
				} else if (destinations [(2 * j) - 1] == 4 || destinations [(2 * j) - 2] == 4) {
					flightLengths [j - 1] = 3;
				} else if (destinations [(2 * j) - 1] == 5 || destinations [(2 * j) - 2] == 5) {
					flightLengths [j - 1] = 6;
				} else if (destinations [(2 * j) - 1] == 6 || destinations [(2 * j) - 2] == 6) {
					flightLengths [j - 1] = 5;
				} else if (destinations [(2 * j) - 1] == 7 || destinations [(2 * j) - 2] == 7) {
					flightLengths [j - 1] = 2;
				}
			} else if (destinations [(2 * j) - 1] == 2 || destinations [(2 * j) - 2] == 2) {
				if (destinations [(2 * j) - 1] == 3 || destinations [(2 * j) - 2] == 3) {
					flightLengths [j - 1] = 4;
				} else if (destinations [(2 * j) - 1] == 4 || destinations [(2 * j) - 2] == 4) {
					flightLengths [j - 1] = 1;
				} else if (destinations [(2 * j) - 1] == 5 || destinations [(2 * j) - 2] == 5) {
					flightLengths [j - 1] = 3;
				} else if (destinations [(2 * j) - 1] == 6 || destinations [(2 * j) - 2] == 6) {
					flightLengths [j - 1] = 9;
				} else if (destinations [(2 * j) - 1] == 7 || destinations [(2 * j) - 2] == 7) {
					flightLengths [j - 1] = 4;
				}
			} else if (destinations [(2 * j) - 1] == 3 || destinations [(2 * j) - 2] == 3) {
				if (destinations [(2 * j) - 1] == 4 || destinations [(2 * j) - 2] == 4) {
					flightLengths [j - 1] = 6;
				} else if (destinations [(2 * j) - 1] == 5 || destinations [(2 * j) - 2] == 5) {
					flightLengths [j - 1] = 9;
				} else if (destinations [(2 * j) - 1] == 6 || destinations [(2 * j) - 2] == 6) {
					flightLengths [j - 1] = 5;
				} else if (destinations [(2 * j) - 1] == 7 || destinations [(2 * j) - 2] == 7) {
					flightLengths [j - 1] = 15;
				}
			} else if (destinations [(2 * j) - 1] == 4 || destinations [(2 * j) - 2] == 4) {
				if (destinations [(2 * j) - 1] == 5 || destinations [(2 * j) - 2] == 5) {
					flightLengths [j - 1] = 6;
				} else if (destinations [(2 * j) - 1] == 6 || destinations [(2 * j) - 2] == 6) {
					flightLengths [j - 1] = 8;
				} else if (destinations [(2 * j) - 1] == 7 || destinations [(2 * j) - 2] == 7) {
					flightLengths [j - 1] = 20;
				}
			} else if (destinations [(2 * j) - 1] == 5 || destinations [(2 * j) - 2] == 5) {
				if (destinations [(2 * j) - 1] == 6 || destinations [(2 * j) - 2] == 6) {
					flightLengths [j - 1] = 4;
				} else if (destinations [(2 * j) - 1] == 7 || destinations [(2 * j) - 2] == 7) {
					flightLengths [j - 1] = 4;
				}
			} else {
				flightLengths [j - 1] = 20;
			}

		}

		//Choosing the items - 1
		bool starInBag = false;
		int passportChance = Random.Range(0,100);
		if (passportChance < 85) {
			itemsBagOne [Random.Range (1, 6)] = 1;
		} else {
			if (!(airlines [0] == "InfernoFly")) {
				oneDetained = true;
			}
		}
		for (int i = 0; i < 6; i++) {
			int j = i;
			if (itemsBagOne [j] != 1) {
				int emptyChance = Random.Range (0, 100);
				if (emptyChance < 85) {
					if (!starInBag) {
						itemsBagOne [j] = Random.Range (2, 17);
						if (itemsBagOne[j] == 2) {
							starInBag = true;
						}
					} else {
						itemsBagOne [j] = Random.Range (3, 17);
					}
				} else {
					itemsBagOne [j] = 0;
				}
			}
		}
		//Choosing the items - 2
		starInBag = false;
		passportChance = Random.Range(0,100);
		if (passportChance < 85) {
			itemsBagTwo [Random.Range (1, 6)] = 1;
		} else {
			if (!(airlines [1] == "InfernoFly")) {
				twoDetained = true;
			}
		}
		for (int i = 0; i < 6; i++) {
			int j = i;
			if (itemsBagTwo [j] != 1) {
				int emptyChance = Random.Range (0, 100);
				if (emptyChance < 85) {
					if (!starInBag) {
						itemsBagTwo [j] = Random.Range (2, 17);
						if (itemsBagTwo [j] == 2) {
							starInBag = true;
						}
					} else {
						itemsBagTwo [j] = Random.Range (3, 17);
					}
				} else {
					itemsBagTwo [j] = 0;
				}
			}
		}
		//Choosing the items - 3
		starInBag = false;
		passportChance = Random.Range(0,100);
		if (passportChance < 85) {
			itemsBagThree [Random.Range (1, 6)] = 1;
		} else {
			if (!(airlines [2] == "InfernoFly")) {
				threeDetained = true;
			}
		}
		for (int i = 0; i < 6; i++) {
			int j = i;
			if (itemsBagThree [j] != 1) {
				int emptyChance = Random.Range (0, 100);
				if (emptyChance < 85) {
					if (!starInBag) {
						itemsBagThree [j] = Random.Range (2, 17);
						if (itemsBagThree [j] == 2) {
							starInBag = true;
						}
					} else {
						itemsBagThree [j] = Random.Range (3, 17);
					}
				} else {
					itemsBagThree [j] = 0;
				}
			}
		}

		//Testing items & rules - Bag 1
		bool starbadge = false;
		int starPos = 0;
		for (int i = 0; i < 6; i++) {
			int j = i;
			if (itemsBagOne [j] == 0) {
				itemButtonRenderers[j].gameObject.SetActive (false);
				moduleSelectable.Children [j] = null;
				moduleSelectable.UpdateChildrenProperly (xrayButton);


			}
			if (itemsBagOne [j] == 1 || itemsBagOne [j] == 4) {
				if (airlines [0] == "BoomSky Express") {
					shouldBeConfiscatedOne [j] = true;
				}
			}
			//ball
			if (itemsBagOne [j] == 3) {
				if (airlines [0] == "BlastAir" || flightLengths [0] > 12) {
					shouldBeConfiscatedOne [j] = true;
				}
			}
			//flashlight
			else if (itemsBagOne [j] == 5) {
				if ((Info.GetOnIndicators ().Count () - Info.GetOffIndicators ().Count ()) >= 2) {
					shouldBeConfiscatedOne [j] = true;
				} else { 
					if (airlines [0] == "PopAir Travel") {
						shouldBeConfiscatedOne [j] = true;
					}
				}
			}
			//gamecontroller
			else if (itemsBagOne [j] == 6) {
				if (airlines [0] == "SparkJet Airways" || !(Info.GetSerialNumberLetters ().Any ("ABXY".Contains))) {
					shouldBeConfiscatedOne [j] = true;
				}
			}
			//gun
			else if (itemsBagOne [j] == 7) {
				if (!(airlines [0] == "PopAir Travel")) {
					if (flightLengths [0] < 8) {
						shouldBeConfiscatedOne [j] = true;
					} else {
						oneDetained = true;
					}
				}
			}
			//handcuffs
			else if (itemsBagOne [j] == 8) {
				if (airlines [0] == "WhizBang Airlines") {
					shouldBeConfiscatedOne [j] = true;
				}
				oneDetained = true; //change this for 2 and 3
			}
			//knife
			else if (itemsBagOne [j] == 9) {
				if (!(airlines [0] == "PopAir Travel")) {
					shouldBeConfiscatedOne [j] = true;
				}
				//different in 2 and 3
			}
			//nail polish
			else if (itemsBagOne [j] == 11) {
				if (colours [0] <= 2 || airlines[0] == "PopAir Travel") {
					shouldBeConfiscatedOne [j] = true;
				}
			}
			//scales
			else if (itemsBagOne [j] == 12) {
				if (airlines [0] == "BlastAir") {
					shouldBeConfiscatedOne [j] = true;
				}
				//this will have more steps on 2 or 3
			}
			//shirt
			else if (itemsBagOne [j] == 13) {
				if (airlines [0] == "DynamitePlus Airways") {
					shouldBeConfiscatedOne [j] = true;
				}
				
			}
			//tie
			else if (itemsBagOne [j] == 14) {
				shouldBeConfiscatedOne [j] = true;
				for (int q = 0; q < 6; q++) {
					if (itemsBagOne [q] == 13) {
						shouldBeConfiscatedOne [j] = false;
					}
				}
				if (airlines [0] == "DynamitePlus Airways") {
					shouldBeConfiscatedOne [j] = true;
				}
			}
			//wine
			else if (itemsBagOne [j] == 15) {
				if (Info.IsIndicatorPresent ("CAR") || airlines[0] == "InfernoFly") {
					shouldBeConfiscatedOne [j] = true;
				}
			}
			//wire
			else if (itemsBagOne [j] == 16) {
				if (Info.GetPortCount() < 3 || airlines[0] == "SparkJet Airways") {
					shouldBeConfiscatedOne [j] = true;
				}
				if (Info.GetPortCount() == 0) {
					oneDetained = true;
				}
			}
			//star badge
			else if (itemsBagOne [j] == 2) {
				if (airlines [0] == "WhizBang Airlines") {
					shouldBeConfiscatedOne [j] = true;
				} else {
					starbadge = true;
					starPos = j;
				}
			}
		}
		//money
		for (int i = 0; i < 6; i++) {
			int j = i;
			if (itemsBagOne [j] == 10) {
				shouldBeConfiscatedOne [j] = true;
				for (int q = 0; q < 6; q++) {
					if (shouldBeConfiscatedOne [q] && (q != j)) {
						shouldBeConfiscatedOne [j] = false;
					}
				}
				if (airlines [0] == "InfernoFly") {
					shouldBeConfiscatedOne [j] = true;
				}
			}
		}
		if (airlines [0] == "SparkJet Airways") {
			if (oneDetained) {
				oneDetained = false;
				for (int i = 0; i < 6; i++) {
					int j = i;
					shouldBeConfiscatedOne [j] = true;
				}
			}
		}
		if (airlines [0] == "BlastAir" && Info.GetBatteryCount () > 2) {
			shouldBeConfiscatedOne [5] = true;
		}
		if (airlines [0] == "DynamitePlus Airways") {
			for (int i = 0; i < 3; i++) {
				int j = i;
				shouldBeConfiscatedOne [j] = false;
			}
		}
		if (airlines [0] == "BoomSky Express") {
			if (weights [0] == 2 || weights [0] == 3 || weights [0] == 5 || weights [0] == 7 || weights [0] == 11 || weights [0] == 13 || weights [0] == 17 || weights [0] == 19 || weights [0] == 23 || weights [0] == 29 || weights [0] == 31 || weights [0] == 37) {
				for (int i = 0; i < 3; i++) {
					int j = i;
					shouldBeConfiscatedOne [j] = true;
				}
			}
		}
		if (starbadge) {
			if (starPos < 3) {
				for (int i = 0; i < 3; i++) {
					int j = i;
					shouldBeConfiscatedOne [j] = false;
				}
			} else {
				for (int i = 0; i < 3; i++) {
					int j = i + 3;
					shouldBeConfiscatedOne [j] = false;
				}
			}
		}
		for (int i = 0; i < 6; i++) {
			int j = i;
			if (itemsBagOne [j] == 0) {
				shouldBeConfiscatedOne [j] = false;
			}
		}

		//Setting screen text and bag colour
		suitcase[0].material = suitcaseMaterials[colours[0]];
		airlineScreen.text = airlines[0];
		flightRouteScreen.text = destinationNames [destinations[0]] + " -> " + destinationNames [destinations[1]];
		weightScreen.text = weights [0].ToString() + "kg";

		//Assigning item textures
		currentBagArray = itemsBagOne;
		for (int i = 0; i < itemButtonRenderers.Length; i++) {
			itemButtonRenderers [i].material = itemMaterials [itemsBagOne [i]];
		}
		Debug.LogFormat("[Bag Search #{0}] Bag 1 contains: {1}, {2}, {3}, {4}, {5}, {6}.", _moduleId, itemNames[itemsBagOne[0]], itemNames[itemsBagOne[1]], itemNames[itemsBagOne[2]], itemNames[itemsBagOne[3]], itemNames[itemsBagOne[4]], itemNames[itemsBagOne[5]]);
		int count = 0;
		for (int i = 0; i < 6; i++) {
			int j = i;
			if (shouldBeConfiscatedOne [j]) {
				count++;
			}
		}
		if (oneDetained) {
			Debug.LogFormat ("[Bag Search #{0}] Bag 1 should be detained.", _moduleId);
		} else {
			Debug.LogFormat ("[Bag Search #{0}] Bag 1 should be allowed to board.", _moduleId);
		}
		Debug.LogFormat("[Bag Search #{0}] Bag 1 contains: {1} item(s) should be confiscated total.", _moduleId, count);
		Debug.LogFormat("[Bag Search #{0}] The item positions which should be confiscated are {1}, {2}, {3}, {4}, {5}, {6}.", _moduleId, shouldBeConfiscatedOne[0], shouldBeConfiscatedOne[1], shouldBeConfiscatedOne[2], shouldBeConfiscatedOne[3], shouldBeConfiscatedOne[4], shouldBeConfiscatedOne[5]);

		//Button delegation
		detainButton.OnInteract += delegate () {
			handleDetain ();
			return false;
		};

		xrayButton.OnInteract += delegate () {
			handleXray ();
			return false;
		};

		boardButton.OnInteract += delegate () {
			handleBoard ();
			return false;
		};

		itemButtons [0].OnInteract += delegate {
			handleConfiscate (0);
			return false;
		};
		itemButtons [1].OnInteract += delegate {
			handleConfiscate (1);
			return false;
		};
		itemButtons [2].OnInteract += delegate {
			handleConfiscate (2);
			return false;
		};
		itemButtons [3].OnInteract += delegate {
			handleConfiscate (3);
			return false;
		};
		itemButtons [4].OnInteract += delegate {
			handleConfiscate (4);
			return false;
		};
		itemButtons [5].OnInteract += delegate {
			handleConfiscate (5);
			return false;
		};
	}
	
	// Update is called once per frame
	void Update () {
		xrayMachine.gameObject.SetActive (xrayOn);
	}

	void bagTwoSetup (){
		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, Module.transform);
		for (int i = 0; i < 6; i++) {
			int j = i;
			itemButtons [j].gameObject.SetActive (true);
		}
		xrayOn = false;
		xrayRemoveChildren ();
		numberScreen.text = "2";
		currentBagArray = itemsBagTwo;
		currentBag = 1;

		colourObject.gameObject.SetActive (false);
		if (colourBlindMode) {
			string[] coloursBlind = { "Red", "Blue", "Yellow", "Pink", "Green", "Orange", "Brown" };
			colourObject.gameObject.SetActive (true);
			colourText.text = coloursBlind [colours [1]];
		}

		//START

		bool starbadge = false;
		int starPos = 0;
		for (int i = 0; i < 6; i++) {
			int j = i;
			if (itemsBagTwo [j] == 0) {
				itemButtonRenderers[j].gameObject.SetActive (false);
				moduleSelectable.Children [j] = null;
				moduleSelectable.UpdateChildrenProperly (xrayButton);
			}
			if (itemsBagTwo [j] == 1 || itemsBagTwo [j] == 4) {
				if (airlines [1] == "BoomSky Express") {
					shouldBeConfiscatedOne [j] = true;
				}
			}
			//ball
			if (itemsBagTwo [j] == 3) {
				if (airlines [1] == "BlastAir" || flightLengths [1] > 12) {
					shouldBeConfiscatedTwo [j] = true;
				}
			}
			//flashlight
			else if (itemsBagTwo [j] == 5) {
				if ((Info.GetOnIndicators ().Count () - Info.GetOffIndicators ().Count ()) >= 2) {
					shouldBeConfiscatedTwo [j] = true;
				} else { 
					if (airlines [1] == "PopAir Travel") {
						shouldBeConfiscatedTwo [j] = true;
					}
				}
			}
			//gamecontroller
			else if (itemsBagTwo [j] == 6) {
				if (airlines [1] == "SparkJet Airways" || !(Info.GetSerialNumberLetters ().Any ("ABXY".Contains))) {
					shouldBeConfiscatedTwo [j] = true;
				}
			}
			//gun
			else if (itemsBagTwo [j] == 7) {
				if (!(airlines [1] == "PopAir Travel")) {
					if (flightLengths [1] < 8) {
						shouldBeConfiscatedTwo [j] = true;
					} else {
						twoDetained = true;
					}
				}
			}
			//handcuffs
			else if (itemsBagTwo [j] == 8) {
				if (airlines [1] == "WhizBang Airlines") {
					shouldBeConfiscatedTwo [j] = true;
				}
				if (!oneDetained){
					twoDetained = true; //change this for 3
				}
			}
			//knife
			else if (itemsBagTwo [j] == 9) {
				if (!(airlines [1] == "PopAir Travel")) {
					twoDetained = true;
				}
			}
			//nail polish
			else if (itemsBagTwo [j] == 11) {
				if (colours [1] <= 2 || airlines[1] == "PopAir Travel") {
					shouldBeConfiscatedTwo [j] = true;
				}
			}
			//scales
			else if (itemsBagTwo [j] == 12) {
				if (airlines [1] == "BlastAir") {
					shouldBeConfiscatedTwo [j] = true;
				}
				if (weights [1] > weights [0]) {
					shouldBeConfiscatedTwo [j] = true;
				}
			}
			//shirt
			else if (itemsBagTwo [j] == 13) {
				if (airlines [1] == "DynamitePlus Airways") {
					shouldBeConfiscatedTwo [j] = true;
				}

			}
			//tie
			else if (itemsBagTwo [j] == 14) {
				shouldBeConfiscatedTwo [j] = true;
				for (int q = 0; q < 6; q++) {
					if (itemsBagTwo [q] == 13) {
						shouldBeConfiscatedTwo [j] = false;
					}
				}
				if (airlines [1] == "DynamitePlus Airways") {
					shouldBeConfiscatedTwo [j] = true;
				}
			}
			//wine
			else if (itemsBagTwo [j] == 15) {
				if (Info.IsIndicatorPresent ("CAR") || airlines[1] == "InfernoFly") {
					shouldBeConfiscatedTwo [j] = true;
				}
			}
			//wire
			else if (itemsBagTwo [j] == 16) {
				if (Info.GetPortCount() < 3 || airlines[1] == "SparkJet Airways") {
					shouldBeConfiscatedTwo [j] = true;
				}
				if (Info.GetPortCount() == 0) {
					twoDetained = true;
				}
			}
			//star badge
			else if (itemsBagTwo [j] == 2) {
				if (airlines [1] == "WhizBang Airlines") {
					shouldBeConfiscatedTwo [j] = true;
				} else {
					starbadge = true;
					starPos = j;
				}
			}
		}

		//money
		for (int i = 0; i < 6; i++) {
			int j = i;
			if (itemsBagTwo [j] == 10) {
				shouldBeConfiscatedTwo [j] = true;
				for (int q = 0; q < 6; q++) {
					if (shouldBeConfiscatedTwo [q] && (q != j)) {
						shouldBeConfiscatedTwo [j] = false;
					}
				}
				if (airlines [1] == "InfernoFly") {
					shouldBeConfiscatedTwo [j] = true;
				}
			}
		}
		if (airlines [1] == "SparkJet Airways") {
			if (twoDetained) {
				twoDetained = false;
				for (int i = 0; i < 6; i++) {
					int j = i;
					shouldBeConfiscatedTwo [j] = true;
				}
			}
		}
		if (airlines [1] == "BlastAir" && Info.GetBatteryCount () > 2) {
			shouldBeConfiscatedTwo [5] = true;
		}
		if (airlines [1] == "DynamitePlus Airways") {
			for (int i = 0; i < 3; i++) {
				int j = i;
				shouldBeConfiscatedTwo [j] = false;
			}
		}
		if (airlines [1] == "BoomSky Express") {
			if (weights [1] == 2 || weights [1] == 3 || weights [1] == 5 || weights [1] == 7 || weights [1] == 11 || weights [1] == 13 || weights [1] == 17 || weights [1] == 19 || weights [1] == 23 || weights [1] == 29 || weights [1] == 31 || weights [1] == 37) {
				for (int i = 0; i < 3; i++) {
					int j = i;
					shouldBeConfiscatedTwo [j] = true;
				}
			}
		}
		if (starbadge) {
			if (starPos < 3) {
				for (int i = 0; i < 3; i++) {
					int j = i;
					shouldBeConfiscatedTwo [j] = false;
				}
			} else {
				for (int i = 0; i < 3; i++) {
					int j = i + 3;
					shouldBeConfiscatedTwo [j] = false;
				}
			}
		}
		for (int i = 0; i < 6; i++) {
			int j = i;
			if (itemsBagTwo [j] == 0) {
				shouldBeConfiscatedTwo [j] = false;
			}
		}

		//END

		suitcase[0].material = suitcaseMaterials[colours[1]];
		airlineScreen.text = airlines[1];
		flightRouteScreen.text = destinationNames [destinations[2]] + " -> " + destinationNames [destinations[3]];
		weightScreen.text = weights [1].ToString() + "kg";
		currentBagArray = itemsBagTwo;
		for (int i = 0; i < itemButtonRenderers.Length; i++) {
			itemButtonRenderers [i].material = itemMaterials [itemsBagTwo [i]];
		}
		Debug.LogFormat("[Bag Search #{0}] Bag 2 contains: {1}, {2}, {3}, {4}, {5}, {6}.", _moduleId, itemNames[itemsBagTwo[0]], itemNames[itemsBagTwo[1]], itemNames[itemsBagTwo[2]], itemNames[itemsBagTwo[3]], itemNames[itemsBagTwo[4]], itemNames[itemsBagTwo[5]]);
		int count = 0;
		for (int i = 0; i < 6; i++) {
			int j = i;
			if (shouldBeConfiscatedTwo [j]) {
				count++;
			}
		}
		if (twoDetained) {
			Debug.LogFormat ("[Bag Search #{0}] Bag 2 should be detained.", _moduleId);
		} else {
			Debug.LogFormat ("[Bag Search #{0}] Bag 2 should be allowed to board.", _moduleId);
		}
		Debug.LogFormat("[Bag Search #{0}] Bag 2 contains: {1} item(s) should be confiscated total.", _moduleId, count);
		Debug.LogFormat("[Bag Search #{0}] The item positions which should be confiscated are {1}, {2}, {3}, {4}, {5}, {6}.", _moduleId, shouldBeConfiscatedTwo[0], shouldBeConfiscatedTwo[1], shouldBeConfiscatedTwo[2], shouldBeConfiscatedTwo[3], shouldBeConfiscatedTwo[4], shouldBeConfiscatedTwo[5]);
	}

	void bagThreeSetup () {
		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, Module.transform);
		for (int i = 0; i < 6; i++) {
			int j = i;
			itemButtons [j].gameObject.SetActive (true);
		}
		xrayOn = false;
		xrayRemoveChildren ();
		numberScreen.text = "3";
		currentBagArray = itemsBagThree;
		currentBag = 2;

		colourObject.gameObject.SetActive (false);
		if (colourBlindMode) {
			string[] coloursBlind = { "Red", "Blue", "Yellow", "Pink", "Green", "Orange", "Brown" };
			colourObject.gameObject.SetActive (true);
			colourText.text = coloursBlind [colours [2]];
		}

		//START

		bool starbadge = false;
		int starPos = 0;
		for (int i = 0; i < 6; i++) {
			int j = i;
			if (itemsBagThree [j] == 0) {
				itemButtonRenderers[j].gameObject.SetActive (false);
				moduleSelectable.Children [j] = null;
				moduleSelectable.UpdateChildrenProperly (xrayButton);
			}
			if (itemsBagThree [j] == 1 || itemsBagThree [j] == 4) {
				if (airlines [2] == "BoomSky Express") {
					shouldBeConfiscatedThree [j] = true;
				}
			}
			//ball
			if (itemsBagThree [j] == 3) {
				if (airlines [2] == "BlastAir" || flightLengths [2] > 12) {
					shouldBeConfiscatedThree [j] = true;
				}
			}
			//flashlight
			else if (itemsBagThree [j] == 5) {
				if ((Info.GetOnIndicators ().Count () - Info.GetOffIndicators ().Count ()) >= 2) {
					shouldBeConfiscatedThree [j] = true;
				} else { 
					if (airlines [2] == "PopAir Travel") {
						shouldBeConfiscatedThree [j] = true;
					}
				}
			}
			//gamecontroller
			else if (itemsBagThree [j] == 6) {
				if (airlines [2] == "SparkJet Airways" || !(Info.GetSerialNumberLetters ().Any ("ABXY".Contains))) {
					shouldBeConfiscatedThree [j] = true;
				}
			}
			//gun
			else if (itemsBagThree [j] == 7) {
				if (!(airlines [2] == "PopAir Travel")) {
					if (flightLengths [2] < 8) {
						shouldBeConfiscatedThree [j] = true;
					} else {
						threeDetained = true;
					}
				}
			}
			//handcuffs
			else if (itemsBagThree [j] == 8) {
				if (airlines [2] == "WhizBang Airlines") {
					shouldBeConfiscatedThree [j] = true;
				}
				if (!oneDetained && !twoDetained){
					threeDetained = true;
				}
			}
			//knife
			else if (itemsBagThree [j] == 9) {
				if (!(airlines [2] == "PopAir Travel")) {
					threeDetained = true;
				}
			}
			//nail polish
			else if (itemsBagThree [j] == 11) {
				if (colours [2] <= 2 || airlines[2] == "PopAir Travel") {
					shouldBeConfiscatedThree [j] = true;
				}
			}
			//scales
			else if (itemsBagThree [j] == 12) {
				if (airlines [2] == "BlastAir") {
					shouldBeConfiscatedThree [j] = true;
				}
				if (weights [2] > weights [1]) {
					shouldBeConfiscatedThree [j] = true;
				}
			}
			//shirt
			else if (itemsBagThree [j] == 13) {
				if (airlines [2] == "DynamitePlus Airways") {
					shouldBeConfiscatedThree [j] = true;
				}

			}
			//tie
			else if (itemsBagThree [j] == 14) {
				shouldBeConfiscatedThree [j] = true;
				for (int q = 0; q < 6; q++) {
					if (itemsBagThree [q] == 13) {
						shouldBeConfiscatedThree [j] = false;
					}
				}
				if (airlines [2] == "DynamitePlus Airways") {
					shouldBeConfiscatedThree [j] = true;
				}
			}
			//wine
			else if (itemsBagThree [j] == 15) {
				if (Info.IsIndicatorPresent ("CAR") || airlines[2] == "InfernoFly") {
					shouldBeConfiscatedThree [j] = true;
				}
			}
			//wire
			else if (itemsBagThree [j] == 16) {
				if (Info.GetPortCount() < 3 || airlines[2] == "SparkJet Airways") {
					shouldBeConfiscatedThree [j] = true;
				}
				if (Info.GetPortCount() == 0) {
					threeDetained = true;
				}
			}
			//star badge
			else if (itemsBagThree [j] == 2) {
				if (airlines [2] == "WhizBang Airlines") {
					shouldBeConfiscatedThree [j] = true;
				} else {
					starbadge = true;
					starPos = j;
				}
			}
		}

		//money
		for (int i = 0; i < 6; i++) {
			int j = i;
			if (itemsBagThree [j] == 10) {
				shouldBeConfiscatedThree [j] = true;
				for (int q = 0; q < 6; q++) {
					if (shouldBeConfiscatedThree [q] && (q != j)) {
						shouldBeConfiscatedThree [j] = false;
					}
				}
				if (airlines [2] == "InfernoFly") {
					shouldBeConfiscatedThree [j] = true;
				}
			}
		}
		if (airlines [2] == "SparkJet Airways") {
			if (threeDetained) {
				threeDetained = false;
				for (int i = 0; i < 6; i++) {
					int j = i;
					shouldBeConfiscatedThree [j] = true;
				}
			}
		}
		if (airlines [2] == "BlastAir" && Info.GetBatteryCount () > 2) {
			shouldBeConfiscatedThree [5] = true;
		}
		if (airlines [2] == "DynamitePlus Airways") {
			for (int i = 0; i < 3; i++) {
				int j = i;
				shouldBeConfiscatedThree [j] = false;
			}
		}
		if (airlines [2] == "BoomSky Express") {
			if (weights [2] == 2 || weights [2] == 3 || weights [2] == 5 || weights [2] == 7 || weights [2] == 11 || weights [2] == 13 || weights [2] == 17 || weights [2] == 19 || weights [2] == 23 || weights [2] == 29 || weights [2] == 31 || weights [2] == 37) {
				for (int i = 0; i < 3; i++) {
					int j = i;
					shouldBeConfiscatedThree [j] = true;
				}
			}
		}
		if (starbadge) {
			if (starPos < 3) {
				for (int i = 0; i < 3; i++) {
					int j = i;
					shouldBeConfiscatedThree [j] = false;
				}
			} else {
				for (int i = 0; i < 3; i++) {
					int j = i + 3;
					shouldBeConfiscatedThree [j] = false;
				}
			}
		}
		for (int i = 0; i < 6; i++) {
			int j = i;
			if (itemsBagThree [j] == 0) {
				shouldBeConfiscatedThree [j] = false;
			}
		}

		//END

		suitcase[0].material = suitcaseMaterials[colours[2]];
		airlineScreen.text = airlines[2];
		flightRouteScreen.text = destinationNames [destinations[4]] + " -> " + destinationNames [destinations[5]];
		weightScreen.text = weights [2].ToString() + "kg";
		currentBagArray = itemsBagThree;
		for (int i = 0; i < itemButtonRenderers.Length; i++) {
			itemButtonRenderers [i].material = itemMaterials [itemsBagThree [i]];
		}
		Debug.LogFormat("[Bag Search #{0}] Bag 3 contains: {1}, {2}, {3}, {4}, {5}, {6}.", _moduleId, itemNames[itemsBagThree[0]], itemNames[itemsBagThree[1]], itemNames[itemsBagThree[2]], itemNames[itemsBagThree[3]], itemNames[itemsBagThree[4]], itemNames[itemsBagThree[5]]);
		int count = 0;
		for (int i = 0; i < 6; i++) {
			int j = i;
			if (shouldBeConfiscatedThree [j]) {
				count++;
			}
		}
		if (threeDetained) {
			Debug.LogFormat ("[Bag Search #{0}] Bag 3 should be detained.", _moduleId);
		} else {
			Debug.LogFormat ("[Bag Search #{0}] Bag 3 should be allowed to board.", _moduleId);
		}
		Debug.LogFormat("[Bag Search #{0}] Bag 3 contains: {1} item(s) should be confiscated total.", _moduleId, count);
		Debug.LogFormat("[Bag Search #{0}] The item positions which should be confiscated are {1}, {2}, {3}, {4}, {5}, {6}.", _moduleId, shouldBeConfiscatedThree[0], shouldBeConfiscatedThree[1], shouldBeConfiscatedThree[2], shouldBeConfiscatedThree[3], shouldBeConfiscatedThree[4], shouldBeConfiscatedThree[5]);
	}

	void handleDetain() {
		if (_isSolved) {
			return;
		}
		if (airlinesFake [currentBag]) {
			Debug.LogFormat ("[Bag Search #{0}] Detained passenger successfully.", _moduleId);
			if (currentBag == 0) {
				bagTwoSetup ();
			} else if (currentBag == 1) {
				bagThreeSetup ();
			} else {
				solvedModule ();
			}
			return;
		}
		bool temp = false;
		if (currentBag == 0) {
			for (int i = 0; i < 6; i++) {
				int j = i;
				if (shouldBeConfiscatedOne [j]) {
					temp = true;
				}
			}
		} else if (currentBag == 1) {
			for (int i = 0; i < 6; i++) {
				int j = i;
				if (shouldBeConfiscatedTwo [j]) {
					temp = true;
				}
			}
		} else if (currentBag == 2) {
			for (int i = 0; i < 6; i++) {
				int j = i;
				if (shouldBeConfiscatedThree [j]) {
					temp = true;
				}
			}
		}
		if (!temp) {
			//add elseifs for other bags
			if (currentBag == 0 && oneDetained) {
				bool bonePresent = false;
				for (int i = 0; i < currentBagArray.Length; i++) {
					int j = i;
					if (currentBagArray [j] == 4) {
						bonePresent = true;	
					}
				} 
				if (!(bonePresent == xrayOn)) {
					Module.HandleStrike ();
					Debug.LogFormat ("[Bag Search #{0}] Detain button should not have been pressed as the X-ray machine should have been toggled, strike issued.", _moduleId);
				} else {
					Debug.LogFormat ("[Bag Search #{0}] Detained passenger successfully.", _moduleId);
					bagTwoSetup ();
				}
			} else if (currentBag == 1 && twoDetained) {
				bool bonePresent = false;
				for (int i = 0; i < currentBagArray.Length; i++) {
					int j = i;
					if (currentBagArray [j] == 4) {
						bonePresent = true;	
					}
				} 
				if (!(bonePresent == xrayOn)) {
					Module.HandleStrike ();
					Debug.LogFormat ("[Bag Search #{0}] Detain button should not have been pressed as the X-ray machine should have been toggled, strike issued.", _moduleId);
				} else {
					Debug.LogFormat ("[Bag Search #{0}] Detained passenger successfully.", _moduleId);
					bagThreeSetup ();
				}
			} else if (currentBag == 2 && threeDetained) {
				bool bonePresent = false;
				for (int i = 0; i < currentBagArray.Length; i++) {
					int j = i;
					if (currentBagArray [j] == 4) {
						bonePresent = true;	
					}
				} 
				if (!(bonePresent == xrayOn)) {
					Module.HandleStrike ();
					Debug.LogFormat ("[Bag Search #{0}] Detain button should not have been pressed as the X-ray machine should have been toggled, strike issued.", _moduleId);
				} else {
					Debug.LogFormat ("[Bag Search #{0}] Detained passenger successfully.", _moduleId);
					solvedModule();
				}
			} else {
				Module.HandleStrike ();
				Debug.LogFormat ("[Bag Search #{0}] Detain button should not have been pressed as the passenger should be allowed to board, strike issued.", _moduleId);
			}
		} else {
			Module.HandleStrike ();
			Debug.LogFormat ("[Bag Search #{0}] Detain button should not have been pressed as there are still items left to confiscate, strike issued.", _moduleId);
		}
	}

	void handleXray() {
		if (_isSolved) {
			return;
		}
		Audio.PlayGameSoundAtTransform (KMSoundOverride.SoundEffect.ButtonPress, xrayButton.transform);
		if (xrayOn == false) {
			//If airline is fake strike
			if (airlinesFake [currentBag]) {
				Module.HandleStrike ();
				Debug.LogFormat ("[Bag Search #{0}] Xray machine should not have been turned on as the airline is fake, strike issued.", _moduleId);
			//If FRQandU rule is true
			} else if (airlines [currentBag] == "FRQandU" && Info.IsIndicatorPresent ("FRQ")) {
				Module.HandleStrike ();
				Debug.LogFormat("[Bag Search #{0}] Xray machine should not have been turned on due to the FRQandU airline rule, strike issued.", _moduleId);
			} else {
				xrayOn = true;
				xrayNewChildren ();
				Audio.PlaySoundAtTransform ("xrayOn", Module.transform);
				Debug.LogFormat("[Bag Search #{0}] Xray machine activated.", _moduleId);
			}
		} else {
			//If bone is present strike
			bool bonePresent = false;
			for (int i = 0; i < currentBagArray.Length; i++){
				int j = i;
				if (currentBagArray[j] == 4){
					bonePresent = true;	
				}
			}
			if (bonePresent) {
				Module.HandleStrike ();
				Debug.LogFormat("[Bag Search #{0}] Xray machine should not have been turned off because there is a bone in the bag, strike issued.", _moduleId);
			} else {
				bool temp = false;
				if (currentBag == 0) {
					for (int i = 0; i < 6; i++) {
						int j = i;
						if (shouldBeConfiscatedOne [j]) {
							temp = true;
						}
					}
				} else if (currentBag == 1) {
					for (int i = 0; i < 6; i++) {
						int j = i;
						if (shouldBeConfiscatedTwo [j]) {
							temp = true;
						}
					}
				} else if (currentBag == 2) {
					for (int i = 0; i < 6; i++) {
						int j = i;
						if (shouldBeConfiscatedThree [j]) {
							temp = true;
						}
					}
				}
				if (!temp) {
					xrayOn = false;
					xrayRemoveChildren ();
					Audio.PlaySoundAtTransform ("xrayOff", Module.transform);
					Debug.LogFormat ("[Bag Search #{0}] Xray machine deactivated.", _moduleId);
				} else {
					Module.HandleStrike ();
					Debug.LogFormat ("[Bag Search #{0}] Xray machine should not have been turned off because more items still need to be confiscated, strike issued.", _moduleId);
				}
			}
		}
	}

	public GameObject suitcaseParts;

	void solvedModule ()
	{
		Audio.PlaySoundAtTransform ("correct", Module.transform);
		Debug.LogFormat ("[Bag Search #{0}] Module solved.", _moduleId);

		suitcase [0].gameObject.SetActive (false);
		suitcaseParts.gameObject.SetActive (false);
		xrayOn = false;
		xrayRemoveChildren ();
		airlineScreen.text = "Enjoy your flight!";
		flightRouteScreen.text = "Boarding...";
		weightScreen.text = 0 + "kg";
		numberScreen.text = "✓";

		_isSolved = true;
		Module.HandlePass ();
	}

	void handleBoard() {
		if (_isSolved) {
			return;
		}
		if (airlines [currentBag] == "FRQandU" && Info.IsIndicatorPresent ("FRQ")) {
			Debug.LogFormat ("[Bag Search #{0}] Boarded passenger successfully.", _moduleId);
			if (currentBag == 0) {
				bagTwoSetup ();
			} else if (currentBag == 1) {
				bagThreeSetup ();
			} else {
				solvedModule ();
			}
		}
		bool temp = false;
		if (currentBag == 0) {
			for (int i = 0; i < 6; i++) {
				int j = i;
				if (shouldBeConfiscatedOne [j]) {
					temp = true;
				}
			}
		} else if (currentBag == 1) {
			for (int i = 0; i < 6; i++) {
				int j = i;
				if (shouldBeConfiscatedTwo [j]) {
					temp = true;
				}
			}
		} else if (currentBag == 2) {
			for (int i = 0; i < 6; i++) {
				int j = i;
				if (shouldBeConfiscatedThree [j]) {
					temp = true;
				}
			}
		}
		if (!temp) {
			//add elseifs for other bags
			if (currentBag == 0 && !(oneDetained)) {
				bool bonePresent = false;
				for (int i = 0; i < currentBagArray.Length; i++) {
					int j = i;
					if (currentBagArray [j] == 4) {
						bonePresent = true;	
					}
				} 
				if (!(bonePresent == xrayOn)) {
					Module.HandleStrike ();
					Debug.LogFormat ("[Bag Search #{0}] Board button should not have been pressed as the X-ray machine should have been toggled, strike issued.", _moduleId);
				} else {
					Debug.LogFormat ("[Bag Search #{0}] Boarded passenger successfully.", _moduleId);
					bagTwoSetup ();
				}
			} else if (currentBag == 1 && !(twoDetained)) {
					bool bonePresent = false;
					for (int i = 0; i < currentBagArray.Length; i++) {
						int j = i;
						if (currentBagArray [j] == 4) {
							bonePresent = true;	
						}
					} 
					if (!(bonePresent == xrayOn)) {
						Module.HandleStrike ();
						Debug.LogFormat ("[Bag Search #{0}] Board button should not have been pressed as the X-ray machine should have been toggled, strike issued.", _moduleId);
					} else {
						Debug.LogFormat ("[Bag Search #{0}] Boarded passenger successfully.", _moduleId);
						bagThreeSetup ();
					}
			} else if (currentBag == 2 && !(threeDetained)) {
				bool bonePresent = false;
				for (int i = 0; i < currentBagArray.Length; i++) {
					int j = i;
					if (currentBagArray [j] == 4) {
						bonePresent = true;	
					}
				} 
				if (!(bonePresent == xrayOn)) {
					Module.HandleStrike ();
					Debug.LogFormat ("[Bag Search #{0}] Board button should not have been pressed as the X-ray machine should have been toggled, strike issued.", _moduleId);
				} else {
					Debug.LogFormat ("[Bag Search #{0}] Boarded passenger successfully.", _moduleId);
					solvedModule();
				}
			} else {
				Module.HandleStrike ();
				Debug.LogFormat ("[Bag Search #{0}] Board button should not have been pressed as the passenger should been detained, strike issued.", _moduleId);
			}
		} else {
			Module.HandleStrike ();
			Debug.LogFormat ("[Bag Search #{0}] Board button should not have been pressed as there are still items left to confiscate, strike issued.", _moduleId);
		}
	}

	void handleConfiscate (int position){
		if (_isSolved) {
			return;
		}
		if (currentBag == 0) {
			if (shouldBeConfiscatedOne [position]) {
				shouldBeConfiscatedOne [position] = false;
				itemButtons [position].gameObject.SetActive (false);
				int tempPos = position;
				if (position > 2) {
					tempPos++;
				}
				moduleSelectable.Children [tempPos] = null;
				moduleSelectable.UpdateChildrenProperly (null);
				Audio.PlayGameSoundAtTransform (KMSoundOverride.SoundEffect.BinderDrop, Module.transform);
				Debug.LogFormat("[Bag Search #{0}] Confiscated correct item in position {1}.", _moduleId, (position + 1));
			} else {
				Module.HandleStrike ();
				Debug.LogFormat("[Bag Search #{0}] Confiscated incorrect item in position {1}, strike issued.", _moduleId, (position + 1));
			}
		} else if (currentBag == 1) {
			if (shouldBeConfiscatedTwo [position]) {
				shouldBeConfiscatedTwo [position] = false;
				itemButtons [position].gameObject.SetActive (false);
				int tempPos = position;
				if (position > 2) {
					tempPos++;
				}
				moduleSelectable.Children [tempPos] = null;
				moduleSelectable.UpdateChildrenProperly (null);
				Audio.PlayGameSoundAtTransform (KMSoundOverride.SoundEffect.BinderDrop, Module.transform);
				Debug.LogFormat("[Bag Search #{0}] Confiscated correct item in position {1}.", _moduleId, (position + 1));
			} else {
				Module.HandleStrike ();
				Debug.LogFormat("[Bag Search #{0}] Confiscated incorrect item in position {1}, strike issued.", _moduleId, (position + 1));
			}
		} else if (currentBag == 2) {
			if (shouldBeConfiscatedThree [position]) {
				shouldBeConfiscatedThree [position] = false;
				itemButtons [position].gameObject.SetActive (false);
				int tempPos = position;
				if (position > 2) {
					tempPos++;
				}
				moduleSelectable.Children [tempPos] = null;
				moduleSelectable.UpdateChildrenProperly (null);
				Audio.PlayGameSoundAtTransform (KMSoundOverride.SoundEffect.BinderDrop, Module.transform);
				Debug.LogFormat("[Bag Search #{0}] Confiscated correct item in position {1}.", _moduleId, (position + 1));
			} else {
				Module.HandleStrike ();
				Debug.LogFormat("[Bag Search #{0}] Confiscated incorrect item in position {1}, strike issued.", _moduleId, (position + 1));
			}
		}
	}

	void xrayNewChildren(){
		for (int i = 0; i < 6; i++) {
			int j = i;
			int q = i;
			if (!(currentBagArray [j] == 0)) {
				if (j >= 3) {
					q++;
				}
				moduleSelectable.Children [q] = itemButtons [j];
			}
		}
		moduleSelectable.UpdateChildrenProperly(xrayButton);
	}
	void xrayRemoveChildren(){
		moduleSelectable.Children [0] = null;
		moduleSelectable.Children [1] = null;
		moduleSelectable.Children [2] = null;
		moduleSelectable.Children [4] = null;
		moduleSelectable.Children [5] = null;
		moduleSelectable.Children [6] = null;
		moduleSelectable.UpdateChildrenProperly(null);
	}

	private readonly string TwitchHelpMessage = @"Toggle the X-ray machine with '!{0} xray', detain the passenger with '!{0} detain', allow the passenger to board with '!{0} board', confiscate an item in a specific position with '!{0} confiscate 1' with the number being the position in scanline order of the item or enable colourblind mode using '!{0} colourblind'";
	private IEnumerator ProcessTwitchCommand(string command) {
		command = command.ToLowerInvariant ();
		if (command.Equals ("xray")) {
			yield return null;
			handleXray ();
		} else if (command.Equals ("detain")) {
			yield return null;
			handleDetain ();
		} else if (command.Equals ("board")) {
			yield return null;
			handleBoard ();
		} else if (command.Equals ("confiscate 1")) {
			yield return null;
			if (itemButtons [0].isActiveAndEnabled) {
				handleConfiscate (0);
			}
		} else if (command.Equals ("confiscate 2")) {
			yield return null;
			if (itemButtons [1].isActiveAndEnabled) {
				handleConfiscate (1);
			}
		} else if (command.Equals ("confiscate 3")) {
			yield return null;
			if (itemButtons [2].isActiveAndEnabled) {
				handleConfiscate (2);
			}
		} else if (command.Equals ("confiscate 4")) {
			yield return null;
			if (itemButtons [3].isActiveAndEnabled) {
				handleConfiscate (3);
			}
		} else if (command.Equals ("confiscate 5")) {
			yield return null;
			if (itemButtons [4].isActiveAndEnabled) {
				handleConfiscate (4);
			}
		} else if (command.Equals ("confiscate 6")) {
			yield return null;
			if (itemButtons [5].isActiveAndEnabled) {
				handleConfiscate (5);
			}
		} else if (command.Equals ("colourblind") || command.Equals("colorblind")) {
			yield return null;
			if (!_isSolved) {
				colourBlindMode = true;
				string[] coloursBlind = { "Red", "Blue", "Yellow", "Pink", "Green", "Orange", "Brown" };
				colourObject.gameObject.SetActive (true);
				colourText.text = coloursBlind [colours [currentBag]];
			}
		}
	}
}