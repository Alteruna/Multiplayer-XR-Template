namespace Alteruna
{
    public class SetEnableStatusOnPossession : CommunicationBridge
    {
        public bool EnableStatusForOwner = true;
        
        public override void Possessed(bool isMe, User user)
        {
            if (!isMe) EnableStatusForOwner = !EnableStatusForOwner;
            
            gameObject.SetActive(EnableStatusForOwner);
        }
    }
}
