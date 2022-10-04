#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;

namespace BaseGame
{
    [Serializable]
    public class UserBehaviour : IUpdateLoop
    {
        private int userId;
        private User? user;

        /// <summary>
        /// The player that this user behaviour belongs to.
        /// </summary>
        public User User
        {
            get
            {
                if (user is null)
                {
                    user = PlayerLoop.Get<User>(new ID(userId));
                    if (user is null)
                    {
                        //orphan :(
                        throw ExceptionBuilder.Format("User with ID {0} does not exist.", userId);
                    }
                }

                return user;
            }
        }

        public UserBehaviour(User user)
        {
            userId = user.ID.GetHashCode();
            this.user = user;
        }

        protected virtual void OnUpdate(float delta) { }
        protected virtual void OnUpdate(float delta, IPlayer player) { }

        public virtual bool TryGetInputState(ReadOnlySpan<char> name, [MaybeNullWhen(false)] out InputState state)
        {
            state = default;
            return false;
        }

        public static UserBehaviour Create(Type userBehaviourType, User user)
        {
            return (UserBehaviour)Activator.CreateInstance(userBehaviourType, new object[] { user });
        }

        void IUpdateLoop.OnUpdate(float delta)
        {
            OnUpdate(delta);
            if (User.Player is IPlayer player)
            {
                OnUpdate(delta, player);
            }
        }
    }
}