using UnityEngine;

namespace Magi.Scripts.GameData
{
    public class GameConst
    {
    }
    
    public static class AnimatorConst
    {
        public static readonly int WalkTrigger = Animator.StringToHash("WalkTrigger");
        public static readonly int AttackTrigger = Animator.StringToHash("AttackTrigger");
        public static readonly int DieTrigger = Animator.StringToHash("DieTrigger");
    }

    public class SceneConst
    {
        public const string BootScene = "0_Boot";
        public const string MainScene = "1_MainMenu";
        public const string GameScene = "2_MainGame";
    }

    public class MusicConst
    {
        public const string MainMenuMusic = "main_menu_music";
        public const string MainGameMusic = "main_game_music";
    }

    public class SFXConst
    {
        public const string HitBase = "hit_base";
        public const string SpawnAnimal = "spawn_animal";
    }
}