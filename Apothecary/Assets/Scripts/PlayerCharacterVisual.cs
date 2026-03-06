using Unity.Netcode;
using UnityEngine;

public class PlayerCharacterVisual : NetworkBehaviour
{
    [SerializeField] private GameObject mage01;
    [SerializeField] private GameObject mage02;

    // 0 = Mage_01
    // 1 = Mage_02
    private NetworkVariable<int> characterId = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public int CurrentId => characterId.Value;

    public override void OnNetworkSpawn()
    {
        // When the characterId changes over the network, reapply the correct model locally
        characterId.OnValueChanged += (_, __) => Apply();

        // Apply character when object first spawns
        Apply();
    }

    // Called by server when assigning which character a player should use
    public void SetCharacterServer(int id)
    {
        if (!IsServer) return;

        characterId.Value = id;
    }

    private void Apply()
    {
        // Activate character model based on characterId
        if (mage01) mage01.SetActive(characterId.Value == 0);
        if (mage02) mage02.SetActive(characterId.Value == 1);

        var animDriver = GetComponent<PlayerAnimation>();

        if (animDriver != null)
        {
            Animator a = null;

            if (characterId.Value == 0 && mage01 != null)
                a = mage01.GetComponentInChildren<Animator>(true);

            if (characterId.Value == 1 && mage02 != null)
                a = mage02.GetComponentInChildren<Animator>(true);

            // this lets movement script control the correct models animation
            animDriver.worldAnimator = a;
        }
    }
}