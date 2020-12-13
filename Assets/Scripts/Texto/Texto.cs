using System.Collections;
using System.Collections.Generic;
using PHL.Common.Utility;
using UnityEngine;

namespace PHL.Texto
{
    public enum TextoLanguage
    {
        None,
        English,
        French,
        Italian,
        German,
        Spanish,
        LatinSpanish,
        BrazilianPortuguese,
        SimplifiedChinese,
        Russian,
        Japanese,
        Arabic,
        Polish
    }

    //Static Texto-related functions
    public static class Texto
    {
        public static TextoLanguage currentLanguage { get; private set; }
        public static SecureEvent languageUpdatedEvent { get; private set; } = new SecureEvent();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            ChooseApplicationLanguage();
        }

        public static void SetLanguage(TextoLanguage newLanguage)
        {
            currentLanguage = newLanguage;
            languageUpdatedEvent.Invoke();
        }

        private static void ChooseApplicationLanguage()
        {
            if(TextoSettingsData.instance.overrideLanguage != TextoLanguage.None)
            {
                SetLanguage(TextoSettingsData.instance.overrideLanguage);
                return;
            }

            switch (Application.systemLanguage)
            {
                case SystemLanguage.English:
                    SetLanguage(TextoLanguage.English);
                    break;
                case SystemLanguage.French:
                    SetLanguage(TextoLanguage.French);
                    break;
                case SystemLanguage.Italian:
                    SetLanguage(TextoLanguage.Italian);
                    break;
                case SystemLanguage.German:
                    SetLanguage(TextoLanguage.German);
                    break;
                case SystemLanguage.Spanish:
                    SetLanguage(TextoLanguage.Spanish);
                    break;
                case SystemLanguage.Portuguese:
                    SetLanguage(TextoLanguage.BrazilianPortuguese);
                    break;
                case SystemLanguage.ChineseSimplified:
                    SetLanguage(TextoLanguage.SimplifiedChinese);
                    break;
                case SystemLanguage.ChineseTraditional:
                    SetLanguage(TextoLanguage.SimplifiedChinese);
                    break;
                case SystemLanguage.Chinese:
                    SetLanguage(TextoLanguage.SimplifiedChinese);
                    break;
                case SystemLanguage.Russian:
                    SetLanguage(TextoLanguage.Russian);
                    break;
                case SystemLanguage.Japanese:
                    SetLanguage(TextoLanguage.Japanese);
                    break;
                case SystemLanguage.Arabic:
                    SetLanguage(TextoLanguage.Arabic);
                    break;
                case SystemLanguage.Polish:
                    SetLanguage(TextoLanguage.Polish);
                    break;
                default:
                    SetLanguage(TextoLanguage.English);
                    break;
            }
        }

        public static TextoLanguage StringToLanguage(string languageString)
        {
            languageString = languageString.ToLower().Replace(" ", "");

            if (languageString == "english")
            {
                return TextoLanguage.English;
            }
            else if (languageString == "french")
            {
                return TextoLanguage.French;
            }
            else if (languageString == "italian")
            {
                return TextoLanguage.Italian;
            }
            else if (languageString == "german")
            {
                return TextoLanguage.German;
            }
            else if (languageString == "spanish")
            {
                return TextoLanguage.Spanish;
            }
            else if (languageString == "latinspanish")
            {
                return TextoLanguage.LatinSpanish;
            }
            else if (languageString == "brazilianportuguese")
            {
                return TextoLanguage.BrazilianPortuguese;
            }
            else if (languageString == "simplifiedchinese")
            {
                return TextoLanguage.SimplifiedChinese;
            }
            else if (languageString == "russian")
            {
                return TextoLanguage.Russian;
            }
            else if (languageString == "japanese")
            {
                return TextoLanguage.Japanese;
            }
            else if (languageString == "arabic")
            {
                return TextoLanguage.Arabic;
            }
            else if (languageString == "polish")
            {
                return TextoLanguage.Polish;
            }

            return TextoLanguage.None;
        }
    }
}
