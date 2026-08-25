using System;
using UnityEngine;

internal enum BotanicaLanguage { PortugueseBrazilian, English, Spanish }

internal static class BotanicaLocalization {
    private static GameSettings _activeSettings;
    private static bool _languageLookupWarningLogged;

    internal static void SetActiveSettings(GameSettings settings) {
        if (settings != null) _activeSettings = settings;
    }

    internal static BotanicaLanguage CurrentLanguage {
        get {
            if (BotanicaSettings.LanguageMode == BotanicaLanguageMode.PortugueseBrazilian)
                return BotanicaLanguage.PortugueseBrazilian;
            if (BotanicaSettings.LanguageMode == BotanicaLanguageMode.Spanish)
                return BotanicaLanguage.Spanish;
            if (BotanicaSettings.LanguageMode == BotanicaLanguageMode.English)
                return BotanicaLanguage.English;
            return DetectGameLanguage();
        }
    }

    internal static string LanguageCode {
        get {
            if (CurrentLanguage == BotanicaLanguage.PortugueseBrazilian) return "pt-BR";
            if (CurrentLanguage == BotanicaLanguage.Spanish) return "es";
            return "en";
        }
    }

    internal static string CommonName(BotanicaEntry entry) {
        if (CurrentLanguage == BotanicaLanguage.PortugueseBrazilian) return entry.Portuguese;
        if (CurrentLanguage == BotanicaLanguage.Spanish) return entry.Spanish;
        return entry.English;
    }

    internal static string ConfidenceLabel(string confidence) {
        BotanicaLanguage language = CurrentLanguage;
        string normalized = confidence ?? string.Empty;
        if (language == BotanicaLanguage.PortugueseBrazilian) {
            if (normalized == "alta") return "identificação: alta confiança";
            if (normalized == "media_alta") return "identificação: confiança média-alta";
            if (normalized == "media") return "identificação: confiança média";
            if (normalized == "alta_para_genero") return "identificação segura até o gênero";
            if (normalized == "baixa_para_especie") return "espécie não confirmada";
            return "identificação botânica";
        }
        if (language == BotanicaLanguage.Spanish) {
            if (normalized == "alta") return "identificación: confianza alta";
            if (normalized == "media_alta") return "identificación: confianza media-alta";
            if (normalized == "media") return "identificación: confianza media";
            if (normalized == "alta_para_genero") return "identificación segura hasta el género";
            if (normalized == "baixa_para_especie") return "especie no confirmada";
            return "identificación botánica";
        }
        if (normalized == "alta") return "identification: high confidence";
        if (normalized == "media_alta") return "identification: medium-high confidence";
        if (normalized == "media") return "identification: medium confidence";
        if (normalized == "alta_para_genero") return "identification reliable to genus";
        if (normalized == "baixa_para_especie") return "species not confirmed";
        return "botanical identification";
    }

    internal static string SynonymLabel { get {
        if (CurrentLanguage == BotanicaLanguage.PortugueseBrazilian) return "sinônimo";
        if (CurrentLanguage == BotanicaLanguage.Spanish) return "sinónimo";
        return "synonym";
    } }

    internal static string GameIdentificationLabel { get {
        if (CurrentLanguage == BotanicaLanguage.PortugueseBrazilian) return "nome usado pelo jogo";
        if (CurrentLanguage == BotanicaLanguage.Spanish) return "nombre usado por el juego";
        return "name used by the game";
    } }

    internal static string Message(string portuguese, string english, string spanish) {
        if (CurrentLanguage == BotanicaLanguage.PortugueseBrazilian) return portuguese;
        if (CurrentLanguage == BotanicaLanguage.Spanish) return spanish;
        return english;
    }

    private static BotanicaLanguage DetectGameLanguage() {
        try {
            if (_activeSettings == null) {
                GameSettings[] settings = Resources.FindObjectsOfTypeAll<GameSettings>();
                for (int i = 0; settings != null && i < settings.Length; i++) {
                    if (settings[i] != null && settings[i].isActiveAndEnabled) {
                        _activeSettings = settings[i];
                        break;
                    }
                }
                if (_activeSettings == null && settings != null && settings.Length > 0)
                    _activeSettings = settings[0];
            }
            if (_activeSettings != null) {
                Enums.Language language = _activeSettings.m_Language;
                if (language == Enums.Language.PortugueseBrazilian ||
                    language == Enums.Language.Portuguese)
                    return BotanicaLanguage.PortugueseBrazilian;
                if (language == Enums.Language.Spanish) return BotanicaLanguage.Spanish;
            }
        } catch (Exception exception) {
            if (!_languageLookupWarningLogged) {
                _languageLookupWarningLogged = true;
                Debug.LogWarning("[Botany Discovery] Language detection failed; using English: " + exception.Message);
            }
        }
        return BotanicaLanguage.English;
    }
}
