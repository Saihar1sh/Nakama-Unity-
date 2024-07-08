using Nakama;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace shGames.Network
{
    [CreateAssetMenu(menuName = "Nakama/NakamaConnection", fileName = "New NakamaConnection", order = 0)]
    public class NakamaConnection : ScriptableObject
    {
        #region Private Variables

        public string scheme = "http";
        public string host = "127.0.0.1";
        public int port = 7350;
        public string serverKey = "defaultkey";

        private IClient client;
        private ISession session;
        private ISocket socket;
        
        public ISocket Socket => socket;
        public ISession Session => session;
        public IClient Client => client;

        /// <summary>
        /// Matchmaker Ticket; Used to find or cancel a match
        /// </summary>
        private string currentMatchmakingTicket;
        private string currentMatchID;

        #endregion


        #region Public Functions

        public async Task Connect()
        {
            client = new Client(scheme, host, port, serverKey, UnityWebRequestAdapter.Instance);

            string authToken = NetworkUtils.GetSessionAuthToken();
            if (string.IsNullOrEmpty(authToken))
            {
                var session = Nakama.Session.Restore(authToken);
                if (session != null && !session.IsExpired)
                {
                    this.session = session;
                }
            }

            if (this.session == null)
            {
                string deviceID = NetworkUtils.GetDeviceID();

                session = await client.AuthenticateDeviceAsync(deviceID);

                NetworkUtils.SetSessionAuthToken(session.AuthToken);
            }


            socket = client.NewSocket();

            await socket.ConnectAsync(session, appearOnline: true);

            

            Debug.Log(session);
            Debug.Log(socket);
        }

        

        #endregion


        #region Other Functions

        public async void FindMatch()
        {
            Debug.Log("Finding Match");
            var matchmakingTicket = await socket.AddMatchmakerAsync(minCount: 2, maxCount: 3);
            
            currentMatchmakingTicket = matchmakingTicket.Ticket;
        }

        public async Task CancelMatchmaking()
        {
            await socket.RemoveMatchmakerAsync(currentMatchmakingTicket);
        }

        #endregion
    }
}