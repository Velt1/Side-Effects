using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when a player collides with a potion.
    /// </summary>
    /// <typeparam name="PlayerCollision"></typeparam>
    public class PlayerPotionCollision : Simulation.Event<PlayerPotionCollision>
    {
        public PlayerController player;
        public PotionInstance potion;

        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            AudioSource.PlayClipAtPoint(potion.potionCollectAudio, potion.transform.position);
        }
    }
}