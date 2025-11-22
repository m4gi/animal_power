namespace Game.Scripts
{
    public partial class AnimalUnit
    {
        public void UseSkill()
        {
            skill.StartCast();
        }

        public void EndSkill()
        {
            skill.EndCast();
        }
    }
}