#nullable enable
using BaseGame.Managers;
using System.Diagnostics.CodeAnalysis;
using Unity.Netcode;

namespace BaseGame
{
    public class Connection : NetworkBehaviour, IComponent
    {
        private User? user;

        public static Connection? LocalPlayer
        {
            get
            {
                foreach (Connection connection in PlayerLoop.GetAll<Connection>())
                {
                    if (connection.IsLocalPlayer)
                    {
                        return connection;
                    }
                }

                return null;
            }
        }

        public User User
        {
            get
            {
                if (user is null || !user)
                {
                    user = CreateUser();
                }

                return user;
            }
        }

        private void OnEnable()
        {
            PlayerLoop.Add(this);
        }

        private void OnDisable()
        {
            PlayerLoop.Remove(this);
        }

        public override void OnNetworkSpawn()
        {
            _ = User;
        }

        private User CreateUser() => PlayerManager.CreateUser(OwnerClientId);

        public static bool TryGet(User user, [NotNullWhen(true)] out Connection? connection)
        {
            foreach (Connection c in PlayerLoop.GetAll<Connection>())
            {
                if (c.User == user)
                {
                    connection = c;
                    return true;
                }
            }

            connection = null;
            return false;
        }
    }
}