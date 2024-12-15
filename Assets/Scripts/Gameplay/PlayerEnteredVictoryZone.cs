using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine;
using UnityEngine.SceneManagement; // Wichtig!

namespace Platformer.Gameplay
{
    /// <summary>
    /// This event is triggered when the player character enters a trigger with a VictoryZone component.
    /// </summary>
    /// <typeparam name="PlayerEnteredVictoryZone"></typeparam>
    public class PlayerEnteredVictoryZone : Simulation.Event<PlayerEnteredVictoryZone>
    {
        public VictoryZone victoryZone;

        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            Debug.Log("Player entered victory zone");

            // Spieleranimation für den Sieg
            model.player.animator.SetTrigger("victory");
            model.player.controlEnabled = false;

            // Lade das nächste Level nach einer kurzen Verzögerung
            Simulation.Schedule<LoadNextLevel>(1.5f); // 1.5 Sekunden Verzögerung
        }
    }

    /// <summary>
    /// Event zum Laden des nächsten Levels.
    /// </summary>
    public class LoadNextLevel : Simulation.Event<LoadNextLevel>
    {
        public override void Execute()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = currentSceneIndex + 1;

            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                Debug.Log($"Loading next level: {nextSceneIndex}");
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.Log("No more levels to load. End of game!");
                // Optional: Lade das Hauptmenü oder ein Endbildschirm
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}
