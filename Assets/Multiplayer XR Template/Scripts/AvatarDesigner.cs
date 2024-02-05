using UnityEngine;
using UnityEngine.Serialization;

namespace Alteruna
{
    public class AvatarDesigner : CommunicationBridge
    {
        [FormerlySerializedAs("hats")] [SerializeField]
        private Accessors[] accessors;

        [SerializeField]
        private MeshFilter[] slots;

        public override void Possessed(bool isMe, User user)
        {
            GenerateAvatar(user.Name.GetHashCode());
            Multiplayer.Connect();
        }

        public void GenerateAvatar(int seed)
        {
            System.Random random = new System.Random(seed);
            
            int[] slotCount = new int[slots.Length];
            foreach (Accessors hat in accessors)
            {
                slotCount[hat.slot]++;
            }
            
            for (int i = 0; i < slots.Length; i++)
            {
                if (slotCount[i] > 0)
                {
                    int meshId = random.Next(slotCount[i]);
                    int j = 0;
                    
                    foreach (Accessors hat in accessors)
                    {
                        if (hat.slot != i || j++ != meshId)
                        {
                            continue;
                        }

                        if (hat.mesh != null && (hat.blockSlot < 0 || !slots[hat.blockSlot].gameObject.activeSelf))
                        {
                            slots[i].mesh = hat.mesh;
                            if (hat.materials != null && hat.materials.Length > 0)
                            {
                                slots[i].GetComponent<MeshRenderer>().material = hat.materials[random.Next(hat.materials.Length)];
                            }
                            slots[i].gameObject.SetActive(true);
                            break;
                        }
                                
                        slots[i].gameObject.SetActive(false);
                        break;
                    }
                    
                }
                else
                {
                    slots[i].gameObject.SetActive(false);
                }
            }
        }

        [System.Serializable]
        public class Accessors
        {
            public int slot;
            public int blockSlot = -1;
            public Mesh mesh;
            public Material[] materials;
        }
    }
}
