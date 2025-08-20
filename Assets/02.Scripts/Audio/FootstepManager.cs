using UnityEngine;
using Constants;

public class FootstepManager : Singleton<FootstepManager>
{
    [SerializeField] private AudioSource audioSource;

    public void PlayFootstep(Vector3 myPos)
    {
        if (Physics.Raycast(myPos, Vector3.down, out RaycastHit hit, 1.5f))
        {
            string materialTag = hit.collider.tag;
            SoundData soundData = Resources.Load<SoundData>(AudioConstants.FootstepPath + materialTag);
            AudioManager.Instance.PlaySFX(soundData, hit.point);
        }
    }
}