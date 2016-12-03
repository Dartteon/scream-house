using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmojiPopup : MonoBehaviour {
	public Sprite[] emojiSprites;

	private string[] chats = { "Holy...", "OMG", "OMGWTFBBQ", "Lame.", 
								"I see you...", "Noob", "HEART ATTACK" };
	public string[] scaredTexts = { "HOLY!", "OMG", "WTH", "OMAGAD", "NOPENOPENOPE"};
	private string[] disappointedTexts = { "I saw you..", "You suck at hiding" };
	private string[] notScaredTexts = { "Meh 1/10", "I saw that coming", "Not scary bro" };
	private string[] superScaredTexts = { "OMGWTFBBQ", "AAH!!11cos(0)1!1", "WAT DA FAQ", "I SHAT MY PANTS"  };

	public enum EmojiType { SCARED = 0, CRYING = 1, SCREAMING = 2, HEARTATTACK = 3, 
							DISAPPOINTED = 4, SORRY = 5, MORBID = 6, THUMBSUP = 7, THUMBSDOWN = 8 }
	
	private void ShowEmoji (EmojiType type) {
		transform.Find ("Emoji").GetComponent<SpriteRenderer> ().sprite = emojiSprites [(int)type];
		transform.Find ("TextCanvas").Find("Text").GetComponent<Text> ().text = chats [(int)type];
	}
	private void ShowEmoji (int type) {
		transform.Find ("Emoji").GetComponent<SpriteRenderer> ().sprite = emojiSprites [type];
		transform.Find ("TextCanvas").Find("Text").GetComponent<Text> ().text = chats [(int)type];
	}
	private void ShowEmoji (int type, string text) {
		transform.Find ("Emoji").GetComponent<SpriteRenderer> ().sprite = emojiSprites [type];
		transform.Find ("TextCanvas").Find ("Text").GetComponent<Text> ().text = text;
	}

	public void ShowRandomScaredEmoji() {
		int[] indexes = { 0, 1, 7 };
		int randInt = Random.Range (0, indexes.Length);
		ShowEmoji (indexes[randInt], scaredTexts[Random.Range(0, scaredTexts.Length)]);
	}
	public void ShowRandomSuperScaredEmoji() {
		int[] indexes = { 2, 6 };
		int randInt = Random.Range (0, indexes.Length);
		ShowEmoji (indexes[randInt], superScaredTexts[Random.Range(0, superScaredTexts.Length)]);
	}
	public void ShowRandomNotScaredEmoji() {
		int[] indexes = { 4, 8 };
		int randInt = Random.Range (0, indexes.Length);
		ShowEmoji (indexes[randInt], notScaredTexts[Random.Range(0, notScaredTexts.Length)]);
	}

	public void ShowRandomDisappointedEmoji() {
		int[] indexes = { 1, 6 };
		int randInt = Random.Range (0, indexes.Length);
		ShowEmoji (indexes[randInt], disappointedTexts[Random.Range(0, disappointedTexts.Length)]);
	}

	public void ShowRandomHeartAttackEmoji() {
		ShowEmoji ((int)EmojiType.HEARTATTACK, "HEART ATTACK");
	}

	public string GetRandomText(EmojiType type) {
		switch (type) {
		case EmojiType.SCARED:
		case EmojiType.SCREAMING:
		case EmojiType.THUMBSUP:
			return scaredTexts [Random.Range (0, scaredTexts.Length)];
		case EmojiType.MORBID:
		case EmojiType.CRYING:
			return superScaredTexts [Random.Range (0, superScaredTexts.Length)];
		case EmojiType.DISAPPOINTED:
		case EmojiType.THUMBSDOWN:
			return disappointedTexts [Random.Range (0, disappointedTexts.Length)];
		case EmojiType.HEARTATTACK:
			return "HEART ATTACK";
		default:
			return "ERROR";
		}
	}
}
