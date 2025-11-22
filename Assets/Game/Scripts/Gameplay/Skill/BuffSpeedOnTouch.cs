namespace Game.Scripts
{
    public class BuffSpeedOnTouch : AnimalSkill
    {
        public float multiplier;
        
        public override void StartCast()
        {
            lane.central.SetPushSpeed(multiplier);
        }

        public override void EndCast()
        {
            lane.central.ResetPushSpeed();
        }
    }
}