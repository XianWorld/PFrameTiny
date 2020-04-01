using PFrame.Entities;
using PFrame.Entities.Authoring;
using Unity.Entities;
using UnityEngine;

namespace PFrame.Tiny.Authoring
{
    [CreateAssetMenu(fileName = "FontData_", menuName = "TinySTS/FontData")]
    public class FontDataSO : AGameDataSO
    {
        public override byte DataType => (byte)ECommonGameDataType.Font;

        public UnityEngine.Font Font;
    }
}
