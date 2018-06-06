[BoltGlobalBehaviour(BoltNetworkModes.Host, "Level1")]
public class Callbacks : Bolt.GlobalEventListener
{
    public override void LoadSceneLocal(string map)
    {
        BoltNetwork.Instantiate(BoltPrefabs.Player);
    }
    public override void LoadSceneRemote(BoltConnection connection)
    {
        BoltNetwork.Instantiate(BoltPrefabs.Player);
    }}