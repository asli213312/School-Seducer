using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
using _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune;
using _School_Seducer_.Editor.Scripts;
using _School_Seducer_.Editor.Scripts.Chat;
using _School_Seducer_.Editor.Scripts.Utility;
using _School_Seducer_.Editor.Scripts.Utility.Translation;
using _School_Seducer_.Editor.Scripts.UI.Shop;

namespace _School_Seducer_.Editor.Scripts.Services 
{
	[Serializable]
	public class PlayerData
	{
		public int gold;
		public int diamonds;
		public SettingsSave settings;
		public ShopSave shop;
		public List<CharacterSave> characters;
		public List<string> giftsId;
		public List<WheelSlotData> gifts;

		public void Initialize(List<WheelSlotData> gifts) 
		{
			if (giftsId.Count > 0) giftsId.Clear();

			foreach (var gift in gifts) 
			{
				giftsId.Add(gift.name);
			}

			this.gifts = gifts;
		}

		public void LoadShopData(ShopTabViewBase[] tabs) 
		{
			if (shop.tabs.Count == 0) return;

			foreach (var tabSave in shop.tabs) 
			{
				foreach (var tabView in tabs) 
				{
					if (tabView.Data.name != tabSave.name) continue;

					tabSave.GetSoldedCount(tabView.Data.ItemsData);
				}	
			}
		}

		public void SaveShopData(ShopTabDataBase data) 
		{
			shop.tabs ??= new();

			ShopTabDataSave save = new ShopTabDataSave();
			save.SetSoldedCount(data.ItemsData);
			save.name = data.name;

			ShopTabDataSave existSave = shop.tabs.FirstOrDefault(x => x.name == save.name);

			if (existSave != null) 
			{
				shop.tabs.Remove(existSave);
				shop.tabs.Add(save);
			}
			else
				shop.tabs.Add(save);
		}

		public void LoadCharacterData(CharacterData characterData)
		{
			CharacterSave save = characters.FirstOrDefault(x => x.name == characterData.name);

			if (save == null) return;
			
			save.GetScriptableData(this);
			save.Get(characterData);		
		}

		public void AddCharacterData(CharacterData characterData)
		{
			characters ??= new();
			
			CharacterSave save = new CharacterSave();
			save.SetScriptableData(this, characterData);
			save.Set(characterData);

			characters.Remove(characters.FirstOrDefault(x => x.name == save.name));
			
			characters.Add(save);
		}
	}

	[Serializable]
	public class SettingsSave 
	{
		public bool tutorialCompleted;
		public bool soundEnabled;
		public bool musicEnabled;
		public string languageCode;
	}

	[Serializable]
	public class ConversationSave
	{
		public string name;
		public int completedMessages;

		public ConversationSave(string name, int completedMessages)
		{
			this.name = name;
			this.completedMessages = completedMessages;
		}
	}

	[Serializable]
	public class ShopSave 
	{
		public List<ShopTabDataSave> tabs;
	}

	[Serializable]
	public class ShopTabDataSave 
	{
		public string name; 
		public int countSoldedItems;
		public List<ShopGroupItemSave> groupSaves;

		public void GetSoldedCount(IShopItemPurchasable[] items) 
		{
			for (int i = 0; i < items.Length && i < countSoldedItems; i++) 
			{
				items[i].IsSold = true;

				if (items[i] is ShopGroupItemData groupItem) 
				{
					if (groupSaves.Count == 0) continue;

					ShopGroupItemSave groupSave = groupSaves.FirstOrDefault(x => x.name == groupItem.name);

					if (groupSave == null) continue;

					for (int j = 0; j < groupItem.items.Length && i < groupSave.countSoldedItems; j++) 
					{
						IShopItemPurchasable singlePurchasableItem = groupItem.items[j] as IShopItemPurchasable;
						singlePurchasableItem.IsSold = true;
					}
				}
			}
		}

		public void SetSoldedCount(IShopItemPurchasable[] items) 
		{
			int count = 0;
			groupSaves = new();

			foreach (var item in items) 
			{
				if (item.IsSold)
					count++;

				if (item is ShopGroupItemData groupItem) 
				{	
					ShopGroupItemSave groupItemSave = new();
					groupItemSave.SetSoldedCount(groupItem.items);
					groupItemSave.name = groupItem.name;

					groupSaves.Add(groupItemSave);
				}
			}

			countSoldedItems = count;
		}
	}

	[Serializable]
	public class ShopGroupItemSave 
	{
		public string name;
		public int countSoldedItems;

		public void SetSoldedCount(IShopItemPurchasable[] items) 
		{
			int count = 0;

			foreach (var item in items) 
			{
				if (item.IsSold)
					count++;
			}

			countSoldedItems = count;
		}
	}

	[Serializable]
	public class CharacterSave
	{
		public string name;
		public int experience;
		public int countCompletedConversations;
		public int countUnlockedConversations;
		public int countUnseenConversation;
		public List<WheelSlotData> gifts;
		public List<string> giftsId;
		public List<ConversationSave> conversations;

		public void GetScriptableData(PlayerData mainData) 
		{
			if (giftsId.Count == 0) return;

			List<WheelSlotData> newGifts = new();

			foreach (var gift in mainData.gifts) 
			{
				foreach (var giftId in giftsId) 
				{
					if (gift.name != giftId) continue;

					newGifts.Add(gift);
				}
			}

			gifts = newGifts;
		}

		public void Get(CharacterData data)
		{
			for (int i = 0; i < data.allConversations.Count && i < countCompletedConversations; i++)
			{
				data.allConversations[i].isCompleted = true;	
			}

			for (int i = 0; i < data.allConversations.Count && i < countUnlockedConversations; i++)
			{
				data.allConversations[i].isUnlocked = true;
			}

			for (int i = 0; i < data.allConversations.Count && i < countUnseenConversation; i++)
			{
				data.allConversations[i].isSeen = false;
			}

			foreach (var conversation in data.allConversations)
			{
				ConversationSave save = conversations.FirstOrDefault(x => x.name == conversation.name);
				if (save == null)
				{
					Debug.LogWarning("Conversation " + conversation.name + " not found in save file of character: " + data.name);
					continue;
				}

				for (int i = 0; i < conversation.Messages.Length && i < save.completedMessages; i++)
					conversation.Messages[i].completed = true;
			}

			data.gifts = gifts;
			data.experience = experience;
		}

		public void SetScriptableData(PlayerData mainData, CharacterData data) 
		{
			giftsId = new();

			foreach (var giftNew in data.gifts) 
			{
				giftsId.Add(giftNew.name);
			}
		}

		public void Set(CharacterData data) 
		{
			name = data.name;
			experience = data.experience;
			countCompletedConversations = GetCompletedConversationsCount(data);
			countUnlockedConversations = GetUnlockedConversationsCount(data);
			countUnseenConversation = GetUnseenConversationsCount(data);

			conversations ??= new();
			conversations.Clear();

			foreach (var conversation in data.allConversations)
			{
				if (conversation.Messages[0].completed == false) continue;

				int completedMessages = GetCompletedMessagesCount(conversation);
				
				conversations.Add(new ConversationSave(conversation.name, completedMessages));
			}
		}

		private int GetCompletedMessagesCount(СonversationData conversation)
		{
			return conversation.Messages.Count(x => x.completed);
		}

		private int GetCompletedConversationsCount(CharacterData data)
		{
			return data.allConversations.Count(x => x.isCompleted);
		}

		private int GetUnlockedConversationsCount(CharacterData data)
		{
			return data.allConversations.Count(x => x.isUnlocked);
		}

		private int GetUnseenConversationsCount(CharacterData data)
		{
			return data.allConversations.Count(x => x.isSeen == false);
		}
	}

	public class SaveToDB : Zenject.MonoInstaller
	{
		[Zenject.Inject] private Bank _bank;

		[SerializeField] private LocalizedGlobalMonoBehaviour localizer;
		[SerializeField] private GlobalSettings globalSettings;
		[SerializeField] public PlayerData mainData;
		[SerializeField] private List<WheelSlotData> slots;

		public event Action OnLoaded;

		private string token;
		public string id;

	    public override void InstallBindings()
	    {
	        Container.Bind<SaveToDB>().FromComponentInHierarchy().AsSingle();
	    }

	    public void Initialize() 
	    {
	    	mainData.Initialize(slots);
	    }

	    private void OnAuthenticate(string result) 
	    {
	    	Debug.Log("OnAuthenticate invoked function");

	        JObject response = JObject.Parse(result);

	        if (response.ContainsKey("token"))
		    {
		        string token = (string)response["token"];
		        string id = (string)response["id"];
		        Debug.Log("Received token: " + token);
		        
		        this.token = token;
		        this.id = id;
		    }
		    else
		    {
		        Debug.LogError("Error! Token not found OnAuthenticate");
		    }
	    }

	    public void SAVE() 
	    {
	    	mainData.gold = _bank.Money;
	    	mainData.diamonds = _bank.Diamonds;
	    	mainData.settings.tutorialCompleted = globalSettings.TutorialCompleted;
	    	mainData.settings.languageCode = localizer.GlobalLanguageCodeRuntime;
	    	string jsonString = JsonUtility.ToJson(mainData);
	        Debug.Log("jsonified save: " + jsonString);

	        StartCoroutine(GetRequest($"https://fuckfest.college/scripts/saveBD.php?id={id}&value={jsonString}&token={token}"));
	    }

	    public IEnumerator LOAD() 
	    {
	    	yield return GetRequestLoad($"https://fuckfest.college/scripts/loadSQL.php?id={id}");
	    }

	    private IEnumerator GetRequest(string uri)
	    {
	        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
	        {
	            // Request and wait for the desired page.
	            yield return webRequest.SendWebRequest();

	            string[] pages = uri.Split('/');
	            int page = pages.Length - 1;

	            switch (webRequest.result)
	            {
	                case UnityWebRequest.Result.ConnectionError:
	                case UnityWebRequest.Result.DataProcessingError:
	                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
	                    break;
	                case UnityWebRequest.Result.ProtocolError:
	                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
	                    break;
	                case UnityWebRequest.Result.Success:
	                    string request = webRequest.downloadHandler.text;
	                    int startIndex = request.IndexOf("{\""); // Проверяем наличие JSON в строке

	                    /*if (startIndex != -1)
	                    {
		                    string result = request.Substring(startIndex);
		                    string[] parts = result.Split("}\"");
		                    parts[0] += "}";
		                    Debug.Log(parts[0]); //JSON С ССОХРАНЕНИЯМИ
	                    }
	                    else
	                    {
		                    Debug.LogError("JSON data not found in the response");
	                    }*/
	                    break;
	            }
	        }
	    }

	    private IEnumerator GetRequestLoad(string uri)
	    {
	        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
	        {
	            // Request and wait for the desired page.
	            yield return webRequest.SendWebRequest();

	            string[] pages = uri.Split('/');
	            int page = pages.Length - 1;

	            switch (webRequest.result)
	            {
	                case UnityWebRequest.Result.ConnectionError:
	                case UnityWebRequest.Result.DataProcessingError:
	                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
	                    break;
	                case UnityWebRequest.Result.ProtocolError:
	                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
	                    break;
	                case UnityWebRequest.Result.Success:
	                    string request = webRequest.downloadHandler.text;
	                    string result = request.Substring(request.IndexOf("{\""));
	                    string[] parts = result.Split("}\""); 
	                    parts[0] += "}";
	                    Debug.Log(parts[0]); //JSON С ССОХРАНЕНИЯМИ  parts[0].

						mainData = JsonUtility.FromJson<PlayerData>(parts[0]);
	                    _bank.Money = mainData.gold;
	                    _bank.Diamonds = mainData.diamonds;
	                    globalSettings.TutorialCompleted = mainData.settings.tutorialCompleted;
	                    localizer.GlobalLanguageCodeRuntime = mainData.settings.languageCode;
	                    localizer.Notify();

	                    OnLoaded?.Invoke();

	                    Debug.Log("SAVE DATA: " + mainData);

	                    break;
	            }
	        }
	    }
	}
}
