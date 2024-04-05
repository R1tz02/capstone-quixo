using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public static class Data
{
    public static string CURRENT_LANGUAGE = "English";

    public static Dictionary<string, Dictionary<string, string>> LOCALIZATION =
        new Dictionary<string, Dictionary<string, string>>()
        {
            { "easyAI_key", new Dictionary<string, string>()
            {
                {"English", "Easy" },
                {"Español", "Facil" }
            }
            },
            { "hardAI_key", new Dictionary<string, string>()
            {
                {"English", "Hard" },
                {"Español", "Dificil" }
            }
            },
            { "quickplayAI_key", new Dictionary<string, string>()
            {
                {"English", "Quickplay" },
                {"Español", "Partida Rapida" }
            }
            },
            { "settings_key", new Dictionary<string, string>()
            {
                {"English", "Learn to Play" },
                {"Español", "Aprende a Jugar" }
            }
            },
            { "online_key", new Dictionary<string, string>()
            {
                {"English", "Online" },
                {"Español", "En Linea" }
            }
            },
            { "multiplayer_key", new Dictionary<string, string>()
            {
                {"English", "Multiplayer" },
                {"Español", "Multijugador" }
            }
            },
            { "host_key", new Dictionary<string, string>()
            {
                {"English", "Host" },
                {"Español", "Crear Sala" }
            }
            },
            { "join_key", new Dictionary<string, string>()
            {
                {"English", "Join" },
                {"Español", "Unirse" }
            }
            },
            { "joinLobby_key", new Dictionary<string, string>()
            {
                {"English", "Join Lobby" },
                {"Español", "Unirse A Sala" }
            }
            },
            { "story_key", new Dictionary<string, string>()
            {
                {"English", "Story Mode" },
                {"Español", "Modo Campaña" }
            }
            },
            { "help_key", new Dictionary<string, string>()
            {
                {"English", "Choose a clear ingot or an already hot ingot that is located on the edge of the forge. Once selected the player can move their piece to the end of one of the now incomplete rows and shift the other pieces. As a blacksmith your goal is to align five hot ingots in a row either diagonally, vertically or horizontally before the forge cools down by accomplishing the same task with cold coals." },
                {"Español", "Seleccione un lingote claro o un lingote ya caliente que se encuentre English el borde de la fragua. Una vez seleccionado, el jugador puede mover su pieza al final de una de las filas ahora incompletas y empujar las otras piezas. Como herrero, tu objetivo Español alinear cinco lingotes calientes English fila, ya sea English diagonal, vertical u horizontal antes de que la forja se enfrie, con los lingotes frios." }
            }
            },
            { "congrats_key", new Dictionary<string, string>()
            {
                {"English", "Congratulations" },
                {"Español", "Felicidades" }
            }
            },
            { "restart_key", new Dictionary<string, string>()
            {
                {"English", "Restart" },
                {"Español", "Reiniciar" }
            }
            },
            { "restartOne_key", new Dictionary<string, string>()
            {
                {"English", "Restart (1/2)" },
                {"Español", "Reiniciar (1/2)" }
            }
            },
            { "returnToMain_key", new Dictionary<string, string>()
            {
                {"English", "Return to Main Menu" },
                {"Español", "Volver al Menu Principal" }
            }
            },
            { "loss_key", new Dictionary<string, string>()
            {
                {"English", "You lost! AI won!" },
                {"Español", "Perdiste! IA gano!" }
            }
            },
            { "tie_key", new Dictionary<string, string>()
            {
                {"English", "It's a TIE!" },
                {"Español", "Es un EMPATE!" }
            }
            },
            { "drawDeny_key", new Dictionary<string, string>()
            {
                {"English", "Draw Denied!" },
                {"Español", "Empate Negado!" }
            }
            },
            { "resume_key", new Dictionary<string, string>()
            {
                {"English", "Resume" },
                {"Español", "Reanudar" }
            }
            },
            { "accept_key", new Dictionary<string, string>()
            {
                {"English", "Accept" },
                {"Español", "Aceptar" }
            }
            },
            { "deny_key", new Dictionary<string, string>()
            {
                {"English", "Deny" },
                {"Español", "Negar" }
            }
            },
            { "pause_key", new Dictionary<string, string>()
            {
                {"English", "Pause" },
                {"Español", "Pausa" }
            }
            },
            { "helpWord_key", new Dictionary<string, string>()
            {
                {"English", "Help" },
                {"Español", "Ayuda" }
            }
            },
            { "lvl2_key", new Dictionary<string, string>()
            {
                {"English", "Level 2" },
                {"Español", "Nivel 2" }
            }
            },
            { "lvl3_key", new Dictionary<string, string>()
            {
                {"English", "Level 3" },
                {"Español", "Nivel 3" }
            }
            },
            { "lvl4_key", new Dictionary<string, string>()
            {
                {"English", "Level 4 (Final Level)" },
                {"Español", "Nivel 4 (Nivel Final)" }
            }
            },
            { "otherModes_key", new Dictionary<string, string>()
            {
                {"English", "Try Another Mode" },
                {"Español", "Prueba Otro Modo" }
            }
            },
            { "leftDiag_key", new Dictionary<string, string>()
            {
                {"English", "Left Diagonal" },
                {"Español", "Diagonal Izquierda" }
            }
            },
            { "rightDiag_key", new Dictionary<string, string>()
            {
                {"English", "Right Diagonal" },
                {"Español", "Diagonal Derecha" }
            }
            },
            { "step1_key", new Dictionary<string, string>()
            {
                {"English", "The yellow cubes are your ingots. The black cubes are cooled coals. Select the highlighted grey cube." },
                {"Español", "Los cubos amarillos son tus lingotes. Los cubos negros son carbones enfriados. Seleccione el cubo gris resaltado." }
            }
            },
            { "step2_key", new Dictionary<string, string>()
            {
                {"English", "These arrows indicate which direction a piece can move. When an arrow is red, it indicates the piece can move that direction. Select the red arrow." },
                {"Español", "Estas flechas indican en que direccion se puede mover una pieza. Cuando una flecha Español roja, indica que la pieza puede moverse en esa direccion. Seleccione la flecha roja." }
            }
            },
            { "offerDraw_key", new Dictionary<string, string>()
            {
                {"English", "Offer Draw"},
                {"Español", "Ofrecer Empate"}
            }
            },
            { "vertical_key", new Dictionary<string, string>()
            {
                {"English", "Vertical"},
                {"Español", "Vertical"} 
            }
            },
            { "horizontal_key", new Dictionary<string, string>()
            {
                {"English", "Horizontal"},
                {"Español", "Horizontal"}
            }
            },
            { "local_key", new Dictionary<string, string>()
            {
                {"English", "Local"},
                {"Español", "Local"}
            }
            },
            { "single_key", new Dictionary<string, string>()
            {
                {"English", "SinglePlayer"},
                {"Español", "Individual"}
            }
            },
            { "order_key", new Dictionary<string, string>()
            {
                {"English", "Turn Order"},
                {"Español", "Individual"} // Fernando needs to translate
            }
            },
            { "first_key", new Dictionary<string, string>()
            {
                {"English", "First"},
                {"Español", "Individual"} // Fernando needs to translate
            }
            },
            { "second_key", new Dictionary<string, string>()
            {
                {"English", "Second"},
                {"Español", "Individual"} // Fernando needs to translate
            }
            },
            { "mediumAI_key", new Dictionary<string, string>()
            {
                {"English", "Medium"},
                {"Español", "Individual"} // Fernando needs to translate
            }
            },
        };

    public static string[] LANGUAGES = new string[] { "English", "Español" };
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

