using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public static class Data
{
    public static string CURRENT_LANGUAGE = "en";

    public static Dictionary<string, Dictionary<string, string>> LOCALIZATION =
        new Dictionary<string, Dictionary<string, string>>()
        {
            { "easyAI_key", new Dictionary<string, string>()
            {
                {"en", "Easy" },
                {"es", "Facil" }
            }
            },
            { "hardAI_key", new Dictionary<string, string>()
            {
                {"en", "Hard" },
                {"es", "Dificil" }
            }
            },
            { "quickplayAI_key", new Dictionary<string, string>()
            {
                {"en", "Quickplay" },
                {"es", "Partida Rapida" }
            }
            },
            { "settings_key", new Dictionary<string, string>()
            {
                {"en", "Learn to Play" },
                {"es", "Aprende a Jugar" }
            }
            },
            { "online_key", new Dictionary<string, string>()
            {
                {"en", "Online" },
                {"es", "En Linea" }
            }
            },
            { "multiplayer_key", new Dictionary<string, string>()
            {
                {"en", "Multiplayer" },
                {"es", "Multijugador" }
            }
            },
            { "host_key", new Dictionary<string, string>()
            {
                {"en", "Host" },
                {"es", "Crear Sala" }
            }
            },
            { "join_key", new Dictionary<string, string>()
            {
                {"en", "Join" },
                {"es", "Unirse" }
            }
            },
            { "joinLobby_key", new Dictionary<string, string>()
            {
                {"en", "Join Lobby" },
                {"es", "Unirse A Sala" }
            }
            },
            { "story_key", new Dictionary<string, string>()
            {
                {"en", "Story Mode" },
                {"es", "Modo Campa�a" }
            }
            },
            { "help_key", new Dictionary<string, string>()
            {
                {"en", "Choose a clear ingot or an already hot ingot that is located on the edge of the forge. Once selected the player can move their piece to the end of one of the now incomplete rows and shift the other pieces. As a blacksmith your goal is to align five hot ingots in a row either diagonally, vertically or horizontally before the forge cools down by accomplishing the same task with cold coals." },
                {"es", "Seleccione un lingote claro o un lingote ya caliente que se encuentre en el borde de la fragua. Una vez seleccionado, el jugador puede mover su pieza al final de una de las filas ahora incompletas y empujar las otras piezas. Como herrero, tu objetivo es alinear cinco lingotes calientes en fila, ya sea en diagonal, vertical u horizontal antes de que la forja se enfr�e, con los lingotes fr�os." }
            }
            },
            { "congrats_key", new Dictionary<string, string>()
            {
                {"en", "Congratulations" },
                {"es", "Felicidades" }
            }
            },
            { "restart_key", new Dictionary<string, string>()
            {
                {"en", "Restart" },
                {"es", "Reiniciar" }
            }
            },
            { "returnToMain_key", new Dictionary<string, string>()
            {
                {"en", "Return to Main Menu" },
                {"es", "Volver al Menu Principal" }
            }
            },
            { "loss_key", new Dictionary<string, string>()
            {
                {"en", "You lost! AI won!" },
                {"es", "Perdiste! IA gano!" }
            }
            },
            { "tie_key", new Dictionary<string, string>()
            {
                {"en", "It's a TIE!" },
                {"es", "Es un EMPATE!" }
            }
            },
            { "drawDeny_key", new Dictionary<string, string>()
            {
                {"en", "Draw Denied!" },
                {"es", "Es un EMPATE!" }
            }
            },
            { "resume_key", new Dictionary<string, string>()
            {
                {"en", "Resume" },
                {"es", "Reanudar" }
            }
            },
            { "accept_key", new Dictionary<string, string>()
            {
                {"en", "Accept" },
                {"es", "Aceptar" }
            }
            },
            { "deny_key", new Dictionary<string, string>()
            {
                {"en", "Deny" },
                {"es", "Negar" }
            }
            },
            { "pause_key", new Dictionary<string, string>()
            {
                {"en", "Pause" },
                {"es", "Pausa" }
            }
            },
            { "helpWord_key", new Dictionary<string, string>()
            {
                {"en", "Help" },
                {"es", "Ayuda" }
            }
            },
            { "lvl2_key", new Dictionary<string, string>()
            {
                {"en", "Level 2" },
                {"es", "Nivel 2" }
            }
            },
            { "lvl3_key", new Dictionary<string, string>()
            {
                {"en", "Level 3" },
                {"es", "Nivel 3" }
            }
            },
            { "lvl4_key", new Dictionary<string, string>()
            {
                {"en", "Level 4 (Final Level)" },
                {"es", "Nivel 4 (Nivel Final)" }
            }
            },
            { "otherModes_key", new Dictionary<string, string>()
            {
                {"en", "Try Another Mode" },
                {"es", "Prueba Otro Modo" }
            }
            },
            { "leftDiag_key", new Dictionary<string, string>()
            {
                {"en", "Left Diagonal" },
                {"es", "Diagonal Izquierda" }
            }
            },
            { "rightDiag_key", new Dictionary<string, string>()
            {
                {"en", "Right Diagonal" },
                {"es", "Diagonal Derecha" }
            }
            },
            { "step1_key", new Dictionary<string, string>()
            {
                {"en", "The yellow cubes are your ingots. The black cubes are cooled coals. Select the highlighted grey cube." },
                {"es", "Los cubos amarillos son tus lingotes. Los cubos negros son carbones enfriados. Seleccione el cubo gris resaltado." }
            }
            },
            { "step2_key", new Dictionary<string, string>()
            {
                {"en", "These arrows indicate which direction a peice can move. When an arrow is red, it indicates the peice can move that direction. Select the red arrow." },
                {"es", "Estas flechas indican en qu� direcci�n se puede mover una pieza. Cuando una flecha es roja, indica que la pieza puede moverse en esa direcci�n. Seleccione la flecha roja." }
            }
            },
            { "offerDraw_key", new Dictionary<string, string>()
            {
                {"en", "Offer Draw"},
                {"es", "Sorteo De Oferta"}//Fernado needs to fix
            }
            },
            { "vertical_key", new Dictionary<string, string>()
            {
                {"en", "Vertical"},
                {"es", "Vertical"} //Fernado needs to fix
            }
            },
            { "horizontal_key", new Dictionary<string, string>()
            {
                {"en", "Horizontal"},
                {"es", "Horizontal"}//Fernado needs to fix
            }
            },
            { "local_key", new Dictionary<string, string>()
            {
                {"en", "Local"},
                {"es", "Local"}//Fernado needs to fix
            }
            },

        };

    public static string[] LANGUAGES = new string[] { "en", "es" };
    private static UnityEvent _OnLanguageChanged;
    public static UnityEvent OnLanguageChanged
    {
        get
        {
            if(_OnLanguageChanged == null)  _OnLanguageChanged = new UnityEvent();
            return _OnLanguageChanged;
        }
    }


}
