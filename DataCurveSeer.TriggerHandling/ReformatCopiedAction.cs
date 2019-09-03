using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DataCurveSeer.Common;
using DataCurveSeer.Common.Interfaces;

namespace DataCurveSeer.TriggerHandling
{
	public interface IReformatCopiedAction
	{
		Classes.action Run(Classes.action formattedAction, int uid, int evRef);
	}
	internal sealed class ReformatCopiedAction : IReformatCopiedAction
	{
		private string _uidAndEvRef;
		private readonly ILogging _logging;

		public ReformatCopiedAction(ILogging logging)
		{
			_logging = logging;

		}

		public Classes.action Run(Classes.action formattedAction, int uid, int evRef)
		{
			_uidAndEvRef = $"{uid.ToString()}_{evRef.ToString()}_"; ;
			//_uid = uid;
			//_evRef = evRef;
			formattedAction = ChangeIncorrectEvRef(formattedAction);
			return formattedAction;
		}

		private Classes.action ChangeIncorrectEvRef(Classes.action formattedAction)
		{
			//Remove duplicate keys. Only keep the one that might be the latest

			_logging.LogIfLogToFileEnabled($"formattedAction has {formattedAction.Keys.Count} keys before reformatting");
			LogAllKeysAndValues(formattedAction);
			formattedAction = RemoveDuplicateKeys(formattedAction);
			formattedAction = RemoveUidEvRefAndAllOtherGarbage(formattedAction);

			_logging.LogIfLogToFileEnabled($"formattedAction has {formattedAction.Keys.Count} keys after reformatting");
			LogAllKeysAndValues(formattedAction);
			return formattedAction;
		}

		private void LogAllKeysAndValues(Classes.action formattedAction)
		{
			if (formattedAction == null || formattedAction.Keys.Count == 0) return;

			foreach (var formattedActionKey in formattedAction.Keys)
			{
				_logging.Log($"Key: {formattedActionKey}", LogLevel.DebugToFile);
			}
		}

		private Classes.action RemoveUidEvRefAndAllOtherGarbage(Classes.action formattedAction)
		{
			var allKeys = formattedAction.Keys.ToList();
			var keyInfoList = new List<KeyToReplaceAndReplacement>();
			var uidKeyToLocate = @"\d{1,5}_\d{1,5}_[A-Z]";
			var regex = new Regex(uidKeyToLocate);
			foreach (var currentKey in allKeys)
			{
				//exit if we dont find any "crap"
				if (!regex.IsMatch(currentKey)) continue;

				var indexOfUnderScore = currentKey.IndexOf('_', 2);
				if (indexOfUnderScore > 0)
				{
					var maxLength = GetLengthToUse(currentKey, indexOfUnderScore);
					//remap a 
					var keyInfo = new KeyToReplaceAndReplacement();
					keyInfo.OriginalKey = currentKey;
					keyInfo.NewKey = currentKey.Substring(0, maxLength);
					keyInfoList.Add(keyInfo);
				}
			}

			foreach (var keyToHandle in keyInfoList)
			{
				//replace garbage keys
				var valueToKeep = formattedAction[keyToHandle.OriginalKey];
				if (!formattedAction.ContainsKey(keyToHandle.NewKey))
				{
					formattedAction.Add(keyToHandle.NewKey, valueToKeep);
				}
				formattedAction.Remove(keyToHandle.OriginalKey);
			}

			return formattedAction;
		}

		private int GetLengthToUse(string currentKey, int indexOfUnderScore)
		{
			var indexOfNextUnderscore = currentKey.IndexOf('_', indexOfUnderScore + 1);
			while (indexOfNextUnderscore > 0 && indexOfNextUnderscore == indexOfUnderScore + 1)
			{
				indexOfUnderScore = indexOfNextUnderscore;
				indexOfNextUnderscore = currentKey.IndexOf('_', indexOfUnderScore);
			}

			return indexOfUnderScore + 1;
		}

		private Classes.action RemoveDuplicateKeys(Classes.action formattedAction)
		{
			var keysToCheck = formattedAction.Keys.ToList();
			var duplicateKeysToKeep = ReturnDuplicateKeysToKeep(keysToCheck);
			foreach (var duplicateKeyToKeep in duplicateKeysToKeep)
			{
				if (duplicateKeyToKeep.KeysToRemove != null)
				{
					foreach (var keyToRemove in duplicateKeyToKeep.KeysToRemove)
					{
						formattedAction.Remove(keyToRemove);
					}
				}
			}

			return formattedAction;
		}

		private List<DuplicateKeyToKeep> ReturnDuplicateKeysToKeep(List<string> keysToCheck)
		{
			var duplicateKeysToKeep = new List<DuplicateKeyToKeep>();
			foreach (var keyToCheck in keysToCheck)
			{
				if (keyToCheck.Contains(_uidAndEvRef) && !duplicateKeysToKeep.Exists(x => x.FullKey == keyToCheck || x.KeysToRemove.Contains(keyToCheck)))
				{
					var startOfKeyToSearchFor =
							keyToCheck.Substring(0, (keyToCheck.IndexOf(_uidAndEvRef, StringComparison.CurrentCulture) + _uidAndEvRef.Length));
					var searchResult = keysToCheck.Where(x => x.StartsWith(startOfKeyToSearchFor)).ToList();
					if (searchResult.Count > 1)
					{
						duplicateKeysToKeep.Add(GetHighestValuedDuplicateKey(searchResult));
					}
				}
			}
			return duplicateKeysToKeep;
		}

		private DuplicateKeyToKeep GetHighestValuedDuplicateKey(List<string> searchResult)
		{
			var keyToKeepInfo = new DuplicateKeyToKeep();
			var keyToKeep = string.Empty;
			foreach (var currentKey in searchResult)
			{
				if (string.IsNullOrEmpty(keyToKeep))
				{
					keyToKeep = currentKey;
				}
				else
				{
					var keyToKeepNumber = 0;
					var currentKeyNumber = 0;
					var lastDigitsRegExp = new Regex(@"(\d{1,6})$");
					var keyToKeepNumberAsString = lastDigitsRegExp.Match(keyToKeep).Value;
					var currentKeyNumberAsString = lastDigitsRegExp.Match(currentKey).Value;
					if (!string.IsNullOrEmpty(keyToKeepNumberAsString))
					{
						int.TryParse(keyToKeepNumberAsString, out keyToKeepNumber);
					}

					if (!string.IsNullOrEmpty(currentKeyNumberAsString))
					{
						int.TryParse(currentKeyNumberAsString, out currentKeyNumber);
					}

					if (currentKeyNumber > keyToKeepNumber)
					{
						keyToKeepInfo.AddKeyToRemove(keyToKeep);
					}
					else
					{
						keyToKeepInfo.AddKeyToRemove(currentKey);
					}
				}
			}

			keyToKeepInfo.FullKey = keyToKeep;
			return keyToKeepInfo;
		}
	}

	internal class DuplicateKeyToKeep
	{
		private List<string> _keysToRemove;
		public string FullKey { get; set; }
		public List<string> KeysToRemove => _keysToRemove;

		public void AddKeyToRemove(string keyToRemove)
		{
			if (_keysToRemove == null) _keysToRemove = new List<string>();
			_keysToRemove.Add(keyToRemove);
		}
	}

	internal class KeyToReplaceAndReplacement
	{
		public string OriginalKey { get; set; }
		public string NewKey { get; set; }
	}
}