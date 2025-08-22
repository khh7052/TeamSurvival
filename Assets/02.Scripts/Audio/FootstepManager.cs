using UnityEngine;
using Constants;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;

public class FootstepManager : Singleton<FootstepManager>
{
    public async void PlayFootstep(Vector3 myPos)
    {
        if (Physics.Raycast(myPos, Vector3.down, out RaycastHit hit, 1.5f))
        {
            string tag = hit.collider.tag;
            SoundData soundData = await AudioManager.LoadSoundData(tag);
            AudioManager.Instance.PlaySFX(soundData, hit.point);
        }
    }

}