using Nakama;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace shGames.Network
{
    public class NakamaConnection : MonoBehaviour
    {
        #region Private Variables
        private const string scheme = "http";
        private const string host = "127.0.0.1";
        private const int port = 7350;
        private const string serverKey = "defaultkey";

        private IClient client;
        private ISession session;
        private ISocket socket;
        #endregion


        #region Unity Functions
        private async void Start()
        {
            client = new Client(scheme, host, port, serverKey, UnityWebRequestAdapter.Instance);

            string authToken = NetworkUtils.GetSessionAuthToken();
            if (string.IsNullOrEmpty(authToken))
            {
                var session = Session.Restore(authToken);
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

    }
}